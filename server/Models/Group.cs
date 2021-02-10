using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("group")]
    public class Group
    {
        public int Id { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public int ManagerId { get; set; }
        public User Manager { get; set; }

        public ICollection<User> Directors { get; set; }
        public ICollection<Role> AvailableRoles { get; set; }
        public ICollection<AssignedRole> AssignedRoles { get; set; }

        public ICollection<Composition> Compositions { get; set; }
        public ICollection<Concert> Concerts { get; set; }
    }
}
