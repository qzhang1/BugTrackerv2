using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerv2.Models.TicketFolder
{
    public class TicketHistory
    {
        public int TicketHistoryId {get; set;}
        public int TicketId {get; set;}
        public string UserId { get; set; }
        public string Property { get; set; }
        public DateTimeOffset Change { get; set; }
        //old and new values can be converted to int if needed
        public string OldValue { get; set; }        
        public string NewValue { get; set; }      
        public virtual Ticket Ticket {get; set;}
        public virtual ApplicationUser User { get; set; }

    }
}