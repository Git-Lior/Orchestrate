using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("concert_composition")]
    public class ConcertComposition
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int ConcertId { get; set; }
        public Concert Concert { get; set; }

        public int CompositionId { get; set; }
        public Composition Composition { get; set; }

        public int Order { get; set; }
    }
}
