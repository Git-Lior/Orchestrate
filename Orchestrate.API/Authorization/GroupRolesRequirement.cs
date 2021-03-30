using Microsoft.AspNetCore.Authorization;

namespace Orchestrate.API.Authorization
{
    public class GroupRolesRequirement : IAuthorizationRequirement
    {
        public GroupRoles Roles { get; }

        public GroupRolesRequirement(GroupRoles role)
        {
            Roles = role;
        }
    }
}
