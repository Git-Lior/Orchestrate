﻿using Orchestrate.Data.Models.Joins;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orchestrate.Data.Models
{
    public record GroupIdentifier(int GroupId);

    public class GroupPayload
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public int ManagerId { get; set; }
    }

    public class Group : GroupPayload
    {
        public int Id { get; set; }

        public User Manager { get; set; }

        public ICollection<User> Directors { get; set; }
        public ICollection<GroupRole> Roles { get; set; }

        public ICollection<Composition> Compositions { get; set; }
        public ICollection<Concert> Concerts { get; set; }
    }
}
