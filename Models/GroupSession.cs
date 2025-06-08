using System;
using System.Collections.Generic;

namespace CareerGuidancePlatform.Models
{
    public class GroupSession
    {
        public int GroupSessionId { get; set; }

        public string Topic { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime ScheduledTime { get; set; }

        public List<int> ParticipantIds { get; set; } = new();
    }
}
