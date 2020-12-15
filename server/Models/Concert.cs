﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("concert")]
    public class Concert
    {
        public int GroupID { get; set; }
        public Group Group { get; set; }

        public int ConcertID { get; set; }

        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }

        public ICollection<ConcertComposition> Compositions { get; set; }
        public ICollection<ConcertAttendance> Attendance { get; set; }
    }
}
