using Microsoft.AspNetCore.Authorization;
using System;

namespace Orchestrate.API.Authorization
{
    [Flags]
    public enum GroupRoles
    {
        Member = 1, Director = 2, Manager = 4
    }

    public class GroupRolesRequirement : IAuthorizationRequirement
    {
        public GroupRoles Roles { get; }

        public GroupRolesRequirement(GroupRoles role)
        {
            Roles = role;
        }
    }

    public static class GroupRolesPolicy
    {
        public const string AdministratorOnly = "AdministratorOnly";
        public const string MemberOnly = "MemberOnly";
        public const string DirectorOnly = "DirectorOnly";
        public const string DirectorOrMember = "DirectorOrMember";
        public const string ManagerOnly = "ManagerOnly";
        public const string ManagerOrMember = "ManagerOrMember";
    }
}
