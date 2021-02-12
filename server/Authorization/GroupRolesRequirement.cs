using Microsoft.AspNetCore.Authorization;
using System;

namespace Orchestrate.API.Authorization
{
    [Flags]
    public enum GroupRoles
    {
        Player = 1, Director = 2, Manager = 4
    }

    public class GroupRolesRequirement : IAuthorizationRequirement
    {
        public GroupRoles Roles { get; }
        
        public GroupRolesRequirement(GroupRoles role)
        {
            Roles = role;
        }
    }
}
