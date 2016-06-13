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
using Shouldly;
using TaskStatus = AllReady.Areas.Admin.Features.Tasks.TaskStatus;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class TaskStatusChangeHandlerTests : InMemoryContextTest
    {

        Mock<IMediator> mediator;
        TaskStatusChangeHandlerAsync handler;

        public TaskStatusChangeHandlerTests()
            : base()
        {
            mediator = new Mock<IMediator>();
            handler = new TaskStatusChangeHandlerAsync(Context, mediator.Object);
        }

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

            var queenAnne = new Event
            {
                Id = 1,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<EventSkill>(),
            };

            var username1 = $"blah@1.com";

            var user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true };
            context.Users.Add(user1);

            htb.Campaigns.Add(firePrev);
            context.Organizations.Add(htb);
            context.Events.Add(queenAnne);

            var eventSignups = new List<EventSignup>
            {
                new EventSignup { Event = queenAnne, User = user1, SignupDateTime = DateTime.UtcNow }
            };

            context.EventSignup.AddRange(eventSignups);

            var newTask = new AllReadyTask
            {
                Event = queenAnne,
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
        public async Task VolunteerAssignedTask()
        {
            var dateTime = DateTime.UtcNow;
            var task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommandAsync
            {
                TaskId = task.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.Assigned,
                TaskStatusDescription = $"Assign {task.Name} to {user.UserName}"
            };
            await handler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            // Verify that the handle method publishes a notification that the task status changed
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            // Make sure the task signup properties are the expected values
            // based on the command.
            taskSignup.Status.ShouldBe(command.TaskStatus.ToString());
            taskSignup.Task.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
            // the datetimes should be within 1 second of each other
            taskSignup.StatusDateTimeUtc.ShouldBe(dateTime, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task VolunteerAcceptsTask()
        {
            var dateTime = DateTime.UtcNow;
            var task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommandAsync
            {
                TaskId = task.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.Accepted,
                TaskStatusDescription = $"{user.UserName} has accepted the task {task.Name}"
            };
            await handler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            // Verify that the handle method publishes a notification that the task status changed
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            // Make sure the task signup properties are the expected values
            // based on the command.
            taskSignup.Status.ShouldBe(command.TaskStatus.ToString());
            taskSignup.Task.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
            // the datetimes should be within 1 second of each other
            taskSignup.StatusDateTimeUtc.ShouldBe(dateTime, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task VolunteerRejectsTask()
        {
            var task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommandAsync
            {
                TaskId = task.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.Rejected
            };
            await handler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            // Verify that the handle method publishes a notification that the task status changed.
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifiyCommand => notifiyCommand.SignupId == taskSignup.Id)), Times.Once());
            // Make sure the task signup has the expected status
            taskSignup.Status.ShouldBe(TaskStatus.Rejected.ToString());
        }

        [Fact]
        public async Task VolunteerCompletesTask()
        {
            // set the current task signup instance to Accepted
            var taskSignup = Context.TaskSignups.First();
            taskSignup.Status = TaskStatus.Accepted.ToString();
            await Context.SaveChangesAsync();

            var task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommandAsync
            {
                TaskId = task.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.Completed
            };
            await handler.Handle(command);

            taskSignup = Context.TaskSignups.First();
            // Verify that the handle method publishes a notification that the task status changed
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            // Make sure the task signup has the expected status
            taskSignup.Status.ShouldBe(TaskStatus.Completed.ToString());
        }

        [Fact]
        public async Task VolunteerCannotCompleteTask()
        {
            // set the current task signup instance to Accepted
            var taskSignup = Context.TaskSignups.First();
            taskSignup.Status = TaskStatus.Accepted.ToString();
            await Context.SaveChangesAsync();

            var task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommandAsync
            {
                TaskId = task.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.CanNotComplete
            };
            await handler.Handle(command);

            taskSignup = Context.TaskSignups.First();
            // Verify that the handle method publishes a notification that the task status changed
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            // Make sure the task signup has the expected status
            taskSignup.Status.ShouldBe(TaskStatus.CanNotComplete.ToString());
        }
    }
}
