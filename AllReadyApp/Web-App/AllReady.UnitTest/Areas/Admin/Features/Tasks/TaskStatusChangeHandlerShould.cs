using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Moq;
using Xunit;
using Shouldly;
using TaskStatus = AllReady.Areas.Admin.Features.Tasks.TaskStatus;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class TaskStatusChangeHandlerShould : InMemoryContextTest
    {
        private readonly Mock<IMediator> mediator;
        private readonly TaskStatusChangeHandler handler;
        private readonly DateTime dateTimeUtcNow = DateTime.UtcNow;

        public TaskStatusChangeHandlerShould()
        {
            mediator = new Mock<IMediator>();
            handler = new TaskStatusChangeHandler(Context, mediator.Object) { DateTimeUtcNow = () => dateTimeUtcNow };
        }

        protected override void LoadTestData()
        {
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
            Context.Users.Add(user1);

            htb.Campaigns.Add(firePrev);
            Context.Organizations.Add(htb);
            Context.Events.Add(queenAnne);

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

            Context.Tasks.Add(newTask);

            Context.SaveChanges();
        }

        protected async Task InitStatus(TaskStatus status)
        {
            var taskSignup = Context.TaskSignups.First();
            taskSignup.Status = status.ToString();
            await Context.SaveChangesAsync();
        }

        protected TaskStatusChangeCommand CreateCommand(TaskStatus status, string description = "")
        {
            var user = Context.Users.First();
            var theTask = Context.Tasks.First();

            return new TaskStatusChangeCommand
            {
                UserId = user.Id,
                TaskId = theTask.Id,
                TaskStatus = status,
                TaskStatusDescription = description ?? string.Empty
            };
        }

        [Fact]
        public async Task VolunteerAssignedTask()
        {
            var theTask = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommand
            {
                TaskId = theTask.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.Assigned,
                TaskStatusDescription = $"Assign {theTask.Name} to {user.UserName}"
            };
            await handler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus.ToString());
            taskSignup.Task.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerAcceptsTaskFromAssignedStatus()
        {
            var theTask = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommand
            {
                TaskId = theTask.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.Accepted,
                TaskStatusDescription = $"{user.UserName} has accepted the task {theTask.Name}"
            };
            await handler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus.ToString());
            taskSignup.Task.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerAcceptsTaskFromCanNotCompleteStatus()
        {
            var taskSignup = Context.TaskSignups.First();
            taskSignup.Status = TaskStatus.CanNotComplete.ToString();
            await Context.SaveChangesAsync();

            var theTask = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommand
            {
                TaskId = theTask.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.Accepted,
                TaskStatusDescription = $"{user.UserName} accepted task {theTask.Name}"
            };

            await handler.Handle(command);

            taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus.ToString());
            taskSignup.Task.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerAcceptsTaskFromCompletedStatus()
        {
            var taskSignup = Context.TaskSignups.First();
            taskSignup.Status = TaskStatus.Completed.ToString();
            await Context.SaveChangesAsync();

            var theTask = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommand
            {
                TaskId = theTask.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.Accepted,
                TaskStatusDescription = $"{user.UserName} accepted task {theTask.Name}"
            };

            await handler.Handle(command);

            taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus.ToString());
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.Task.Id.ShouldBe(command.TaskId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerAcceptsTaskFromAcceptedStatusShouldThrow()
        {
            await InitStatus(TaskStatus.Accepted);
            var command = CreateCommand(TaskStatus.Accepted, "User accepted task");
            Should.Throw<Exception>(() => handler.Handle(command));
        }

        [Fact]
        public async Task VolunteerAcceptsTaskFromRejectedStatusShouldThrow()
        {
            await InitStatus(TaskStatus.Rejected);
            var command = CreateCommand(TaskStatus.Accepted, "User accepted task");
            Should.Throw<Exception>(() => handler.Handle(command));
        }

        [Fact]
        public async Task VolunteerRejectsTask()
        {
            var theTask = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommand
            {
                TaskId = theTask.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.Rejected,
                TaskStatusDescription = $"{user.UserName} rejected task {theTask.Name}"
            };
            await handler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifiyCommand => notifiyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus.ToString());
            taskSignup.Task.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerRejectsTaskFromAcceptedStatusShouldThrow()
        {
            await InitStatus(TaskStatus.Accepted);
            var command = CreateCommand(TaskStatus.Rejected, "User rejects task");
            Should.Throw<Exception>(() => handler.Handle(command));
        }

        [Fact]
        public async Task VolunteerRejectsTaskFromRejectedStatusShouldThrow()
        {
            await InitStatus(TaskStatus.Rejected);
            var command = CreateCommand(TaskStatus.Rejected, "User rejects task");
            Should.Throw<Exception>(() => handler.Handle(command));
        }

        [Fact]
        public async Task VolunteerRejectsTaskFromCompletedStatusShouldThrow()
        {
            await InitStatus(TaskStatus.Completed);
            var command = CreateCommand(TaskStatus.Rejected, "User rejected task");
            Should.Throw<Exception>(() => handler.Handle(command));
        }

        [Fact]
        public async Task VolunteerRejectsTaskFromCanNotCompleteStatusShouldThrow()
        {
            await InitStatus(TaskStatus.CanNotComplete);
            var command = CreateCommand(TaskStatus.CanNotComplete, "User rejected task");
            Should.Throw<Exception>(() => handler.Handle(command));
        }

        [Fact]
        public async Task VolunteerCompletesTaskFromAcceptedStatus()
        {
            var taskSignup = Context.TaskSignups.First();
            taskSignup.Status = TaskStatus.Accepted.ToString();
            await Context.SaveChangesAsync();

            var theTask = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommand
            {
                TaskId = theTask.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.Completed,
                TaskStatusDescription = $"{user.UserName} completed {theTask.Name}"
            };
            await handler.Handle(command);

            taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus.ToString());
            taskSignup.Task.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerCompletesTaskFromAssignedStatus()
        {
            var theTask = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommand
            {
                TaskId = theTask.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.Completed,
                TaskStatusDescription = $"{user.UserName} completed task {theTask.Name}"
            };

            await handler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus.ToString());
            taskSignup.Task.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
            taskSignup.StatusDateTimeUtc.ShouldBe(dateTimeUtcNow);
        }

        [Fact]
        public async Task VolunteerCompletesTaskFromRejectedStatusShouldThrow()
        {
            await InitStatus(TaskStatus.Rejected);
            var command = CreateCommand(TaskStatus.Completed, "User completed task");
            Should.Throw<Exception>(() => handler.Handle(command));
        }

        [Fact]
        public async Task VolunteerCompletesTaskFromCompletedStatusShouldThrow()
        {
            await InitStatus(TaskStatus.Completed);
            var command = CreateCommand(TaskStatus.Completed, "User completed task");
            Should.Throw<Exception>(() => handler.Handle(command));
        }

        [Fact]
        public async Task VolunteerCompletesTaskFromCanNotCompleteStatusShouldThrow()
        {
            await InitStatus(TaskStatus.CanNotComplete);
            var command = CreateCommand(TaskStatus.Completed, "User completed task");
            Should.Throw<Exception>(() => handler.Handle(command));
        }

        [Fact]
        public async Task VolunteerCannotCompleteTaskFromAcceptedStatus()
        {
            var taskSignup = Context.TaskSignups.First();
            taskSignup.Status = TaskStatus.Accepted.ToString();
            await Context.SaveChangesAsync();

            var theTask = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new TaskStatusChangeCommand
            {
                TaskId = theTask.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.CanNotComplete,
                TaskStatusDescription = $"{user.UserName} cannot complete {theTask.Name}"
            };
            await handler.Handle(command);

            taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus.ToString());
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.Task.Id.ShouldBe(command.TaskId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerCannotCompleteTaskFromAssignedStatus()
        {
            var user = Context.Users.First();
            var theTask = Context.Tasks.First();
            var command = new TaskStatusChangeCommand
            {
                TaskId = theTask.Id,
                UserId = user.Id,
                TaskStatus = TaskStatus.CanNotComplete,
                TaskStatusDescription = $"{user.UserName} cannot complete task {theTask.Name}"
            };

            await handler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus.ToString());
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.Task.Id.ShouldBe(command.TaskId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
            taskSignup.StatusDateTimeUtc.ShouldBe(dateTimeUtcNow);
        }

        [Fact]
        public async Task VolunteerCannotCompleteTaskFromRejectedStatusShouldThrow()
        {
            await InitStatus(TaskStatus.Rejected);
            var command = CreateCommand(TaskStatus.CanNotComplete, "User cannot complete task");
            Should.Throw<Exception>(() => handler.Handle(command));
        }

        [Fact]
        public async Task VolunteerCannotCompleteTaskFromCompletedStatusShouldThrow()
        {
            await InitStatus(TaskStatus.Completed);
            var command = CreateCommand(TaskStatus.CanNotComplete, "User cannot complete task");
            Should.Throw<Exception>(() => handler.Handle(command));
        }

        [Fact]
        public async Task VolunteerCannotCompleteTaskFromCanNotCompleteStatusShouldThrow()
        {
            await InitStatus(TaskStatus.CanNotComplete);
            var command = CreateCommand(TaskStatus.CanNotComplete, "User cannot complete task");
            Should.Throw<Exception>(() => handler.Handle(command));
        }

        [Fact]
        public void ShouldThrowIfTaskDoesNotExist()
        {
            var command = CreateCommand(TaskStatus.Accepted, "User accepted task");
            command.TaskId = 90124;
            Should.Throw<Exception>(() => handler.Handle(command));
        }

        [Fact]
        public void ShouldThrowIfTaskSignupDoesNotExist()
        {
            var command = CreateCommand(TaskStatus.Completed, "User completed task");
            command.UserId = Guid.NewGuid().ToString();
            Should.Throw<Exception>(() => handler.Handle(command));
        }
    }
}
