using System.Collections.Generic;

namespace Orchestrate.API.DTOs
{
    public class BasicGroupRoleData
    {
        public int Id { get; set; }
        public string Section { get; set; }
        public int? Num { get; set; }
    }

    public class GroupRoleData : BasicGroupRoleData
    {   
        public IEnumerable<UserData> Members { get; set; }
    }

    public class GroupRoleAttendanceData : BasicGroupRoleData
    {
        public IEnumerable<UserDataWithAttendance> Attendances { get; set; }
    }
}
