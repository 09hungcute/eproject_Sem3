using System.ComponentModel.DataAnnotations;

namespace CareerGuidancePlatform.Models
{
    public class Mentor
    {
        public int MentorId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public string Field { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public string Availability { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;
    }
}
