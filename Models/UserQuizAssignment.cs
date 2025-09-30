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

        // NEW: Phase 1 properties
        public DateTime DueDate { get; set; }        // When it's due
        public int? AttemptId { get; set; }          // Link to attempt if completed

        // Navigation properties (for easier data binding)
        public string Username { get; set; }         // Will be populated from joins
        public string QuizTitle { get; set; }        // Will be populated from joins

        // NEW: Calculated properties for display
        public int QuestionCount { get; set; }       // Auto-populated from quiz
        public int TimeLimitMinutes { get; set; }    // Auto-calculated: QuestionCount * 1

        // NEW: Status properties for UI
        public bool IsOverdue => DueDate < DateTime.Now && !IsCompleted;
        public string Status
        {
            get
            {
                if (IsCompleted) return "Completed";
                if (IsOverdue) return "Overdue";
                return "Pending";
            }
        }
    }
}