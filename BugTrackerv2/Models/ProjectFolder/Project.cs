using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace BugTrackerv2.Models.ProjectFolder
{
    public class Project
    {
        public Project()
        {
            this.Users = new HashSet<ApplicationUser>();
        }

        public int ProjectId { get; set; }
        [Display(Name="Project")]
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}