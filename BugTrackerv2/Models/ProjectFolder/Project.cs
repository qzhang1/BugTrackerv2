using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerv2.Models.ProjectFolder
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}