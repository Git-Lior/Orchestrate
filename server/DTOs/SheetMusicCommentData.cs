using System;

namespace Orchestrate.API.DTOs
{
    public class SheetMusicCommentData
    {
        public int Id { get; set; }
        public UserData User { get; set; }
        public long CreatedAt { get; set; }
        public long? UpdatedAt { get; set; }
        public string Content { get; set; }
    }
}
