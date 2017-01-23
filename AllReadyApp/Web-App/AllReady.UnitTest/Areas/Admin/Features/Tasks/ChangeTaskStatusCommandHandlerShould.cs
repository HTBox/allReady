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

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class ChangeTaskStatusCommandHandlerShould : InMemoryContextTest
    {
        private readonly Mock<IMediator> mediator;
        private readonly ChangeTaskStatusCommandHandler commandHandler;
        private readonly DateTime dateTimeUtcNow = DateTime.UtcNow;

        public ChangeTaskStatusCommandHandlerShould()
        {
            mediator = new Mock<IMediator>();
            commandHandler = new ChangeTaskStatusCommandHandler(Context, mediator.Object) { DateTimeUtcNow = () => dateTimeUtcNow };
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

            var newTask = new VolunteerTask
            {
                Event = queenAnne,
                Description = "Description of a very important task",
                Name = "Task # 1",
                EndDateTime = DateTime.Now.AddDays(5),
                StartDateTime = DateTime.Now.AddDays(3),
                Organization = htb
            };

            newTask.AssignedVolunteers.Add(new VolunteerTaskSignup
            {
                VolunteerTask = newTask,
                User = user1
            });

            Context.Tasks.Add(newTask);

            Context.SaveChanges();
        }

        protected async Task InitStatus(VolunteerTaskStatus status)
        {
            var taskSignup = Context.TaskSignups.First();
            taskSignup.Status = status;
            await Context.SaveChangesAsync();
        }

        protected ChangeTaskStatusCommand CreateCommand(VolunteerTaskStatus status, string description = "")
        {
            var user = Context.Users.First();
            var@task = Context.Tasks.First();

            return new ChangeTaskStatusCommand
            {
                UserId = user.Id,
                TaskId =@task.Id,
                TaskStatus = status,
                TaskStatusDescription = description ?? string.Empty
            };
        }

        [Fact]
        public async Task VolunteerAssignedTask()
        {
            var@task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new ChangeTaskStatusCommand
            {
                TaskId =@task.Id,
                UserId = user.Id,
                TaskStatus = VolunteerTaskStatus.Assigned,
                TaskStatusDescription = $"Assign {@task.Name} to {user.UserName}"
            };
            await commandHandler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus);
            taskSignup.VolunteerTask.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerAcceptsTaskFromAssignedStatus()
        {
            var@task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new ChangeTaskStatusCommand
            {
                TaskId =@task.Id,
                UserId = user.Id,
                TaskStatus = VolunteerTaskStatus.Accepted,
                TaskStatusDescription = $"{user.UserName} has accepted the task {@task.Name}"
            };
            await commandHandler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus);
            taskSignup.VolunteerTask.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerAcceptsTaskFromCanNotCompleteStatus()
        {
            var taskSignup = Context.TaskSignups.First();
            taskSignup.Status = VolunteerTaskStatus.CanNotComplete;
            await Context.SaveChangesAsync();

            var@task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new ChangeTaskStatusCommand
            {
                TaskId =@task.Id,
                UserId = user.Id,
                TaskStatus = VolunteerTaskStatus.Accepted,
                TaskStatusDescription = $"{user.UserName} accepted task {@task.Name}"
            };

            await commandHandler.Handle(command);

            taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus);
            taskSignup.VolunteerTask.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerAcceptsTaskFromCompletedStatus()
        {
            var taskSignup = Context.TaskSignups.First();
            taskSignup.Status = VolunteerTaskStatus.Completed;
            await Context.SaveChangesAsync();

            var@task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new ChangeTaskStatusCommand
            {
                TaskId =@task.Id,
                UserId = user.Id,
                TaskStatus = VolunteerTaskStatus.Accepted,
                TaskStatusDescription = $"{user.UserName} accepted task {@task.Name}"
            };

            await commandHandler.Handle(command);

            taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.VolunteerTask.Id.ShouldBe(command.TaskId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerAcceptsTaskFromAcceptedStatusShouldThrow()
        {
            await InitStatus(VolunteerTaskStatus.Accepted);
            var command = CreateCommand(VolunteerTaskStatus.Accepted, "User accepted task");
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }

        [Fact]
        public async Task VolunteerAcceptsTaskFromRejectedStatusShouldThrow()
        {
            await InitStatus(VolunteerTaskStatus.Rejected);
            var command = CreateCommand(VolunteerTaskStatus.Accepted, "User accepted task");
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }

        [Fact]
        public async Task VolunteerRejectsTask()
        {
            var @task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new ChangeTaskStatusCommand
            {
                TaskId = @task.Id,
                UserId = user.Id,
                TaskStatus = VolunteerTaskStatus.Rejected,
                TaskStatusDescription = $"{user.UserName} rejected task {@task.Name}"
            };
            await commandHandler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifiyCommand => notifiyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus);
            taskSignup.VolunteerTask.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerRejectsTaskFromAcceptedStatusShouldThrow()
        {
            await InitStatus(VolunteerTaskStatus.Accepted);
            var command = CreateCommand(VolunteerTaskStatus.Rejected, "User rejects task");
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }

        [Fact]
        public async Task VolunteerRejectsTaskFromRejectedStatusShouldThrow()
        {
            await InitStatus(VolunteerTaskStatus.Rejected);
            var command = CreateCommand(VolunteerTaskStatus.Rejected, "User rejects task");
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }

        [Fact]
        public async Task VolunteerRejectsTaskFromCompletedStatusShouldThrow()
        {
            await InitStatus(VolunteerTaskStatus.Completed);
            var command = CreateCommand(VolunteerTaskStatus.Rejected, "User rejected task");
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }

        [Fact]
        public async Task VolunteerRejectsTaskFromCanNotCompleteStatusShouldThrow()
        {
            await InitStatus(VolunteerTaskStatus.CanNotComplete);
            var command = CreateCommand(VolunteerTaskStatus.CanNotComplete, "User rejected task");
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }

        [Fact]
        public async Task VolunteerCompletesTaskFromAcceptedStatus()
        {
            var taskSignup = Context.TaskSignups.First();
            taskSignup.Status = VolunteerTaskStatus.Accepted;
            await Context.SaveChangesAsync();

            var @task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new ChangeTaskStatusCommand
            {
                TaskId = @task.Id,
                UserId = user.Id,
                TaskStatus = VolunteerTaskStatus.Completed,
                TaskStatusDescription = $"{user.UserName} completed {@task.Name}"
            };
            await commandHandler.Handle(command);

            taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus);
            taskSignup.VolunteerTask.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerCompletesTaskFromAssignedStatus()
        {
            var @task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new ChangeTaskStatusCommand
            {
                TaskId = @task.Id,
                UserId = user.Id,
                TaskStatus = VolunteerTaskStatus.Completed,
                TaskStatusDescription = $"{user.UserName} completed task {@task.Name}"
            };

            await commandHandler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus);
            taskSignup.VolunteerTask.Id.ShouldBe(command.TaskId);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
            taskSignup.StatusDateTimeUtc.ShouldBe(dateTimeUtcNow);
        }

        [Fact]
        public async Task VolunteerCompletesTaskFromRejectedStatusShouldThrow()
        {
            await InitStatus(VolunteerTaskStatus.Rejected);
            var command = CreateCommand(VolunteerTaskStatus.Completed, "User completed task");
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }

        [Fact]
        public async Task VolunteerCompletesTaskFromCompletedStatusShouldThrow()
        {
            await InitStatus(VolunteerTaskStatus.Completed);
            var command = CreateCommand(VolunteerTaskStatus.Completed, "User completed task");
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }

        [Fact]
        public async Task VolunteerCompletesTaskFromCanNotCompleteStatusShouldThrow()
        {
            await InitStatus(VolunteerTaskStatus.CanNotComplete);
            var command = CreateCommand(VolunteerTaskStatus.Completed, "User completed task");
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }

        [Fact]
        public async Task VolunteerCannotCompleteTaskFromAcceptedStatus()
        {
            var taskSignup = Context.TaskSignups.First();
            taskSignup.Status = VolunteerTaskStatus.Accepted;
            await Context.SaveChangesAsync();

            var @task = Context.Tasks.First();
            var user = Context.Users.First();
            var command = new ChangeTaskStatusCommand
            {
                TaskId = @task.Id,
                UserId = user.Id,
                TaskStatus = VolunteerTaskStatus.CanNotComplete,
                TaskStatusDescription = $"{user.UserName} cannot complete {@task.Name}"
            };
            await commandHandler.Handle(command);

            taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.VolunteerTask.Id.ShouldBe(command.TaskId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerCannotCompleteTaskFromAssignedStatus()
        {
            var user = Context.Users.First();
            var @task = Context.Tasks.First();
            var command = new ChangeTaskStatusCommand
            {
                TaskId = @task.Id,
                UserId = user.Id,
                TaskStatus = VolunteerTaskStatus.CanNotComplete,
                TaskStatusDescription = $"{user.UserName} cannot complete task {@task.Name}"
            };

            await commandHandler.Handle(command);

            var taskSignup = Context.TaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<TaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == taskSignup.Id)), Times.Once());
            taskSignup.Status.ShouldBe(command.TaskStatus);
            taskSignup.User.Id.ShouldBe(command.UserId);
            taskSignup.VolunteerTask.Id.ShouldBe(command.TaskId);
            taskSignup.StatusDescription.ShouldBe(command.TaskStatusDescription);
            taskSignup.StatusDateTimeUtc.ShouldBe(dateTimeUtcNow);
        }

        [Fact]
        public async Task VolunteerCannotCompleteTaskFromRejectedStatusShouldThrow()
        {
            await InitStatus(VolunteerTaskStatus.Rejected);
            var command = CreateCommand(VolunteerTaskStatus.CanNotComplete, "User cannot complete task");
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }

        [Fact]
        public async Task VolunteerCannotCompleteTaskFromCompletedStatusShouldThrow()
        {
            await InitStatus(VolunteerTaskStatus.Completed);
            var command = CreateCommand(VolunteerTaskStatus.CanNotComplete, "User cannot complete task");
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }

        [Fact]
        public async Task VolunteerCannotCompleteTaskFromCanNotCompleteStatusShouldThrow()
        {
            await InitStatus(VolunteerTaskStatus.CanNotComplete);
            var command = CreateCommand(VolunteerTaskStatus.CanNotComplete, "User cannot complete task");
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }

        [Fact]
        public void ShouldThrowIfTaskDoesNotExist()
        {
            var command = CreateCommand(VolunteerTaskStatus.Accepted, "User accepted task");
            command.TaskId = 90124;
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }

        [Fact]
        public void ShouldThrowIfTaskSignupDoesNotExist()
        {
            var command = CreateCommand(VolunteerTaskStatus.Completed, "User completed task");
            command.UserId = Guid.NewGuid().ToString();
            Should.Throw<Exception>(() => commandHandler.Handle(command));
        }
    }
}