namespace Orchestrate.API.DTOs
{
    public class GroupData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserData Manager { get; set; }
    }
}
