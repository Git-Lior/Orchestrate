namespace Orchestrate.Data.Models.Joins
{
    public class GroupRoleMember
    {
        public int RoleId { get; set; }
        public int GroupId { get; set; }
        public GroupRole GroupRole { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
