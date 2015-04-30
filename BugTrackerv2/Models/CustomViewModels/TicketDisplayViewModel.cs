using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerv2.Models.CustomViewModels
{
    public class TicketDisplayViewModel
    {
        public string Title { get; set; }
        public string ProjectName { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string Owner { get; set; }
        public string Assigned { get; set; }
    }
}