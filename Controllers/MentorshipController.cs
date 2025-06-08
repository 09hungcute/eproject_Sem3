using Microsoft.AspNetCore.Mvc;
using CareerGuidancePlatform.Models;
using CareerGuidancePlatform.Data;
using Microsoft.EntityFrameworkCore;

namespace CareerGuidancePlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MentorshipController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MentorshipController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("add-mentor")]
        public async Task<IActionResult> AddMentor([FromBody] Mentor mentor)
        {
            _context.Mentors.Add(mentor);
            await _context.SaveChangesAsync();
            return Ok("Mentor added");
        }
        [HttpGet("mentors")]
        public async Task<IActionResult> GetMentors([FromQuery] string? field, [FromQuery] string? specialization, [FromQuery] string? availability)
        {
            var query = _context.Mentors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(field))
                query = query.Where(m => m.Field.Contains(field));
            if (!string.IsNullOrWhiteSpace(specialization))
                query = query.Where(m => m.Specialization.Contains(specialization));
            if (!string.IsNullOrWhiteSpace(availability))
                query = query.Where(m => m.Availability.Contains(availability));

            return Ok(await query.ToListAsync());
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            message.Timestamp = DateTime.UtcNow;
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return Ok("Message sent");
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages(int userId, int otherUserId)
        {
            var messages = await _context.Messages
                .Where(m =>
                    (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost("request-meeting")]
        public async Task<IActionResult> RequestMeeting([FromBody] MeetingRequest request)
        {
            request.Status = "Pending";
            _context.MeetingRequests.Add(request);
            await _context.SaveChangesAsync();
            return Ok("Meeting request submitted.");
        }

        [HttpPost("update-meeting-status")]
        public async Task<IActionResult> UpdateMeetingStatus(int requestId, string status)
        {
            var request = await _context.MeetingRequests.FindAsync(requestId);
            if (request == null)
                return NotFound("Meeting request not found.");

            request.Status = status;
            await _context.SaveChangesAsync();
            return Ok($"Meeting request updated to {status}");
        }

        [HttpGet("group-sessions")]
        public async Task<IActionResult> GetGroupSessions()
        {
            return Ok(await _context.GroupSessions.ToListAsync());
        }

        [HttpPost("create-session")]
        public async Task<IActionResult> CreateGroupSession([FromBody] GroupSession session)
        {
            session.GroupSessionId = 0; // Auto-increment
            _context.GroupSessions.Add(session);
            await _context.SaveChangesAsync();
            return Ok("Group session created");
        }

        [HttpPost("join-session")]
        public async Task<IActionResult> JoinGroupSession(int sessionId, int userId)
        {
            var session = await _context.GroupSessions.FindAsync(sessionId);
            if (session == null)
                return NotFound("Session not found.");

            if (!session.ParticipantIds.Contains(userId))
                session.ParticipantIds.Add(userId);

            await _context.SaveChangesAsync();
            return Ok("Joined group session");
        }
    }
}
