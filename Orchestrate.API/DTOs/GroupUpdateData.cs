namespace Orchestrate.API.DTOs
{
    public class GroupUpdateData
    {
        public long Date { get; set; }
    }

    public class CompositionUpdateData : GroupUpdateData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public UserData Uploader { get; set; }
    }

    public class ConcertUpdateData : GroupUpdateData
    {
        public int Attendance { get; set; }
        public BasicConcertData Concert { get; set; }
    }
}
