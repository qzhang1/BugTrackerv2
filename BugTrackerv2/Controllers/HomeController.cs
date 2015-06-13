using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTrackerv2.Models;
using BugTrackerv2.Models.CustomViewModels;
using Microsoft.AspNet.Identity;
namespace BugTrackerv2.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Landing");
            }
        }

        //GET -> populate sidebars with number of tickets, notifications, and projects. also short lists of recent tickets, projects, and notifications to review
        public ActionResult PopulateSideBar()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.NotificationCount = db.TicketNotifications.Where(n => n.UserId == userId).Count();            
            return PartialView("_SideBarNav");
        }


        public ActionResult PopulateMainBar()
        {
            var userId = User.Identity.GetUserId();
            var projects = db.Users.FirstOrDefault(u => u.Id == userId).Projects.ToList();
            var notifications = db.TicketNotifications.Where(n => n.UserId == userId).ToList();
            var tickets = db.Tickets.Where(t => (t.OwnerUserId == userId) || (t.AssignedToUserId == userId)).ToList();

            var model = new NavBarViewModel
            {
                NotificationCount = notifications.Count,
                Notifications = notifications.OrderBy(o => o.Created).Take(4),
                ProjectCount = projects.Count,
                Projects = projects.OrderBy(o => o.Created).Take(4),
                TicketCount = tickets.Count,
                Tickets = tickets.OrderBy(o => o.Created).Take(4)
            };

            return PartialView("_MainBar", model);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Landing()
        {
            return View();
        }
    }
}