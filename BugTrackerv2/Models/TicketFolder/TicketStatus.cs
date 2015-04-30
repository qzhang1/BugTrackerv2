using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace BugTrackerv2.Models.TicketFolder
{
    public class TicketStatus
    {
        public TicketStatus()
        {
            this.Tickets = new HashSet<Ticket>();
        }
        public int TicketStatusId { get; set; }
        [Display(Name="Status")]
        public string Name { get; set; }
        
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}