using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAppDotNetFrameWork.Models
{
    public class Option
    {
        public int OptionId { get; set; }
        public int QuestionId { get; set; }
        public string OptionText { get; set; }
        public bool isCorrect { get; set; }
    }
}