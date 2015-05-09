using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTrackerv2.Models.HelperFolder;
using BugTrackerv2.Models;
using BugTrackerv2.Models.CustomViewModels;
namespace BugTrackerv2.Controllers
{
    [Authorize(Roles="Administrator")]
    public class RoleController : Controller
    {
        UserRolesHelper helper = new UserRolesHelper();
        ApplicationDbContext db = new ApplicationDbContext();


        // GET: Role
        // Display a list of Roles and users currently in that role
        public ActionResult Index()
        {
            

            var roles = db.Roles.ToList();
            var users = db.Users.ToList();
           
            //items.Add(new SelectListItem { Text = users });
            var model = new UserRoleViewModel
            {
                UserList = users,
                RoleList = roles,                
            };
            var roless = db.Users.Where(u => u.Roles.All(r => r.UserId != u.Id)).ToList();
            ViewBag.noRole = new MultiSelectList(db.Users.Where(u => u.Roles.All(r => r.UserId!= u.Id)), "Id", "DisplayName");

            return View(model);
        }

        public ActionResult EditRole(string RoleName, string query)
        {
            var usersinRole = helper.UsersInRole(RoleName).Select(u => u.Id);
            //var usersnotinRole = helper.UsersNotInRole(RoleName);

            //if(query != null)
            //{
            //    ViewBag.query = query;
            //    usersnotinRole = usersnotinRole.Where(s => s.UserName.Contains(query)).ToList();
            //    usersinRole = usersinRole.Where(s => s.UserName.Contains(query)).ToList();
            //}

            //var ToBeAdded = new MultiSelectList(usersnotinRole, "Id", "UserName");
            //var ToBeRemoved = new MultiSelectList(usersinRole, "Id", "UserName");


            var model = new UnifiedRoleView
            {
                RoleId = db.Roles.FirstOrDefault(r => r.Name == RoleName).Id,
                RoleName = RoleName,
                Users =  new MultiSelectList(db.Users,"Id","DisplayName",usersinRole),
                //AddedUsers = ToBeAdded,
                //DeletedUsers = ToBeRemoved
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(UnifiedRoleView model)
        {
            if(ModelState.IsValid)
            {
                foreach(var user in db.Users)
                {
                    if(model.Selected != null && model.Selected.Contains(user.Id))
                    {
                        helper.AddUserToRole(user.Id, model.RoleName);
                    }
                    else
                    {
                        helper.RemoveUserFromRole(user.Id, model.RoleName);
                    }
                }
            }
            return RedirectToAction("EditRole", new { RoleName = model.RoleName });
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(UnifiedRoleView UserView)
        {
            if (ModelState.IsValid)
            {
                //people that were selected. string list of selected users
                var selected = UserView.AddedSelect;
                foreach (var user in selected)
                {
                    helper.AddUserToRole(user, UserView.RoleName);
                }

            }
            return RedirectToAction("EditRole", new { RoleName = UserView.RoleName });
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRole(UnifiedRoleView UserView)
        {
            if (ModelState.IsValid)
            {
                var selected = UserView.DeletedSelect;
                foreach (var user in selected)
                {
                    helper.RemoveUserFromRole(user, UserView.RoleName);
                }
            }
            return RedirectToAction("EditRole", new { RoleName = UserView.RoleName });
        }
    }
}