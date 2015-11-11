using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Areas.Admin.Models;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllReady.UnitTest.Tasks
{
    public class SignupStatusChange : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            Tenant htb = new Tenant()
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };
            Campaign firePrev = new Campaign()
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingTenant = htb
            };
            Activity queenAnne = new Activity()
            {
                Id = 1,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTimeUtc = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTimeUtc = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                Tenant = htb,
                RequiredSkills = new List<ActivitySkill>(),
            };

            var username1 = $"blah@1.com";
            var username2 = $"blah@2.com";

            var user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true };
            context.Users.Add(user1);

            htb.Campaigns.Add(firePrev);
            context.Tenants.Add(htb);
            context.Activities.Add(queenAnne);

            var activitySignups = new List<ActivitySignup>();
            activitySignups.Add(new ActivitySignup { Activity = queenAnne, User = user1, SignupDateTime = DateTime.UtcNow });

            context.ActivitySignup.AddRange(activitySignups);

            var newTask = new AllReadyTask()
            {
                Activity = queenAnne,
                Description = "Description of a very important task",
                Name = "Task # 1",
                EndDateTimeUtc = DateTime.Now.AddDays(5),
                StartDateTimeUtc = DateTime.Now.AddDays(3),
                Tenant = htb
            };
            newTask.AssignedVolunteers.Add(new TaskSignup()
            {
                Task = newTask,
                User = user1
            });
            context.Tasks.Add(newTask);

            context.SaveChanges();
        }

        [Fact]
        public void VolunteerAcceptsTask()
        {
            var bus = new Mock<IMediator>();

            var task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommand
            {
                TaskId = task.Id, UserId = user.Id, TaskStatus = TaskStatus.Accepted
            };
            var handler = new TaskStatusChangeHandler(Context, bus.Object);
            var result = handler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            bus.Verify(b => b.Publish(It.Is<TaskSignupStatusChanged>(notifyCommand =>
                   notifyCommand.SignupId == taskSignup.Id
            )), Times.Once());
        }
    }
}
