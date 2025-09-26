using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAppDotNetFrameWork.Models
{
    public class Quiz
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public int createdBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}