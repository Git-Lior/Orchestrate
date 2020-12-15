using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Models
{
    [Table("group")]
    public class Group
    {
        public int GroupId { get; set; }
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
