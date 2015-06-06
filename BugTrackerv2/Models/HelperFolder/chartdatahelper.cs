using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTrackerv2.Models;
using BugTrackerv2.Models.TicketFolder;
namespace BugTrackerv2.Models.HelperFolder
{
    public class chartdatahelper
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public IList<int> RoleCount()
        {
            List<int> a = new List<int>();
            var roles = db.Roles.ToList();
            foreach(var role in roles)
            {
                a.Add(role.Users.Count);
            }
            var no_roles = db.Users.Where(u => u.Roles.All(r => r.UserId != u.Id)).ToList();
            a.Add(no_roles.Count);
            return a;
        }

        public IList<Ticket> PopularTickets()
        {
            var pt = db.Tickets.OrderByDescending(c => c.Created).Take(3).ToList();
            return pt;
        }
    }
}