using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("concert_attendance")]
    public class ConcertAttendance
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public bool Attending { get; set; }
    }
}
