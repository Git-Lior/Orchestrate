using System.Collections.Generic;

namespace Orchestrate.API.Models
{
    public class GroupRole
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public ICollection<User> Members { get; set; }

        public ICollection<SheetMusic> SheetMusics { get; set; }
    }
}
