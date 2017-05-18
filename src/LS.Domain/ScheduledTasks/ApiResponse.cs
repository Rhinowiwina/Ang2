using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain.ScheduledTasks
{
    public class ApiResponse
    {
        public string TransactionId { get; set; }
        public List<SubscriberCheck> SubscriberCheck { get; set; }
        public List<AgentCheck> AgentCheck { get; set; }
        public List<DeviceCheck> DeviceCheck { get; set; }
        public List<AgentSubscriberCrossCheck> AgentSubscriberCrossCheck { get; set; }
        public bool Blacklist { get; set; }
        public string Status { get; set; }
        public string Message { get; set;}

    }
    public class SubscriberCheck
    {
        public int PeriodDays { get; set; }
        public int Matches { get; set; }
    }

    public class AgentCheck
    {
        public int PeriodDays { get; set; }
        public bool IsMatched { get; set; }
    }

    public class DeviceCheck
    {
        public int PeriodDays { get; set; }
        public bool IsMatched { get; set; }
    }

    public class AgentSubscriberCrossCheck
    {
        public int PeriodDays { get; set; }
        public int Matches { get; set; }
    }
}
