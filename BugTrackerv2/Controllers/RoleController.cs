﻿using System;
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
            List<SelectListItem> items = new List<SelectListItem>();

            var roles = db.Roles.ToList();
            var users = db.Users.ToList();
            
            //items.Add(new SelectListItem { Text = users });
            var model = new UserRoleViewModel
            {
                UserList = users,
                RoleList = roles
            };
            return View(model);
        }

        public ActionResult EditRole(string RoleName)
        {
            var usersinRole = helper.UsersInRole(RoleName);
            var usersnotinRole = helper.UsersNotInRole(RoleName);
            //contains a list of ppl not in role to be added
            var ToBeAdded = new MultiSelectList(usersnotinRole, "Id", "UserName");
            var ToBeRemoved = new MultiSelectList(usersinRole, "Id", "UserName");
            var model = new UnifiedRoleView
            {
                RoleId = db.Roles.FirstOrDefault(r => r.Name == RoleName).Id,
                RoleName = RoleName,
                AddedUsers = ToBeAdded,
                DeletedUsers = ToBeRemoved
            };
            return View(model);
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