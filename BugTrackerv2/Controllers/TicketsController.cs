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
            return View(ticket);
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
                ticket.OwnerUserId = db.Users.FirstOrDefault(u => u.Id == OwnerId).Id;
                ticket.Owner = db.Users.FirstOrDefault(u => u.Id == ticket.OwnerUserId);

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
        public ActionResult Edit([Bind(Include = "TicketId,Title,Description,Created,Updated,OwnerUserId,AssignedToUserId,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "TicketPriorityId", "Name", ticket.TicketPriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "TicketTypeId", "Name", ticket.TicketTypeId);
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
