using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace BugTrackerv2.Models.TicketFolder
{
    public class TicketPriority
    {
        public TicketPriority()
        {
            this.Tickets = new HashSet<Ticket>();
        }
        public int TicketPriorityId { get; set; }
        [Display(Name="Priority")]
        public string Name { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }

    }
}