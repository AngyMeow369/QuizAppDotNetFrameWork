using System;

namespace QuizAppDotNetFrameWork.Models
{
    public class QuizAttempt
    {
        public int AttemptId { get; set; }
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime CompletedOn { get; set; }

        // Add these properties for results:
        public string Grade { get; set; }  // Store the grade (A, B, C, etc.)

        // Navigation properties
        public string Username { get; set; }
        public string QuizTitle { get; set; }

        // Keep this calculated property - it's perfect!
        public double Percentage => TotalQuestions > 0 ? (double)Score / TotalQuestions * 100 : 0;
    }
}