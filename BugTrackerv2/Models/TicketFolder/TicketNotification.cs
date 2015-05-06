using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerv2.Models.TicketFolder
{
    public class TicketNotification
    {
        public int TicketNotificationId { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public string message { get; set; }
        public DateTime Created { get; set; }
        public Ticket Ticket { get; set; }
        public ApplicationUser User { get; set; }
    }
}