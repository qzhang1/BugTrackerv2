using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTrackerv2.Models.TicketFolder;
using BugTrackerv2.Models.ProjectFolder;

namespace BugTrackerv2.Models.CustomViewModels
{
    public class NavBarViewModel
    {
        //label counters
        public int NotificationCount { get; set; }
        public int TicketCount { get; set; }
        public int ProjectCount { get; set; }

        //dropdown overview lists
        public IEnumerable<TicketNotification> Notifications { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }
        public IEnumerable<Project> Projects { get; set; }

    }
}