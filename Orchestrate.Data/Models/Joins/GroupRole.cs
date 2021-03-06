﻿using System.Collections.Generic;

namespace Orchestrate.Data.Models.Joins
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
