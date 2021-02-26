using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("user")]
    public class User : IEquatable<User>
    {
        public int Id { get; set; }

        [Required, StringLength(20, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required, StringLength(20, MinimumLength = 1)]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public bool IsPasswordTemporary { get; set; }

        [InverseProperty("Manager")]
        public ICollection<Group> ManagingGroups { get; set; }
        public ICollection<Group> DirectorOfGroups { get; set; }
        public ICollection<AssignedRole> Roles { get; set; }

        public bool Equals(User other) => Id == other.Id;
        public override int GetHashCode() => Id.GetHashCode();
    }
}
