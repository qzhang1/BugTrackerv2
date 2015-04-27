using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTrackerv2.Models.CustomViewModels;
using BugTrackerv2.Models;
using BugTrackerv2.Models.HelperFolder;
namespace BugTrackerv2.Controllers
{
    public class ProjectUsersController : Controller
    {
        private UserProjectsHelper helper = new UserProjectsHelper();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProjectUsers
        public ActionResult EditUsers(int Id, string RoleName)
        {
            //edit users: find the current project, then find the List of users not on the project and those with the role of project manager
            //create a list of users on the project with role of pm
            //add to the view model another multiselectlist to display users to be added and users to be removed
            var RoleId = db.Roles.FirstOrDefault(r => r.Name == RoleName).Id;
            var project = db.Projects.Find(Id);
            var usersNotOnProject = helper.ListUsersNotOnProject(Id);
            var usersNotOnProjectInRole = usersNotOnProject.Where(u=>u.Roles.Any(r=>r.RoleId==RoleId));
            var usersOnProject = helper.ListUsersOnProject(Id);
            var usersOnProjectInRole = usersOnProject.Where(u => (u.Roles.Any(r => r.RoleId == RoleId)));
            var model = new ProjectUserViewModel
            {
                projectId = Id,
                projectName = project.Name,
                UsersToBeAdded = new MultiSelectList(usersNotOnProjectInRole, "Id","DisplayName"),
                UsersToBeRemoved = new MultiSelectList(usersOnProjectInRole, "Id", "DisplayName"),
                RoleName = RoleName
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Administrator, Project Manager")]
        public ActionResult AssignUsers(ProjectUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(model.SelectUsersToAdd != null)
                {
                    foreach (string id in model.SelectUsersToAdd)
                        helper.AddUsersToProject(id, model.projectId);
                    //return RedirectToAction("Index","Projects");
                }
                else
                {
                    //send error message back to view
                    ViewBag.ErrorMsg = "No Users were selected!";
                }
            }
            return RedirectToAction("EditUsers", new { Id = model.projectId, RoleName = model.RoleName});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Administrator, Project Manager")]
        public ActionResult RemoveUsers(ProjectUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(model.SelectUsersToRemove != null)
                {
                    foreach (string id in model.SelectUsersToRemove)
                        helper.RemoveUserFromProject(id, model.projectId);
                    //return RedirectToAction("Index", "Projects");
                }
                else
                {
                    //send error message back to view
                    ViewBag.ErrorMsg = "No Users were selected!";
                }
            }

            return RedirectToAction("EditUsers", new { Id = model.projectId, RoleName = model.RoleName });
        }
    }

    
}