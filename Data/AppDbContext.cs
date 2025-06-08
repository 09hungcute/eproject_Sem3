    using Microsoft.EntityFrameworkCore;
    using CareerGuidancePlatform.Models;

    namespace CareerGuidancePlatform.Data
    {
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            public DbSet<User> Users { get; set; }

            // ✅ Thêm các bảng cho Mentorship Program
            public DbSet<Mentor> Mentors { get; set; }
            public DbSet<Message> Messages { get; set; }
            public DbSet<MeetingRequest> MeetingRequests { get; set; }
            public DbSet<GroupSession> GroupSessions { get; set; }
        }
    }
