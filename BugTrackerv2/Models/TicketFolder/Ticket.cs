using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTrackerv2.Models.ProjectFolder;
namespace BugTrackerv2.Models.TicketFolder
{
    public class Ticket
    {
        //data properties
        public int TicketId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string OwnerUserId { get; set; }
        public string AssignedToUserId { get; set; }


        //FKs
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        


        //nav propertiers
        public Project project { get; set; }                    //1 project => many tickets
    }
}