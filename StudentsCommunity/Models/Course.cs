using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentsCommunity.Models
{
    public class Courses
    {
        public int Id { get; set; }

        //[Required]
        public string CourseName { get; set; }

        public string CourseCode { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        public string DoctorId { get; set; }

        public int GroupNum { get; set; }

        public virtual ICollection<Enrollment> Enrollment { get; set; }
        public virtual ICollection<Posts> Posts { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
        public virtual ICollection<Conversation> Conversation { get; set; }


        [ForeignKey("DoctorId")]
        public virtual ApplicationUser User { get; set; }

    }
}