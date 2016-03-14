using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using TaskStatus = AllReady.Areas.Admin.Features.Tasks.TaskStatus;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class TaskStatusChangeHandlerTests : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var htb = new Organization
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var firePrev = new Campaign
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb
            };

            var queenAnne = new Activity
            {
                Id = 1,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<ActivitySkill>(),
            };

            var username1 = $"blah@1.com";

            var user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true };
            context.Users.Add(user1);

            htb.Campaigns.Add(firePrev);
            context.Organizations.Add(htb);
            context.Activities.Add(queenAnne);

            var activitySignups = new List<ActivitySignup>
            {
                new ActivitySignup { Activity = queenAnne, User = user1, SignupDateTime = DateTime.UtcNow }
            };

            context.ActivitySignup.AddRange(activitySignups);

            var newTask = new AllReadyTask
            {
                Activity = queenAnne,
                Description = "Description of a very important task",
                Name = "Task # 1",
                EndDateTime = DateTime.Now.AddDays(5),
                StartDateTime = DateTime.Now.AddDays(3),
                Organization = htb
            };

            newTask.AssignedVolunteers.Add(new TaskSignup
            {
                Task = newTask,
                User = user1
            });

            context.Tasks.Add(newTask);

            context.SaveChanges();
        }

        [Fact]
        public async Task VolunteerAcceptsTask()
        {
            var mediator = new Mock<IMediator>();

            var task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommandAsync
            {
                TaskId = task.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.Accepted
            };
            var handler = new TaskStatusChangeHandlerAsync(Context, mediator.Object);
            var result = handler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
        }
    }
}
