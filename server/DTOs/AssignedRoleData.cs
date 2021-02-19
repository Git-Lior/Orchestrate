using System.Collections.Generic;

namespace Orchestrate.API.DTOs
{
    public class AssignedRoleData
    {
        public int Id { get; set; }
        public string Section { get; set; }
        public int? Num { get; set; }
        public IEnumerable<UserData> Members { get; set; }
    }
}
