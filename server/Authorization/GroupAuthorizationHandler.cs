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

            if (_httpContextAccessor.HttpContext == null
                || !_httpContextAccessor.HttpContext.Request.RouteValues.TryGetValue("groupId", out object groupIdObj)) return;

            var pendingRequirements = context.PendingRequirements.OfType<GroupRolesRequirement>().ToList();

            if (pendingRequirements.Count != 1) return;

            var user = await _context.Users.FindAsync(int.Parse(userIdStr));
            if (user == null) throw new UserNotExistException();

            var group = await _context.Groups
                .AsNoTracking()
                .Include(_ => _.Directors)
                .Include(_ => _.AssignedRoles)
                .FirstOrDefaultAsync(_ => _.Id == (int)groupIdObj);
            if (group == null) throw new ArgumentException("Group does not exist");

            var roles = pendingRequirements[0].Roles;

            if (HasRole(roles, GroupRoles.Manager) && group.ManagerId == user.Id
                || HasRole(roles, GroupRoles.Director) && group.Directors.Any(_ => _.Id == user.Id)
                || HasRole(roles, GroupRoles.Player) && group.AssignedRoles.Any(_ => _.UserId == user.Id))
            {
                context.Succeed(pendingRequirements[0]);
            }
            else context.Fail();
        }

        private bool HasRole(GroupRoles roles, GroupRoles role) => (roles & role) == role;
    }
}
