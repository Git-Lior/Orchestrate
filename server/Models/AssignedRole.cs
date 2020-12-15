using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("assigned_role")]
    public class AssignedRole
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        
        public int RoleId { get; set; }
        public Role Role { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
