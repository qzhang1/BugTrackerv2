using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTrackerv2.Models;
using BugTrackerv2.Models.TicketFolder;
using BugTrackerv2.Models.ProjectFolder;
using Microsoft.AspNet.Identity;
using System.Security.Principal;
using System.IO;
using System.Text;
using System.ComponentModel.DataAnnotations;
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

        public int TicketCount(string Id)
        {
            return db.Tickets.Where(u => u.OwnerUserId == Id || u.AssignedToUserId == Id).Count();
        }

        public IList<Ticket> PopularTickets(string Id)
        {
            var pt = db.Tickets.Where(u => u.AssignedToUserId == Id || u.OwnerUserId == Id).OrderByDescending(c => c.Created).Take(3).ToList();
            return pt;
        }

        public IList<TicketNotification> PopularNotifications(string Id)
        {

            var pn = db.TicketNotifications.Where(r => r.read == false && r.UserId == Id).OrderByDescending(c => c.Created).Take(5).ToList();
            foreach(var p in pn)
            {
                p.Ticket = db.Tickets.Find(p.TicketId);
            }
            return pn;
        }

        public IList<Project> PopularProjects(string Id)
        {
            //var pp = db.Projects.Where(u => u.).OrderByDescending(c => c.Created).Take(5).ToList();
            var a = db.Users.Include(p => p.Projects).FirstOrDefault(u => u.Id == Id);
            var pp = a.Projects.ToList();
            return pp;
        }

        //public int ProjectCount(string Id)
        //{
        //    return db.Users.Include(u => u.Projects).FirstOrDefault(a => a.Id == Id).Projects.Count;
        //}
    }
}