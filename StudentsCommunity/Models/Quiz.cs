using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentsCommunity.Models
{
    public class Quiz
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public int CourseId { get; set; }
        public virtual Courses Course { get; set; }
        public virtual ICollection<Question> Questions { get; set; }

    }
}