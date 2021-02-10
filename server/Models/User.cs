using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("user")]
    public class User
    {
        public int Id { get; set; }
        
        [Required, StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }
        
        [Required, StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }
        
        [EmailAddress]
        public string Email { get; set; }
        
        public string PasswordHash { get; set; }
        
        public bool IsPasswordTemporary { get; set; }

        [InverseProperty("Manager")]
        public ICollection<Group> ManagingGroups { get; set; }
        public ICollection<Group> DirectorOfGroups { get; set; }
        public ICollection<AssignedRole> Roles { get; set; }
    }
}
