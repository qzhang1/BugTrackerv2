using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTrackerv2.Models;
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