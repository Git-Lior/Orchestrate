﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("concert")]
    public class Concert
    {
        public int Id { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        [Required, StringLength(50, MinimumLength = 1)]
        public string Location { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        public string Description { get; set; }

        public ICollection<ConcertAttendance> Attendances { get; set; }
        public ICollection<ConcertComposition> ConcertCompositions { get; set; }
    }
}
