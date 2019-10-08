using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentsCommunity.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }
        public string StudentId { get; set; }

        public virtual Courses Course { get; set; }
        public virtual ApplicationUser Student { get; set; }
        public virtual ICollection<StudentsInTeams> StudentsInTeams { get; set; }


    }
}