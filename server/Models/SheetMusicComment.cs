using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Models
{
    [Table("sheet_music_comment")]
    public class SheetMusicComment
    {
        public int CommentId { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int CompositionId { get; set; }
        public Composition Composition { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Content { get; set; }
    }
}
