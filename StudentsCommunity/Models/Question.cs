using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentsCommunity.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int QuizId { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<Choice> Choices { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}