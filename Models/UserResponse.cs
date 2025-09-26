using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAppDotNetFrameWork.Models
{
    public class UserResponse
    {
        public int ResponseId { get; set; }
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public int SelectedOptionId { get; set; }
        public DateTime submittedOn { get; set; }

    }
}