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
using BugTrackerv2.Models.HelperFolder;
using Microsoft.AspNet.Identity;
namespace BugTrackerv2.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserProjectsHelper helper = new UserProjectsHelper();

        // GET: Projects
        [Authorize(Roles="Administrator,Project Manager, Developer")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            if(User.IsInRole("Administrator"))
                return View(db.Projects.ToList());
            else
                return View (db.Users.FirstOrDefault(u => u.Id == userId).Projects.ToList());                            
        }


        

        // GET: Projects/Create
        [Authorize(Roles="Administrator")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Administrator")]
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
        [Authorize(Roles="Administrator,Project Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Include(p => p.Users).FirstOrDefault(pr => pr.ProjectId == id);
            var PM_Id = db.Roles.Single(r => r.Name == "Project Manager").Id;
            var Dev_Id = db.Roles.Single(r => r.Name == "Developer").Id;
            
            IEnumerable<ApplicationUser>listofusers;
            IEnumerable<string> UsersOnProject;
            if(User.IsInRole("Administrator"))
            {
                listofusers = db.Users.Where(u => u.Roles.Any(r => r.RoleId == PM_Id || r.RoleId == Dev_Id));           //list of users who are PMs and Devs
                 UsersOnProject = helper.ListUsersOnProject(project.ProjectId).Where(u => u.Roles.Any(r => r.RoleId == PM_Id || r.RoleId == Dev_Id)).Select(dn => dn.Id);
            }
            else
            {
                listofusers = db.Users.Where(u => u.Roles.Any(r => r.RoleId == Dev_Id));                                //list of users who are Devs
                 UsersOnProject = helper.ListUsersOnProject(project.ProjectId).Where(u => u.Roles.Any(r => r.RoleId == Dev_Id)).Select(dn => dn.Id);    //list users on project who are developers
            }
            TempData["project"] = project;
            
         
            var model = new testingsometingViewModel
            {
                projectName = project.Name,
                projectId = project.ProjectId,
                Users = new MultiSelectList(listofusers, "Id", "DisplayName",UsersOnProject)               

            };
            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Administrator, Project Manager")]
        public ActionResult Edit(testingsometingViewModel model)
        {
            if (ModelState.IsValid)
            {
                //track current project via projectId and change it's name
                var project = db.Projects.Include(u => u.Users).FirstOrDefault(p => p.ProjectId == model.projectId);
                project.Name = model.projectName; 

                //check what multiselectlists are selected and accomplish the associated tasks
                //add/remove project managers
                
                    foreach (var user in db.Users)
                    {                        

                        if(model.selected.Contains(user.Id))
                           {
                               helper.AddUsersToProject(user.Id, project.ProjectId);
                           }
                           else 
                           {
                               helper.RemoveUserFromProject(user.Id, project.ProjectId);
                           }
                       
                    }
                
                             

                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", new {Id = project.ProjectId });
            }
            return View(model);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles="Administrator")]
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
        [Authorize(Roles="Administrator")]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Projects/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //include is required since join table is implicitly created so include is necessary to tell 
            //compiler that there is a user navigation property there
            Project project = db.Projects.Include(p => p.Users).FirstOrDefault(pr => pr.ProjectId == id); ;
            if (project == null)
            {
                return HttpNotFound();
            }
            var model = new UserDisplayViewModel
            {
                projectName = project.Name,
                projectId = project.ProjectId,
                PM = project.Users.Where(u => u.Roles.Any(r => r.RoleId == (db.Roles.Single(rs => rs.Name == "Project Manager").Id))).ToList(),
                Devs = project.Users.Where(u => u.Roles.Any(r => r.RoleId == (db.Roles.Single(rs => rs.Name == "Developer").Id))).ToList()
            };
            return View(model);
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
