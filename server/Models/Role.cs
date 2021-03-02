using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    public class RolePayload
    {
        [Required, StringLength(15, MinimumLength = 2)]
        public string Section { get; set; }

        [Range(1, 10)]
        public int? Num { get; set; }
    }

    [Table("role")]
    public class Role : RolePayload, IEquatable<Role>
    {
        public int Id { get; set; }

        public IEnumerable<Group> InGroups { get; set; }

        public bool Equals(Role other) => Id == other.Id;
        public override int GetHashCode() => Id.GetHashCode();
    }
}
