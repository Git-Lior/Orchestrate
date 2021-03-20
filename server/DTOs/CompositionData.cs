using Orchestrate.API.Models;
using System.Collections.Generic;

namespace Orchestrate.API.DTOs
{
    public class BasicCompositionData
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class CompositionData : BasicCompositionData
    {
        public string Composer { get; set; }
        public string Genre { get; set; }
        public UserData Uploader { get; set; }
    }

    public class FullCompositionData : CompositionData
    {
        public IEnumerable<Role> Roles { get; set; }
    }
}
