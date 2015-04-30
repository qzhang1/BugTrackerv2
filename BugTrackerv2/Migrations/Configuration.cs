namespace BugTrackerv2.Migrations
{
    using BugTrackerv2.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTrackerv2.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTrackerv2.Models.ApplicationDbContext context)
        {
            //create roles
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            //create a role called admin if not created yet
            if(!context.Roles.Any(r=>r.Name == "Administrator"))
            {
                roleManager.Create(new IdentityRole {Name="Administrator"});
            }
            //create a role called moderator if not created yet
            if(!context.Roles.Any(r=>r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            //create users
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            ApplicationUser user;
            if (!context.Users.Any(r => r.Email == "qzhang112@gmail.com"))
            {
                user = new ApplicationUser
                {
                    UserName = "qzhang112@gmail.com",
                    Email = "qzhang112@gmail.com",                    
                    DisplayName = "Qi Zhang"
                };

                userManager.Create(user, "qizzles");
            }
            else
            {
                user = context.Users.Single(u => u.Email == "qzhang112@gmail.com");           //check if user is in db if they already exists then look them up else create them
            }

            //assign users to their roles
            if (!userManager.IsInRole(user.Id, "Administrator"))                                     //if the user is not in the admin role
            {
                userManager.AddToRole(user.Id, "Administrator");
            }
        }
    }
}
