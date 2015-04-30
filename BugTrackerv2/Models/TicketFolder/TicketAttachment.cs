using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerv2.Models.TicketFolder
{
    public class TicketAttachment
    {
        //one-to-one?
        public int TicketAttachmentId { get; set; }
        public int TicketId { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public int UserId { get; set; }
        public string FileUrl { get; set; }

        //attachment belong to one user and one ticket. 
        //one ticket can have many attachments
        //one user can have many attachments
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}