using System;

namespace Orchestrate.API.Authorization
{
    [Flags]
    public enum GroupRoles
    {
        Member = 1, Director = 2, Manager = 4
    }
}
