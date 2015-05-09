using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerv2.Models.HelperFolder
{
    public class notificationHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public int CountNotifications(string Id)
        {
            return (db.TicketNotifications.Where(tn => tn.UserId == Id).Count());
        }
    }
}