using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTrackerv2.Models.CustomViewModels
{
    public class testingsometingViewModel
    {
        public string RoleName { get; set; }
        public int projectId { get; set; }
        public string projectName { get; set; }
        public MultiSelectList Users { get; set; }
        public string[] selected { get; set; }
    }
}