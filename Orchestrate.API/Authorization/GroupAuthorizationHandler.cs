using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Orchestrate.Data.Interfaces;
using Orchestrate.Data.Models;

namespace Orchestrate.API.Authorization
{
    public class GroupAuthorizationHandler : IAuthorizationHandler
    {
        private const GroupRoles ALL_ROLES = GroupRoles.Member | GroupRoles.Director | GroupRoles.Manager;

        private readonly IEntityRepository<User> _usersRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserGroupPositionProvider _positionProvider;

        public GroupAuthorizationHandler(IEntityRepository<User> usersRepo, IHttpContextAccessor httpContextAccessor, IUserGroupPositionProvider userGroupPositionProvider)
        {
            _usersRepo = usersRepo;
            _positionProvider = userGroupPositionProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            string userIdStr = context.User.Identity.Name;
            if (userIdStr == null) return;

            var userId = int.Parse(userIdStr);

            if (await _usersRepo.Entities.AllAsync(_ => _.Id != userId)) throw new UserNotExistException();

            if (!TryGetHttpRouteParam("groupId", out int groupId)) return;
            TryGetHttpRouteParam("roleId", out int roleId);

            await _positionProvider.Initialize(userId, groupId);

            var pendingRequirements = context.PendingRequirements.OfType<GroupRolesRequirement>().ToList();

            if (pendingRequirements.Count > 1) throw new NotSupportedException();

            bool noRequirements = pendingRequirements.Count == 0;

            // if route has groupId and no requirements specified, check all roles
            var roles = noRequirements ? ALL_ROLES : pendingRequirements[0].Roles;

            if (HasRole(roles, GroupRoles.Manager) && _positionProvider.Manager
                || HasRole(roles, GroupRoles.Director) && _positionProvider.Director
                || HasRole(roles, GroupRoles.Member) && _positionProvider.Roles.Any(r => roleId == 0 || r.Id == roleId))
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
