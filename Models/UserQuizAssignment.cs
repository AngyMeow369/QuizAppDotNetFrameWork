using System;

namespace QuizAppDotNetFrameWork.Models
{
    // Track which quizzes are assigned to which users
    public class UserQuizAssignment
    {
        public int AssignmentId { get; set; }
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public DateTime AssignedOn { get; set; }
        public bool IsCompleted { get; set; }

        // Navigation properties (optional - for easier data binding)
        public string Username { get; set; }  // Will be populated from joins
        public string QuizTitle { get; set; } // Will be populated from joins
    }
}
