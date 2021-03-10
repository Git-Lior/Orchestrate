using System.Collections.Generic;

namespace Orchestrate.API.Models
{
    public class SheetMusic
    {
        public int CompositionId { get; set; }
        public Composition Composition { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public byte[] File { get; set; }

        public GroupRole GroupRole { get; set; }
        public ICollection<SheetMusicComment> Comments { get; set; }
    }
}
