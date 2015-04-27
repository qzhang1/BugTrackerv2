using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerv2.Models.HelperFolder
{
    public class UserProjectsHelper
    {        

        private ApplicationDbContext db = new ApplicationDbContext();
        
        public bool IsOnProject(string userId, int projectId)
        {
            //using three db's will open/close database 3 times whereas if you express the three statements into one long statement
            //db.Projects.Find(projectId).Users.Any(u => u.Id == userId))       <-- this method is more efficient hits db only once just with a longer query
            /*
            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);
            var userList = project.Users.ToList();
            if(userList.Contains(user))
            {
                return true;
            }
            return false;
             * */
            //var result = db.Projects.Include(p => p.Users).FirstOrDefault(pr => pr.ProjectId == projectId).Users.ToList();

            if(db.Projects.Include(p => p.Users).FirstOrDefault(pr => pr.ProjectId == projectId).Users.Any(u => u.Id == userId))
            {
                return true;
            }
            return false;
        }

        public void AddUsersToProject(string userId, int projectId)
        {
            if(!IsOnProject(userId,projectId))
            {
                var project = db.Projects.Find(projectId);
                project.Users.Add(db.Users.Find(userId));
                db.Entry(project).State = System.Data.Entity.EntityState.Modified;          //instead of db.savechanges which saves everything this lets the db know to only update project
                db.SaveChanges();
            }
            
        }

        public void RemoveUserFromProject(string userId, int projectId)
        {
            if(IsOnProject(userId, projectId))
            {
                var project = db.Projects.Find(projectId);
                project.Users.Remove(db.Users.Find(userId));
                db.Entry(project).State = System.Data.Entity.EntityState.Modified;          //instead of db.savechanges which saves everything this lets the db know to only update project
                db.SaveChanges();
            }
        }

        public List<ApplicationUser> ListUsersOnProject(int projectId)
        {
            var result = db.Projects.Include(p=>p.Users).FirstOrDefault(pr => pr.ProjectId == projectId).Users.ToList();
            return result;
        }
        
        public List<ProjectFolder.Project> ListProjectsForUser(string userId)
        {
            return db.Users.Find(userId).Projects.ToList();
        }

        public List<ApplicationUser> ListUsersNotOnProject(int projectId)
        {

            var something = db.Users.Where(u => u.Projects.All(p => p.ProjectId != projectId)).ToList();
            //var something = db.Users.Include("Projects").Where(u => !(u.Projects.Any(p => p.ProjectId == projectId))).ToList();
            return something;
        }
    }
}