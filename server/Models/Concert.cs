using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    public class ConcertPayload
    {
        [Required, StringLength(50, MinimumLength = 1)]
        public string Location { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Description { get; set; }
    }

    public class Concert : ConcertPayload
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public ICollection<ConcertAttendance> Attendances { get; set; }

        public ICollection<Composition> Compositions { get; set; }
    }
}
