using System.Collections.Generic;

namespace Orchestrate.API.DTOs
{
    public class GroupData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserData Manager { get; set; }
    }

    public class FullGroupData : GroupData
    {
        public IEnumerable<UserData> Directors { get; set; }
        public IEnumerable<GroupRoleData> Roles { get; set; }
    }
}
