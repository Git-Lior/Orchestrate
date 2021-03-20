using System;
using System.Collections.Generic;

namespace Orchestrate.API.DTOs
{
    public class BasicConcertData
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public long Date { get; set; }
    }

    public class ConcertData : BasicConcertData
    {
        public string Description { get; set; }
        public bool? Attending { get; set; }
        public IEnumerable<CompositionData> Compositions { get; set; }
    }

    public class ConcertDataWithUserAttendance : ConcertData
    {
        public IEnumerable<UserData> AttendingUsers { get; set; }
        public IEnumerable<UserData> NotAttendingUsers { get; set; }
    }
}
