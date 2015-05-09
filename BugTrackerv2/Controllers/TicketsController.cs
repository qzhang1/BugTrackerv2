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
using System.IO;
using System.Text;
using SendGrid;
using System.Net.Mail;
using BugTrackerv2.Models.CustomViewModels;
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
                return View(db.Tickets.OrderByDescending(c => c.Created).ToList());
            else if (User.IsInRole("Submitter"))
                return View(db.Tickets.Where(t => t.OwnerUserId == userId).OrderByDescending(c => c.Created).ToList());
            else
                return View((db.Tickets.Where(t => t.project.Users.Any(u => u.Id == userId)).OrderByDescending(c => c.Created)).ToList());
        }

        // GET: Tickets/Details/5
        [Authorize()]
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
            //if the PM/Dev is not apart of this project then not permitted to view this page. re-route user back to index
            //unable to comment or attach anything to this ticket            
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Administrator"))
            {
                ticket.TicketComments = ticket.TicketComments.OrderByDescending(o => o.Created).ToList();
                return View(ticket);
            }
            else if(User.IsInRole("Project Manager") || User.IsInRole("Developer"))
            {
                var projects = db.Users.FirstOrDefault(u => u.Id == userId).Projects.ToList();           //projects the current user is involved in
                foreach(var project in projects)
                {
                    if(project.ProjectId == ticket.ProjectId)
                    {
                        ticket.TicketComments = ticket.TicketComments.OrderByDescending(o => o.Created).ToList();
                        return View(ticket);
                    }
                }
            }
            else if(User.IsInRole("Submitter") && (ticket.OwnerUserId == userId))
            {
                ticket.TicketComments = ticket.TicketComments.OrderByDescending(o => o.Created).ToList();
                return View(ticket);
            }
            


            return RedirectToAction("Index");
        }

        [Authorize()]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(TicketComment Body)
        {
            if(ModelState.IsValid)
            {               
                    Body.Created = System.DateTime.Now;
                    Body.UserId = User.Identity.GetUserId();
                    db.TicketComments.Add(Body);

                    //notify dev when comment is added
                    var ticket = db.Tickets.FirstOrDefault(u => u.TicketId == Body.TicketId);
                    var notification = new TicketNotification
                    {
                        TicketId = Body.TicketId,
                        UserId = ticket.AssignedToUserId,
                        message = "The user, " + db.Users.FirstOrDefault(u => u.Id == Body.UserId).DisplayName + " has commented in the ticket titled, " + "<strong>" + "\"" + ticket.Title + "\""+ "</strong>",
                        Created = System.DateTime.Now,
                        read = false
                    };
                    db.TicketNotifications.Add(notification);
                    db.SaveChanges();
            }
            return RedirectToAction("Details", new { Id = Body.TicketId });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttachment(TicketAttachment attachment, HttpPostedFileBase file)
        {
            if(file != null && file.ContentLength > 0)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if(extension != ".png" && extension!=".jpg" && extension!=".gif" && extension!=".txt")
                {
                    ModelState.AddModelError("file", "Invalid format");
                }
            }
            if(ModelState.IsValid)
            {
                if(file != null)
                {
                    attachment.Created = System.DateTime.Now;
                    
                    attachment.UserId = User.Identity.GetUserId();
                    attachment.FilePath = "/img/attachments/";
                    var absPath = Server.MapPath("~" + attachment.FilePath);
                    attachment.FileUrl = attachment.FilePath + file.FileName;
                    file.SaveAs(Path.Combine(absPath, file.FileName));
                    attachment.FileName = file.FileName;
                }
                db.TicketAttachments.Add(attachment);
                var ticket = db.Tickets.FirstOrDefault(u => u.TicketId == attachment.TicketId);
                var notification = new TicketNotification
                {
                    TicketId = attachment.TicketId,
                    UserId = ticket.AssignedToUserId,
                    message = "The user, " + db.Users.FirstOrDefault(u => u.Id == attachment.UserId).DisplayName + " has added an attachment in the ticket titled, " + "<strong>" + "\"" + ticket.Title + "\"" + "</strong>",
                    Created = System.DateTime.Now,
                    read = false
                };
                db.TicketNotifications.Add(notification);
                db.SaveChanges();                
            }
            
            return RedirectToAction("Details", new { Id = attachment.TicketId });
        }
        
        // GET: Tickets/Create
        // any authorized person can create tickets
        [Authorize()]
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
            var userId = User.Identity.GetUserId();
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "TicketStatusId", "Name", ticket.TicketStatusId);
            ViewBag.ProjectId = new SelectList((db.Users.FirstOrDefault(u => u.Id ==userId).Projects), "ProjectId", "Name", ticket.ProjectId);
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
                
                /*
                 * Ticket history algorithm:
                 * get oldticket data
                 * for each property
                 *      compare values of oldticket to ticket
                 *      if different
                 *          make new tickethistory
                 *          add to db.history
                 * save changes to db.history
                 * */
                
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.TicketId == ticket.TicketId);
                //unique identifier to identify all the changes made by the user at once
                var EditId = Guid.NewGuid().ToString();
                var UserId = User.Identity.GetUserId();


                //tests for changes in AssignedTouserId
                if(oldTicket.AssignedToUserId != ticket.AssignedToUserId)
                {
                    var AssignedHistory = new TicketHistory
                    {
                        TicketId = ticket.TicketId,
                        UserId = UserId,
                        Property = "Assigned To",
                        OldValue = (oldTicket.AssignedToUserId == null? "Not yet assigned" : db.Users.FirstOrDefault(u => u.Id == oldTicket.AssignedToUserId).DisplayName),
                        NewValue = db.Users.FirstOrDefault(u => u.Id == ticket.AssignedToUserId).DisplayName,
                        Change = System.DateTimeOffset.Now,
                    };
                    db.TicketHistories.Add(AssignedHistory);
                    
                    //make a notification to developer that they've been assigned to a ticket
                    var notification = new TicketNotification
                    {
                        TicketId = ticket.TicketId,
                        UserId = ticket.AssignedToUserId,
                        message = "You've been assigned to ticket titled " + ticket.Title,
                        Created = System.DateTime.Now,
                        read = false
                    };
                    db.TicketNotifications.Add(notification);
                    new EmailService().SendAsync(new IdentityMessage
                    {
                        Subject = "You've been assigned a ticket",
                        Destination = db.Users.FirstOrDefault(u => u.Id == ticket.AssignedToUserId).Email,
                        Body = "You've been assigned to ticket titled " + ticket.Title + "."

                    });

                }

                if(oldTicket.Description != ticket.Description)
                {
                    var ChangedDescription = new TicketHistory
                    {
                        TicketId = ticket.TicketId,
                        UserId = UserId,
                        Property = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = ticket.Description,
                        Change = System.DateTimeOffset.Now,
                    };
                    db.TicketHistories.Add(ChangedDescription);

                    //make a notification that the submitter or whoever has changed the description of the ticket
                    if(ticket.AssignedToUserId != UserId)
                    {
                        var notification = new TicketNotification
                        {
                            TicketId = ticket.TicketId,
                            UserId = ticket.AssignedToUserId,
                            message = db.Users.FirstOrDefault(u => u.Id == UserId).DisplayName + " has changed " + ChangedDescription.Property + " from " + ChangedDescription.OldValue + " to " + ChangedDescription.NewValue,
                            Created = System.DateTime.Now,
                            read = false
                        };
                        db.TicketNotifications.Add(notification);
                    }
                    
                }

                if (oldTicket.ProjectId != ticket.ProjectId)
                {
                    var ChangedProject = new TicketHistory
                    {
                        TicketId = ticket.TicketId,
                        UserId = UserId,
                        Property = "Project",
                        OldValue = db.Projects.FirstOrDefault(p => p.ProjectId == oldTicket.ProjectId).Name,
                        NewValue = db.Projects.FirstOrDefault(p => p.ProjectId == ticket.ProjectId).Name,
                        Change = System.DateTimeOffset.Now,
                    };
                    db.TicketHistories.Add(ChangedProject);
                    if(ticket.AssignedToUserId != UserId)
                    {
                        var notification = new TicketNotification
                        {
                            TicketId = ticket.TicketId,
                            UserId = ticket.AssignedToUserId,
                            message = db.Users.FirstOrDefault(u => u.Id == UserId).DisplayName + " has changed " + ChangedProject.Property + " from " + ChangedProject.OldValue + " to " + ChangedProject.NewValue,
                            Created = System.DateTime.Now,
                            read = false
                        };
                        db.TicketNotifications.Add(notification);
                    }
                }

                if(oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                {
                    var ChangedPriority = new TicketHistory
                    {
                        TicketId = ticket.TicketId,
                        UserId = UserId,
                        Property = "Ticket Priority",
                        OldValue = db.TicketPriorities.FirstOrDefault(p => p.TicketPriorityId == oldTicket.TicketPriorityId).Name,
                        NewValue = db.TicketPriorities.FirstOrDefault(p => p.TicketPriorityId == ticket.TicketPriorityId).Name,
                        Change = System.DateTimeOffset.Now,
                    };
                    db.TicketHistories.Add(ChangedPriority);
                    if(ticket.AssignedToUserId != UserId)
                    {
                        var notification = new TicketNotification
                        {
                            TicketId = ticket.TicketId,
                            UserId = ticket.AssignedToUserId,
                            message = db.Users.FirstOrDefault(u => u.Id == UserId).DisplayName + " has changed " + ChangedPriority.Property + " from " + ChangedPriority.OldValue + " to " + ChangedPriority.NewValue,
                            Created = System.DateTime.Now,
                            read = false
                        };
                        db.TicketNotifications.Add(notification);
                    }

                }

                if (oldTicket.TicketStatusId != ticket.TicketStatusId)
                {
                    var ChangedStatus = new TicketHistory
                    {
                        TicketId = ticket.TicketId,
                        UserId = UserId,
                        Property = "Ticket Status",
                        OldValue = db.TicketStatuses.FirstOrDefault(p => p.TicketStatusId == oldTicket.TicketStatusId).Name,
                        NewValue = db.TicketStatuses.FirstOrDefault(p => p.TicketStatusId == ticket.TicketStatusId).Name,
                        Change = System.DateTimeOffset.Now,
                    };
                    db.TicketHistories.Add(ChangedStatus);
                   
                }

                if (oldTicket.TicketTypeId != ticket.TicketTypeId)
                {
                    var ChangedType = new TicketHistory
                    {
                        TicketId = ticket.TicketId,
                        UserId = UserId,
                        Property = "Ticket Type",
                        OldValue = db.TicketTypes.FirstOrDefault(p => p.TicketTypeId == oldTicket.TicketTypeId).Name,
                        NewValue = db.TicketTypes.FirstOrDefault(p => p.TicketTypeId == ticket.TicketTypeId).Name,
                        Change = System.DateTimeOffset.Now,
                    };
                    db.TicketHistories.Add(ChangedType);
                    
                }
            
                ticket.Updated = System.DateTime.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //remake form with selected options to display 
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
        [Authorize(Roles = "Administrator,Project Manager, Developer")]
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


        //TicketNotification Index List
        [Authorize()]
        public ActionResult Notifications()
        {
            var userid = User.Identity.GetUserId();
            var notifications = db.TicketNotifications.Include(u => u.User).Where(ui => ui.UserId == userid).ToList();
            foreach(var note in notifications)
            {
                note.Ticket = db.Tickets.FirstOrDefault(t => t.TicketId == note.TicketId);
            }
            return View(notifications);
        }

        //GET
        public ActionResult NotificationDetails(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketNotification notification = db.TicketNotifications.Find(Id);
            if (notification == null)
            {
                return HttpNotFound();
            }            
            return View(notification);
        }

        public ActionResult DeleteNotification(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketNotification notification = db.TicketNotifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            notification.Ticket = db.Tickets.FirstOrDefault(t => t.TicketId == notification.TicketId);
            return View(notification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNotification(int[] deletenotifications)
        {
            if(deletenotifications != null && deletenotifications.Length > 0)
            {
                foreach(var id in deletenotifications)
                {
                    var a = db.TicketNotifications.Find(id);
                    db.TicketNotifications.Remove(a);
                }
            }            
            db.SaveChanges();
            return RedirectToAction("Notifications", "Tickets");
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
