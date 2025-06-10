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

        // ✅ GET /api/quiz/questions
        [HttpGet("questions")]
        public IActionResult GetQuestions()
        {
            var questions = new List<QuizQuestion>
            {
                new() { Id = 1, QuestionText = "Tôi thích làm việc với máy tính và phần mềm.", Options = new() { "Không hề thích", "Thi thoảng thôi", "Thường xuyên", "Rất yêu thích" } },
                new() { Id = 2, QuestionText = "Tôi cảm thấy hứng thú với việc thiết kế hình ảnh, poster hoặc video.", Options = new() { "Không hứng thú", "Có một chút", "Khá thích", "Cực kỳ thích" } },
                new() { Id = 3, QuestionText = "Tôi thích phân tích dữ liệu và tìm ra xu hướng.", Options = new() { "Không quan tâm", "Quan tâm nhẹ", "Quan tâm nhiều", "Rất đam mê" } },
                new() { Id = 4, QuestionText = "Tôi có kỹ năng lập trình hoặc hiểu biết về công nghệ.", Options = new() { "Không có", "Cơ bản", "Khá tốt", "Rất thành thạo" } },
                new() { Id = 5, QuestionText = "Tôi giỏi giao tiếp và thuyết trình trước đám đông.", Options = new() { "Rất kém", "Khá e ngại", "Tự tin vừa phải", "Rất tự tin" } },
                new() { Id = 6, QuestionText = "Tôi giỏi tư duy logic và suy luận phân tích.", Options = new() { "Không giỏi", "Trung bình", "Khá tốt", "Rất giỏi" } },
                new() { Id = 7, QuestionText = "Tôi muốn làm công việc giúp đỡ người khác.", Options = new() { "Không muốn", "Cũng được", "Thích", "Rất thích" } },
                new() { Id = 8, QuestionText = "Tôi quan tâm đến sự sáng tạo và tự do trong công việc.", Options = new() { "Không cần", "Một chút", "Có", "Rất quan trọng" } },
                new() { Id = 9, QuestionText = "Tôi coi trọng môi trường làm việc năng động, cạnh tranh.", Options = new() { "Không quan trọng", "Quan trọng nhẹ", "Quan trọng", "Rất quan trọng" } },
                new() { Id = 10, QuestionText = "Tôi là người hướng ngoại và thích kết nối với người khác.", Options = new() { "Không", "Thỉnh thoảng", "Khá thường", "Rất thường" } },
                new() { Id = 11, QuestionText = "Tôi là người chi tiết, tỉ mỉ trong công việc.", Options = new() { "Không tỉ mỉ", "Bình thường", "Khá tỉ mỉ", "Rất tỉ mỉ" } },
                new() { Id = 12, QuestionText = "Tôi thích làm việc nhóm hơn là làm việc một mình.", Options = new() { "Thích làm một mình", "Có thể làm nhóm", "Ưu tiên làm nhóm", "Rất thích teamwork" } }
            };

            return Ok(questions);
        }

        // ✅ POST /api/quiz/submit-letter
        [HttpPost("submit-letter")]
        public IActionResult SubmitQuizWithLetters([FromBody] QuizSubmission submission)
        {
            if (submission.Answers == null || submission.Answers.Count != 12)
                return BadRequest("Bạn phải trả lời đủ 12 câu hỏi.");

            // Convert A/B/C/D -> 1/2/3/4
            var numericAnswers = submission.Answers.Select(letter =>
                letter.Trim().ToUpper() switch
                {
                    "A" => 1,
                    "B" => 2,
                    "C" => 3,
                    "D" => 4,
                    _ => 0
                }).ToArray();

            if (numericAnswers.Any(x => x == 0))
                return BadRequest("Tồn tại đáp án không hợp lệ (chỉ chấp nhận A–D).");

            return ProcessScore(numericAnswers);
        }

        // ✅ POST /api/quiz/submit (cũ, giữ nguyên)
        [HttpPost("submit")]
        public IActionResult SubmitQuiz([FromBody] QuizResponse response)
        {
            if (response.Answers == null || response.Answers.Length != 12)
                return BadRequest("Bạn phải trả lời đủ 12 câu hỏi.");

            return ProcessScore(response.Answers);
        }

        // 🔁 Tính điểm & trả kết quả chung
        private IActionResult ProcessScore(int[] answers)
        {
            int interest = answers[0] + answers[1] + answers[2];
            int skill = answers[3] + answers[4] + answers[5];
            int value = answers[6] + answers[7] + answers[8];
            int personality = answers[9] + answers[10] + answers[11];

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
