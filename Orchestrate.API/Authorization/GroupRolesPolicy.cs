namespace Orchestrate.API.Authorization
{
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
