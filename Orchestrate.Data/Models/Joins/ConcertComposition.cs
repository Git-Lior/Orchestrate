namespace Orchestrate.Data.Models.Joins
{
    public class ConcertComposition
    {
        public int GroupId { get; set; }
        
        public int ConcertId { get; set; }
        public Concert Concert { get; set; }
        
        public int CompositionId { get; set; }
        public Composition Composition { get; set; }
    }
}
