using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAppDotNetFrameWork.Models
{
    public class Quiz
    {
        public int QuizId { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        // Will be auto-calculated
        public int TimeLimitMinutes { get; set; } 
        
    }
}