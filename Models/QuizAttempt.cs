using System;

namespace QuizAppDotNetFrameWork.Models
{
    // Track quiz attempts with scores
    public class QuizAttempt
    {
        public int AttemptId { get; set; }
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime CompletedOn { get; set; }

        // Navigation properties (optional - for easier data binding)
        public string Username { get; set; }   // Will be populated from joins
        public string QuizTitle { get; set; }  // Will be populated from joins
        public double Percentage => TotalQuestions > 0 ? (double)Score / TotalQuestions * 100 : 0;
    }
}