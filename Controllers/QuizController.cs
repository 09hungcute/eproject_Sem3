using Microsoft.AspNetCore.Mvc;
using CareerGuidancePlatform.Models;

namespace CareerGuidancePlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly Dictionary<string, List<string>> _careerJobRoles = new()
        {
            ["Software Developer"] = new List<string> {
                "Web Developer",
                "Mobile App Developer",
                "AI/ML Engineer",
                "Game Developer"
            },
            ["Graphic Designer"] = new List<string> {
                "UI/UX Designer",
                "Brand Identity Designer",
                "Illustrator",
                "Motion Graphics Designer"
            },
            ["Marketing Specialist"] = new List<string> {
                "Digital Marketer",
                "SEO Specialist",
                "Content Strategist",
                "Social Media Manager"
            },
            ["Data Analyst"] = new List<string> {
                "Business Intelligence Analyst",
                "Data Scientist",
                "Product Analyst",
                "Market Research Analyst"
            },
            ["Human Resources"] = new List<string> {
                "Recruiter",
                "HR Generalist",
                "Training & Development Specialist",
                "Employee Relations Manager"
            }
        };

        [HttpPost("submit")]
        public IActionResult SubmitQuiz([FromBody] QuizResponse response)
        {
            if (response.Answers == null || response.Answers.Length != 12)
                return BadRequest("Bạn phải trả lời đủ 12 câu hỏi.");

            // Tính điểm theo nhóm
            int interest = response.Answers[0] + response.Answers[1] + response.Answers[2];
            int skill = response.Answers[3] + response.Answers[4] + response.Answers[5];
            int value = response.Answers[6] + response.Answers[7] + response.Answers[8];
            int personality = response.Answers[9] + response.Answers[10] + response.Answers[11];

            // Tính điểm tương thích với từng nghề
            var careerScores = new Dictionary<string, int>
            {
                ["Software Developer"] = interest + skill * 2 + value + personality,
                ["Graphic Designer"] = interest * 2 + skill + value * 2 + personality,
                ["Marketing Specialist"] = interest + skill + value * 2 + personality * 2,
                ["Data Analyst"] = interest * 2 + skill * 2 + personality,
                ["Human Resources"] = interest + value * 3 + personality * 2
            };

            var sortedCareerMatches = careerScores
                .OrderByDescending(c => c.Value)
                .Select(c => new { career = c.Key, score = c.Value })
                .ToList();

            var bestMatch = sortedCareerMatches.First();

            return Ok(new
            {
                bestCareerMatch = bestMatch.career,
                compatibilityScore = bestMatch.score,
                jobRoles = _careerJobRoles[bestMatch.career],
                allCareerMatches = sortedCareerMatches
            });
        }
    }
}
