using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("sheet_music_comment")]
    public class SheetMusicComment
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Required, StringLength(300, MinimumLength = 1)]
        public string Content { get; set; }
    }
}
