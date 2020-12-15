using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    [Table("sheet_music")]
    public class SheetMusic
    {
        public int GroupID { get; set; }
        public Group Group { get; set; }

        public int CompositionID { get; set; }
        public Composition Composition { get; set; }

        public int RoleID { get; set; }
        public Role Role { get; set; }

        public byte[] File { get; set; }
        public ICollection<SheetMusicComment> Comments { get; set; }
    }
}
