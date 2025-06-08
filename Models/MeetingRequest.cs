using System;

namespace CareerGuidancePlatform.Models
{
    public class MeetingRequest
    {
        public int MeetingRequestId { get; set; }

        public int MenteeId { get; set; }

        public int MentorId { get; set; }

        public DateTime RequestedTime { get; set; }

        public string Status { get; set; } = "Pending";

        public string? Notes { get; set; }
    }
}
