using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Authorization
{
    public class GroupAuthorizationHandler : IAuthorizationHandler
    {
        private const GroupRoles _allRoles = GroupRoles.Member | GroupRoles.Director | GroupRoles.Manager;

        private readonly OrchestrateContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupAuthorizationHandler(OrchestrateContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            string userIdStr = context.User.Identity.Name;
            if (userIdStr == null) return;

            if (!TryGetHttpRouteParam("groupId", out int groupId)) return;
            TryGetHttpRouteParam("roleId", out int roleId);

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(_ => _.Id == int.Parse(userIdStr));
            if (user == null) throw new UserNotExistException();

            var group = await _context.Groups
                .AsNoTracking()
                .Include(_ => _.Directors.Where(_ => _.Id == user.Id))
                .Include(_ => _.Members.Where(_ => _.UserId == user.Id)).ThenInclude(_ => _.Role)
                .FirstOrDefaultAsync(_ => _.Id == groupId);
            if (group == null) throw new ArgumentException("Group does not exist");

            var isUserManager = group.ManagerId == user.Id;
            var isUserDirector = group.Directors.Any();
            var memberRoles = group.Members.Select(_ => _.Role);

            _httpContextAccessor.HttpContext.Items["IsUserManager"] = isUserManager;
            _httpContextAccessor.HttpContext.Items["IsUserDirector"] = isUserDirector;
            _httpContextAccessor.HttpContext.Items["MemberRoles"] = memberRoles;

            var pendingRequirements = context.PendingRequirements.OfType<GroupRolesRequirement>().ToList();

            if (pendingRequirements.Count > 1) throw new NotSupportedException();

            bool noRequirements = pendingRequirements.Count == 0;

            // if route has groupId and no requirements specified, check all roles
            var roles = noRequirements ? _allRoles : pendingRequirements[0].Roles;

            if (HasRole(roles, GroupRoles.Manager) && isUserManager
                || HasRole(roles, GroupRoles.Director) && isUserDirector
                || HasRole(roles, GroupRoles.Member) && memberRoles.Any(r => roleId == 0 || r.Id == roleId))
            {
                if (!noRequirements) context.Succeed(pendingRequirements[0]);
            }
            else context.Fail();
        }

        private bool TryGetHttpRouteParam(string param, out int value)
        {
            object paramObj = null;
            if (_httpContextAccessor.HttpContext?.Request.RouteValues.TryGetValue(param, out paramObj) != true)
            {
                value = 0;
                return false;
            }

            value = Convert.ToInt32(paramObj);
            return true;
        }

        private bool HasRole(GroupRoles roles, GroupRoles role) => (roles & role) == role;
    }
}
