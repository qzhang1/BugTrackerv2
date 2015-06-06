namespace BugTrackerv2.Migrations
{
    using BugTrackerv2.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using BugTrackerv2.Models.TicketFolder;
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

            //create guess/admin login
            ApplicationUser guess;
            if(!context.Users.Any(r => r.Email == "Administrator@Test.com"))
            {
                guess = new ApplicationUser
                {
                    UserName = "Administrator@Test.com",
                    Email="Administrator@Test.com",
                    DisplayName = "Guess"
                };
                userManager.Create(guess, "MjjdHoB21124");
            }
            else
            {
                guess = context.Users.Single(u => u.Email == "Administrator@Test.com");
            }

            if(!userManager.IsInRole(guess.Id, "Administrator"))
            {
                userManager.AddToRole(guess.Id, "Administrator");
            }

            //TICKETTYPES
            if(!context.TicketTypes.Any(tt => tt.Name == "Defect"))
            {
                TicketType defect = new TicketType
                {
                    Name = "Defect",
                    TicketTypeId = 1
                };
                context.TicketTypes.Add(defect);
            }
            if (!context.TicketTypes.Any(tt => tt.Name == "Enhancement"))
            {
                TicketType enhancement = new TicketType
                {
                    Name = "Enhancement",
                    TicketTypeId = 2
                };
                context.TicketTypes.Add(enhancement);
            }
            if (!context.TicketTypes.Any(tt => tt.Name == "Task"))
            {
                TicketType task = new TicketType
                {
                    Name = "Task",
                    TicketTypeId = 3
                };
                context.TicketTypes.Add(task);
            }

            //TICKETSTATUSES
            if (!context.TicketStatuses.Any(tt => tt.Name == "New"))
            {
                TicketStatus New = new TicketStatus
                {
                    Name = "New",
                    TicketStatusId = 1
                };
                context.TicketStatuses.Add(New);
            }
            if (!context.TicketStatuses.Any(tt => tt.Name == "Assigned"))
            {
                TicketStatus Assigned = new TicketStatus
                {
                    Name = "Assigned",
                    TicketStatusId = 2
                };
                context.TicketStatuses.Add(Assigned);
            }
            if (!context.TicketStatuses.Any(tt => tt.Name == "Accepted"))
            {
                TicketStatus Accepted = new TicketStatus
                {
                    Name = "Accepted",
                    TicketStatusId = 3
                };
                context.TicketStatuses.Add(Accepted);
            }
            if (!context.TicketStatuses.Any(tt => tt.Name == "Closed"))
            {
                TicketStatus Closed = new TicketStatus
                {
                    Name = "Closed",
                    TicketStatusId = 4
                };
                context.TicketStatuses.Add(Closed);
            }
            if (!context.TicketStatuses.Any(tt => tt.Name == "Re-Opened"))
            {
                TicketStatus ReOpen = new TicketStatus
                {
                    Name = "Re-Opened",
                    TicketStatusId = 5
                };
                context.TicketStatuses.Add(ReOpen);
            }
            if (!context.TicketStatuses.Any(tt => tt.Name == "Canceled"))
            {
                TicketStatus canceled = new TicketStatus
                {
                    Name = "Canceled",
                    TicketStatusId = 6
                };
                context.TicketStatuses.Add(canceled);
            }


            //TICKETPRIORITIES
            if(!context.TicketPriorities.Any(tt => tt.Name == "Low"))
            {
                TicketPriority low = new TicketPriority
                {
                    Name = "Low",
                    TicketPriorityId = 1
                };
                context.TicketPriorities.Add(low);
            }
            if (!context.TicketPriorities.Any(tt => tt.Name == "Medium"))
            {
                TicketPriority medium = new TicketPriority
                {
                    Name = "Medium",
                    TicketPriorityId = 2
                };
                context.TicketPriorities.Add(medium);
            }
            if (!context.TicketPriorities.Any(tt => tt.Name == "High"))
            {
                TicketPriority high = new TicketPriority
                {
                    Name = "High",
                    TicketPriorityId = 3
                };
                context.TicketPriorities.Add(high);
            }
            if (!context.TicketPriorities.Any(tt => tt.Name == "Emergency"))
            {
                TicketPriority emergency = new TicketPriority
                {
                    Name = "Emergency",
                    TicketPriorityId = 4
                };
                context.TicketPriorities.Add(emergency);
            }
        }
    }
}
