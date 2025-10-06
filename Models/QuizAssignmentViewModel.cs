// Models/QuizAssignmentViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuizAppDotNetFrameWork.Models
{
    public class QuizAssignmentViewModel
    {
        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; set; }

        public List<UserQuizAssignmentItem> UserAssignments { get; set; }

        public List<Quiz> AvailableQuizzes { get; set; }
    }

    public class UserQuizAssignmentItem
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public List<int> SelectedQuizIds { get; set; }
        public bool IsSelected { get; set; }

        // Helper properties for UI
        public string DisplayName => $"{Username} ({Role})";
    }
}