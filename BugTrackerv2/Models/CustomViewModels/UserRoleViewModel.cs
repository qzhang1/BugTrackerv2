using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerv2.Models.CustomViewModels
{
    public class UserRoleViewModel
    {

        public List<Microsoft.AspNet.Identity.EntityFramework.IdentityRole> RoleList { get; set; }
        public List<ApplicationUser> UserList { get; set; }
        public List<ApplicationUser> noRoles { get; set; }
    }
}