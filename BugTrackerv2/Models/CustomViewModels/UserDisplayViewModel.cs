using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerv2.Models.CustomViewModels
{
    public class UserDisplayViewModel
    {
        public List<ApplicationUser> PM { get; set; }
        public List<ApplicationUser> Devs { get; set; }
        public string projectName { get; set; }
        public int? projectId { get; set; }
    }
}