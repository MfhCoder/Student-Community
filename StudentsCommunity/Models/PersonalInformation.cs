using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudentsCommunity.Models
{
    public class PersonalInformation
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }

        public string FullName { get; set; }
        public int StudentId { get; set; }
        public string UserImage { get; set; }
        public string Skills { get; set; }

        public virtual ApplicationUser User { get; set; }

    }
}