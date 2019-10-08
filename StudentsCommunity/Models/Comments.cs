using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudentsCommunity.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public string CommentMsg { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public DateTime CommentedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public virtual Posts Post { get; set; }
        public virtual ICollection<SubComments> SubComment { get; set; }

    }
}