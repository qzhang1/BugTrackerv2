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
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
namespace BugTrackerv2.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Administrator"))
                return View(db.Tickets.ToList());
            else if (User.IsInRole("Submitter"))
                return View(db.Tickets.Where(t => t.OwnerUserId == userId).ToList());
            else
                return View((db.Tickets.Where(t => t.project.Users.Any(u => u.Id == userId))).ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ticket.TicketComments = ticket.TicketComments.OrderByDescending(o => o.Created).ToList();
            return View(ticket);
        }

        [Authorize()]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(TicketComment Body)
        {
            if(ModelState.IsValid)
            {
                if(User.IsInRole("Submitter"))
                {
                    var userId = User.Identity.GetUserId();
                    var ticketProject = db.Tickets.FirstOrDefault(t => t.TicketId == Body.TicketId).OwnerUserId;
                    //submitters
                    if(ticketProject == (userId))
                    {
                        //if submitter owns this ticket then add comment
                        Body.Created = System.DateTime.Now;
                        Body.UserId = User.Identity.GetUserId();
                        db.TicketComments.Add(Body);
                        db.SaveChanges();
                        return RedirectToAction("Details", new { Id = Body.TicketId });
                    }
                }
                else if(User.IsInRole("Project Manager") || User.IsInRole("Developer"))
                {
                    //PM or dev
                    var userId = (User.Identity.GetUserId());
                    var userInProject = db.Users.FirstOrDefault(u => u.Id == userId ).Projects.ToList();
                    var ticketProject = db.Tickets.FirstOrDefault(t => t.TicketId == Body.TicketId).ProjectId;
                    foreach(var project in userInProject)
                    {
                        if (project.ProjectId == ticketProject)
                        {
                            Body.Created = System.DateTime.Now;
                            Body.UserId = User.Identity.GetUserId();
                            db.TicketComments.Add(Body);
                            db.SaveChanges();
                            return RedirectToAction("Details", new { Id = Body.TicketId });
                        }
                    }
                }
                else
                {
                    Body.Created = System.DateTime.Now;
                    Body.UserId = User.Identity.GetUserId();
                    db.TicketComments.Add(Body);
                    db.SaveChanges();
                    return RedirectToAction("Details", new { Id = Body.TicketId });
                }
                
            }
            return RedirectToAction("Index");
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            //owner is the one who is creating the ticket atm
            var userId = User.Identity.GetUserId();
            var Devs = db.Users.Where(u => u.Roles.Any(r => r.RoleId == (db.Roles.FirstOrDefault(ru => ru.Name == "Developer")).Id));
            
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "TicketPriorityId", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "TicketTypeId", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ticket ticket, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                ticket.Created = System.DateTime.Now;
                var OwnerId = User.Identity.GetUserId();
                ticket.OwnerUserId = OwnerId;

                ticket.TicketStatusId = db.TicketStatuses.FirstOrDefault(s => s.Name == "New").TicketStatusId;
                ticket.project = db.Projects.FirstOrDefault(p => p.ProjectId == ticket.ProjectId);

                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "TicketPriorityId", "Name", ticket.TicketPriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "TicketTypeId", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles="Administrator,Project Manager, Developer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            //if user is admin or pm then enable them to assign devs to ticket
            if (User.IsInRole("Administrator") || User.IsInRole("Project Manager"))
            {
                var Devs = db.Users.Where(u => u.Roles.Any(r => r.RoleId == (db.Roles.FirstOrDefault(ru => ru.Name == "Developer")).Id));
                ViewBag.AssignedToUserId = new SelectList(Devs, "Id", "DisplayName", ticket.AssignedToUserId);
            }
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "TicketStatusId", "Name", ticket.TicketStatusId);
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "TicketPriorityId", "Name", ticket.TicketPriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "TicketTypeId", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ticket ticket)
        {
            

            if (ModelState.IsValid)
            {
                //ticket.OwnerUserId = db.Users.FirstOrDefault(u => u.Id == OwnerId).Id;
                //ticket.Owner = db.Users.FirstOrDefault(u => u.Id == ticket.OwnerUserId);

                ticket.Updated = System.DateTime.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (User.IsInRole("Administrator") || User.IsInRole("Project Manager"))
            {
                var Devs = db.Users.Where(u => u.Roles.Any(r => r.RoleId == (db.Roles.FirstOrDefault(ru => ru.Name == "Developer")).Id));
                ViewBag.AssignedToUserId = new SelectList(Devs, "Id", "DisplayName", ticket.AssignedToUserId);
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "TicketPriorityId", "Name", ticket.TicketPriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "TicketTypeId", "Name", ticket.TicketTypeId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "TicketStatusId", "Name", ticket.TicketStatus);

            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
