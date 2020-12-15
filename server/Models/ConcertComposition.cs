using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("concert_composition")]
    public class ConcertComposition
    {
        public int GroupID { get; set; }
        public Group Group { get; set; }

        public int ConcertID { get; set; }
        public Concert Concert { get; set; }

        public int CompositionID { get; set; }
        public Composition Composition { get; set; }

        public int Order { get; set; }
    }
}
