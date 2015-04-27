using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerv2.Models.HelperFolder
{
    public class UserRolesHelper
    {
        //function global usermanager object to obtain user info like roles
        private UserManager<ApplicationUser> manager =
            new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(
                    new ApplicationDbContext()));

        public int UserId;
        public int RoleId;

        //add wrappers for basic usermanager operations
        public bool IsUserInRole(string userId, string roleName)
        {
            return manager.IsInRole(userId, roleName);
        }

        public IList<string> ListUserRoles(string userId)
        {
            return manager.GetRoles(userId);
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            var result = manager.AddToRole(userId, roleName);
            return result.Succeeded;
        }

        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = manager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        //custom operations: users in a role & users not in a role
        public IList<ApplicationUser> UsersInRole(string roleName)
        {
            var db = new ApplicationDbContext();
            var resultList = new List<ApplicationUser>();
            var users = db.Users.Where(user => user.Roles.Any(role => role.RoleId == db.Roles.FirstOrDefault(r => r.Name == roleName).Id));
            return users.ToList();
        }

        public IList<ApplicationUser> UsersNotInRole(string roleName)
        {
            var db = new ApplicationDbContext();
            var users = db.Users.Where(user => user.Roles.All(ur => ur.RoleId != db.Roles.FirstOrDefault(r => r.Name == roleName).Id)).ToList();
            return users;
        }
    }
}