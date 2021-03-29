using Orchestrate.Data.Models.Joins;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.Data.Models
{
    public record UserIdentifier(int UserId);

    public class UserPayload
    {
        [Required, StringLength(20, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required, StringLength(20, MinimumLength = 1)]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }

    public class UserFields : UserPayload
    {
        [Required]
        public string PasswordHash { get; set; }

        public bool IsPasswordTemporary { get; set; }
    }

    public class User : UserFields
    {
        public int Id { get; set; }

        [InverseProperty("Manager")]
        public ICollection<Group> ManagingGroups { get; set; }
        public ICollection<Group> DirectorOfGroups { get; set; }
        public ICollection<GroupRole> MemberOfGroups { get; set; }

        public ICollection<ConcertAttendance> Attendances { get; set; }
    }
}
