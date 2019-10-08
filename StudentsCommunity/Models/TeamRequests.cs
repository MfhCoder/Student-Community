using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentsCommunity.Models
{
    public class TeamRequests
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string FromId { get; set; }
        public string ToId { get; set; }
        public int TeamId { get; set; }
        public bool Accept { get; set; }

    }
}