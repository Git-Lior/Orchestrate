using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("concert_attendance")]
    public class ConcertAttendance
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int ConcertId { get; set; }
        public Concert Concert { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public bool Attending { get; set; }
    }
}
