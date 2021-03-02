using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    public class GroupPayload
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public int ManagerId { get; set; }
    }

    [Table("group")]
    public class Group : GroupPayload, IEquatable<Group>
    {
        public int Id { get; set; }

        public User Manager { get; set; }

        public ICollection<User> Directors { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<GroupMember> Members { get; set; }

        public ICollection<Composition> Compositions { get; set; }
        public ICollection<Concert> Concerts { get; set; }

        public bool Equals(Group other) => Id == other.Id;
        public override int GetHashCode() => Id.GetHashCode();
    }
}
