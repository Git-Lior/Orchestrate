using System;

namespace Orchestrate.API.DTOs
{
    public class SheetMusicCommentData
    {
        public int Id { get; set; }
        public UserData User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Content { get; set; }
    }
}
