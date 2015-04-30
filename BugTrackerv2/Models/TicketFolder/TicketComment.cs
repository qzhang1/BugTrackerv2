using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerv2.Models.TicketFolder
{
    public class TicketComment
    {
        public int TicketCommentId { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }
        
        
        public string FileUrl { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }


        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}