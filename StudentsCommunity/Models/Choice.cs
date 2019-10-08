using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentsCommunity.Models
{
    public class Choice
    {
        public int ChoiceId { get; set; }
        public string ChoiceText { get; set; }
        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }
    }
}