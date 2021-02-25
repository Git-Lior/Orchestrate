using System.Collections.Generic;

namespace Orchestrate.API.DTOs
{
    public class CompositionData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Composer { get; set; }
        public string Genre { get; set; }
        public UserData Uploader { get; set; }
    }

    public class FullCompositionData : CompositionData
    {
        public IEnumerable<SheetMusicData> SheetMusics { get; set; }
    }
}
