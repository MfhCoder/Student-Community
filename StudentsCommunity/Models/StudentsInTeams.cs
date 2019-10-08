using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentsCommunity.Models
{
    public class StudentsInTeams
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string StudentId { get; set; }

        public virtual Team Team { get; set; }
        public virtual ApplicationUser Student { get; set; }

    }
}