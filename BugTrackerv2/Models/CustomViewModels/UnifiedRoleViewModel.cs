﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTrackerv2.Models.ProjectFolder;
namespace BugTrackerv2.Models.CustomViewModels
{
    public class UnifiedRoleView
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        //create multiselectlist objects
        public MultiSelectList AddedUsers { get; set; }
        public MultiSelectList DeletedUsers { get; set; }
        public MultiSelectList Users { get; set; }
        //string array of the items selected
        public string[] AddedSelect { get; set; }
        public string[] DeletedSelect { get; set; }
        public string[] Selected { get; set; }
        public Project Project { get; set; }
    }
}