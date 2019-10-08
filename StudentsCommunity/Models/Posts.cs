using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentsCommunity.Models
{
    public class Posts
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string Message { get; set; }
        public DateTime PostedDate { get; set; }
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public bool IsSolved { get; set; } = false;
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public virtual Courses Course { get; set; }
        public virtual ICollection<Comments> Comment { get; set; }
        public virtual ICollection<Likes> Likes { get; set; }


    }
}