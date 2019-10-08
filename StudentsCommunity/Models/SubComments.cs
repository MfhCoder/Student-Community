using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudentsCommunity.Models
{
    public class SubComments
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public string CommentMsg { get; set; }
        public DateTime CommentedDate { get; set; }
        public string UserId { get; set; }

        public virtual Comments Comment { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}