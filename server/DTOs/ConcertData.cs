using System;
using System.Collections.Generic;

namespace Orchestrate.API.DTOs
{
    public class ConcertData
    {
        public int Id { get; set; }

        public string Location { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public IEnumerable<BasicCompositionData> Compositions { get; set; }
    }

    public class ConcertDataWithAttendance : ConcertData
    {
        public IEnumerable<UserData> Attending { get; set; }
        public IEnumerable<UserData> NotAttending { get; set; }
    }
}
