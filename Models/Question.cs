using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAppDotNetFrameWork.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }
        public string QuestionText { get; set; }

        // Add this for options
        public List<Option> Options { get; set; }
    }
}