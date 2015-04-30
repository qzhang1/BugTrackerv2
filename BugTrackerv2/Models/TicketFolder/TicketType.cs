using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace BugTrackerv2.Models.TicketFolder
{
    public class TicketType
    {
        public TicketType()
        {
            this.Tickets = new HashSet<Ticket>();
        }
        public int TicketTypeId { get; set; }
        [Display(Name="Type")]
        public string Name { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}