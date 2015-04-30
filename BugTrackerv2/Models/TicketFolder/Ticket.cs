using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using BugTrackerv2.Models.ProjectFolder;
namespace BugTrackerv2.Models.TicketFolder
{
    public class Ticket
    {

        public Ticket()
        {
            this.TicketAttachments = new HashSet<TicketAttachment>();
            this.TicketComments = new HashSet<TicketComment>();
        }

        //data properties
        public int TicketId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        [Display(Name="Ticket Owner")]
        public string OwnerUserId { get; set; }
        [Display(Name="Assigned To")]
        public string AssignedToUserId { get; set; }


        //FKs
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        


        //nav propertiers
        [Display(Name="Project")]
        public virtual Project project { get; set; }                  //1 project => many tickets
        public virtual TicketType TicketType { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public virtual ApplicationUser AssignedTo { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }

    }
}