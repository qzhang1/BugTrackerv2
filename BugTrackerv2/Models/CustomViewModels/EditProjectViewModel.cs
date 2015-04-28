using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTrackerv2.Models.CustomViewModels
{
    public class EditProjectViewModel
    {
        public string RoleName { get; set; }
        public int projectId { get; set; }
        public string projectName { get; set; }

        public MultiSelectList UsersToBeAdded { get; set; }
        public MultiSelectList UsersToBeRemoved { get; set; }
        public string[] SelectUsersToAdd { get; set; }
        public string[] SelectUsersToRemove { get; set; }
    }
}