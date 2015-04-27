using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTrackerv2.Models;
using BugTrackerv2.Models.ProjectFolder;
using BugTrackerv2.Models.CustomViewModels;

namespace BugTrackerv2.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
        }
        
        public ActionResult AddPM(int? Id)
        {
            if(Id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var Project = db.Projects.Find(Id);
            if(Project == null)
            {
                return HttpNotFound();
            }

            //display all the project managers on one select list and all the PMs you want on the project
            var PMId = db.Roles.FirstOrDefault(r => r.Name == "Project Manager").Id;            
            var PMusers = db.Users.Where(u => u.Roles.Any(r => r.RoleId == PMId));
            var AlreadyPM = db.Projects.Where(p => p.Users.Any(u => u.Roles.Any(r => r.RoleId == PMId)));
            var ToBeAdded = new MultiSelectList(PMusers,"Id","UserName");
            var ToBeRemoved = new MultiSelectList(AlreadyPM, "Id", "UserName");
            var model = new UnifiedRoleView
            {
                Project = Project,
                AddedUsers = ToBeAdded,
                DeletedUsers = ToBeRemoved
            };

            return View(model);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPM(UnifiedRoleView model)
        {
            var currentProjectId = model.Project.ProjectId;
            var currentProject = db.Projects.Find(currentProjectId);
            if(ModelState.IsValid)
            {
                var selected = model.AddedSelect;
                foreach(var user in selected)
                {
                    var appuser = db.Users.FirstOrDefault(u => u.Id == user);
                    currentProject.Users.Add(appuser);
                }
                
                db.SaveChanges();                
            }
            return RedirectToAction("AddPM", new { Id = currentProject.ProjectId });
        }


        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            //send data about users in pm and dev roles via viewbag
            ViewBag.PMusers = db.Roles.Where(r => r.Name == "Project Manager").ToList();
            ViewBag.Devusers = db.Roles.Where(r => r.Name == "Developer").ToList();
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectId,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Created = System.DateTime.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProjectId,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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
