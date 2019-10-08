using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentsCommunity.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int CourseId { get; set; }
        public string FromId { get; set; }
        public string ToId { get; set; }
        public int TeamId { get; set; }
        public bool IsRead { get; set; }

        public virtual ApplicationUser From { get; set; }
        public virtual Courses Course { get; set; }
        public virtual ICollection<Messages> Messages { get; set; }

    }
}