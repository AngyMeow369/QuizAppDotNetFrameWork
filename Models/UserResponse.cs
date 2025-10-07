using System;

namespace QuizAppDotNetFrameWork.Models
{
    public class UserResponse
    {
        public int ResponseId { get; set; }
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public int SelectedOptionId { get; set; }
        public DateTime SubmittedOn { get; set; }

        // Add this:
        public bool IsCorrect { get; set; }  // Track if answer was correct
        public int AttemptId { get; set; }   // Link to which attempt
    }
}