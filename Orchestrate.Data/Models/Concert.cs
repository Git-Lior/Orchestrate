﻿using Orchestrate.Data.Models.Joins;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.Data.Models
{
    public record ConcertIdentifier(int GroupId, int ConcertId);

    public class ConcertPayload
    {
        [Required, StringLength(50, MinimumLength = 1)]
        public string Location { get; set; }

        [Required]
        public DateTimeOffset Date { get; set; }

        [StringLength(100)]
        public string Description { get; set; }
    }

    public class ConcertFields : ConcertPayload
    {
        public int GroupId { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class Concert : ConcertFields
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Group Group { get; set; }

        public ICollection<ConcertAttendance> Attendances { get; set; }

        public ICollection<Composition> Compositions { get; set; }
    }
}
