using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Models
{
    [Table("role")]
    public class Role
    {
        public int RoleID { get; set; }
        public string Section { get; set; }
        public int RoleNum { get; set; }

        public ICollection<Group> InGroups { get; set; }
    }
}
