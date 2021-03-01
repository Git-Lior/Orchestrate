using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("role")]
    public class Role : IEquatable<Role>
    {
        public int Id { get; set; }

        [Required, StringLength(15, MinimumLength = 2)]
        public string Section { get; set; }

        [Range(1, 10)]
        public int? Num { get; set; }

        public bool Equals(Role other) => Id == other.Id;
        public override int GetHashCode() => Id.GetHashCode();
    }
}
