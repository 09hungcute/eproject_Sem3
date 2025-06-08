namespace CareerGuidancePlatform.Models
{
    public class QuizResponse
    {
        // Mảng 12 câu trả lời (1–5 điểm mỗi câu)
        public int[] Answers { get; set; } = new int[12];
    }
}
