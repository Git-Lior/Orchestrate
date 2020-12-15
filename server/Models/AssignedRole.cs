using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("assigned_role")]
    public class AssignedRole
    {
        public int GroupID { get; set; }
        public Group Group { get; set; }
        
        public int RoleID { get; set; }
        public Role Role { get; set; }
        
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
