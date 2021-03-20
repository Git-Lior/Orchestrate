using System;

namespace Orchestrate.API.DTOs
{
    public class UpdateData
    {
        public long Date { get; set; }
    }

    public class CompositionUpdateData : UpdateData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public UserData Uploader { get; set; }
    }

    public class ConcertUpdateData : UpdateData
    {
        public int Attendance { get; set; }
        public BasicConcertData Concert { get; set; }
    }
}
