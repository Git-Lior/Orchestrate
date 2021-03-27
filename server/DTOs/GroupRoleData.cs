using System.Collections.Generic;

namespace Orchestrate.API.DTOs
{
    public class RoleData
    {
        public int Id { get; set; }
        public string Section { get; set; }
        public int? Num { get; set; }
    }

    public class GroupRoleData : RoleData
    {
        public IEnumerable<UserData> Members { get; set; }
    }

    public class GroupRoleAttendanceData : RoleData
    {
        public IEnumerable<UserDataWithAttendance> Attendances { get; set; }
    }
}
