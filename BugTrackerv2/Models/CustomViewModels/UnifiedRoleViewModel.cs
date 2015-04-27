using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTrackerv2.Models.CustomViewModels
{
    public class UnifiedRoleView
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        //create multiselectlist objects
        public MultiSelectList AddedUsers { get; set; }
        public MultiSelectList DeletedUsers { get; set; }
        //string array of the items selected
        public string[] AddedSelect { get; set; }
        public string[] DeletedSelect { get; set; }
    }
}