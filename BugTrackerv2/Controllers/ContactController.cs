using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using SendGrid;
using System.Configuration;
using Microsoft.AspNet.Identity;
using BugTrackerv2.Models.CustomViewModels;

namespace BugTrackerv2.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }
       
    }
}