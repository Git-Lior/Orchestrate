using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    public class UserPayload
    {
        [Required, StringLength(20, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required, StringLength(20, MinimumLength = 1)]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }

    [Table("user")]
    public class User : UserPayload, IEquatable<User>
    {
        public int Id { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool IsPasswordTemporary { get; set; }

        [InverseProperty("Manager")]
        public ICollection<Group> ManagingGroups { get; set; }

        [InverseProperty("Directors")]
        public ICollection<Group> DirectorOfGroups { get; set; }
        public ICollection<GroupMember> MemberOfGroups { get; set; }

        public bool Equals(User other) => Id == other.Id;
        public override int GetHashCode() => Id.GetHashCode();
    }
}
