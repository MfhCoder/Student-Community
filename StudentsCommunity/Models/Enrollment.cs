using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudentsCommunity.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int CoursesId { get; set; }

        public string StudentId { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("StudentId")]
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("CoursesId")]
        public virtual Courses Courses { get; set; }

    }
}