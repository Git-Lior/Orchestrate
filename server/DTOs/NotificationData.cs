using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.DTOs
{
    public class NotificationData
    {
        public long Date { get; set; }
    }

    public class SheetMusicNotificationData : NotificationData
    {
        public int GroupId { get; set; }
        public BasicCompositionData Composition { get; set; }
        public BasicGroupRoleData Role { get; set; }
        public int Comments { get; set; }
    }

    public class ConcertNotificationData : NotificationData
    {
        public int GroupId { get; set; }
        public BasicConcertData Concert { get; set; }
    }
}
