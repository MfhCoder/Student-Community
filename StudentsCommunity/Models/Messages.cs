using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentsCommunity.Models
{
    public class Messages
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public int ConversationId { get; set; }
        public DateTime Date { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Conversation Conversation { get; set; }

    }
}