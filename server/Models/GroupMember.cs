using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("group_member")]
    public class GroupMember
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
