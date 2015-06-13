using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace BugTrackerv2.Models.TicketFolder
{
    public class TicketNotification
    {
        public int TicketNotificationId { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        [AllowHtml]
        public string message { get; set; }
        public DateTime Created { get; set; }
        public bool read { get; set; }


        public Ticket Ticket { get; set; }
        public ApplicationUser User { get; set; }
    }
}