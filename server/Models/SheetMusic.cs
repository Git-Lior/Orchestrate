using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("sheet_music")]
    public class SheetMusic
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int CompositionId { get; set; }
        public Composition Composition { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public byte[] File { get; set; }
        public ICollection<SheetMusicComment> Comments { get; set; }
    }
}
