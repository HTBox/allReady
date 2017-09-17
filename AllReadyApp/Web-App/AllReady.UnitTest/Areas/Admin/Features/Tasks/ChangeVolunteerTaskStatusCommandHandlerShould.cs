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
    public class ChangeVolunteerTaskStatusCommandHandlerShould : InMemoryContextTest
    {
        private readonly Mock<IMediator> mediator;
        private readonly ChangeVolunteerTaskStatusCommandHandler commandHandler;
        private readonly DateTime dateTimeUtcNow = DateTime.UtcNow;

        public ChangeVolunteerTaskStatusCommandHandlerShould()
        {
            mediator = new Mock<IMediator>();
            commandHandler = new ChangeVolunteerTaskStatusCommandHandler(Context, mediator.Object) { DateTimeUtcNow = () => dateTimeUtcNow };
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

            Context.VolunteerTasks.Add(newTask);

            Context.SaveChanges();
        }

        protected async Task InitStatus(VolunteerTaskStatus status)
        {
            var volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            volunteerTaskSignup.Status = status;
            await Context.SaveChangesAsync();
        }

        protected ChangeVolunteerTaskStatusCommand CreateCommand(VolunteerTaskStatus status, string description = "")
        {
            var user = Context.Users.First();
            var volunteerTask = Context.VolunteerTasks.First();

            return new ChangeVolunteerTaskStatusCommand
            {
                UserId = user.Id,
                VolunteerTaskId = volunteerTask.Id,
                VolunteerTaskStatus = status,
                VolunteerTaskStatusDescription = description ?? string.Empty
            };
        }

        [Fact]
        public async Task VolunteerAssignedTask()
        {
            var volunteerTask = Context.VolunteerTasks.First();
            var user = Context.Users.First();
            var command = new ChangeVolunteerTaskStatusCommand
            {
                VolunteerTaskId = volunteerTask.Id,
                UserId = user.Id,
                VolunteerTaskStatus = VolunteerTaskStatus.Assigned,
                VolunteerTaskStatusDescription = $"Assign {volunteerTask.Name} to {user.UserName}"
            };
            await commandHandler.Handle(command);

            var volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<VolunteerTaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == volunteerTaskSignup.Id)), Times.Once());
            volunteerTaskSignup.Status.ShouldBe(command.VolunteerTaskStatus);
            volunteerTaskSignup.VolunteerTask.Id.ShouldBe(command.VolunteerTaskId);
            volunteerTaskSignup.User.Id.ShouldBe(command.UserId);
            volunteerTaskSignup.StatusDescription.ShouldBe(command.VolunteerTaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerAcceptsTaskFromAssignedStatus()
        {
            var volunteerTask = Context.VolunteerTasks.First();
            var user = Context.Users.First();
            var command = new ChangeVolunteerTaskStatusCommand
            {
                VolunteerTaskId = volunteerTask.Id,
                UserId = user.Id,
                VolunteerTaskStatus = VolunteerTaskStatus.Accepted,
                VolunteerTaskStatusDescription = $"{user.UserName} has accepted the task {volunteerTask.Name}"
            };
            await commandHandler.Handle(command);

            var volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<VolunteerTaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == volunteerTaskSignup.Id)), Times.Once());
            volunteerTaskSignup.Status.ShouldBe(command.VolunteerTaskStatus);
            volunteerTaskSignup.VolunteerTask.Id.ShouldBe(command.VolunteerTaskId);
            volunteerTaskSignup.User.Id.ShouldBe(command.UserId);
            volunteerTaskSignup.StatusDescription.ShouldBe(command.VolunteerTaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerAcceptsTaskFromCanNotCompleteStatus()
        {
            var volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            volunteerTaskSignup.Status = VolunteerTaskStatus.CanNotComplete;
            await Context.SaveChangesAsync();

            var volunteerTask = Context.VolunteerTasks.First();
            var user = Context.Users.First();
            var command = new ChangeVolunteerTaskStatusCommand
            {
                VolunteerTaskId = volunteerTask.Id,
                UserId = user.Id,
                VolunteerTaskStatus = VolunteerTaskStatus.Accepted,
                VolunteerTaskStatusDescription = $"{user.UserName} accepted task {volunteerTask.Name}"
            };

            await commandHandler.Handle(command);

            volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<VolunteerTaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == volunteerTaskSignup.Id)), Times.Once());
            volunteerTaskSignup.Status.ShouldBe(command.VolunteerTaskStatus);
            volunteerTaskSignup.VolunteerTask.Id.ShouldBe(command.VolunteerTaskId);
            volunteerTaskSignup.User.Id.ShouldBe(command.UserId);
            volunteerTaskSignup.StatusDescription.ShouldBe(command.VolunteerTaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerAcceptsTaskFromCompletedStatus()
        {
            var volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            volunteerTaskSignup.Status = VolunteerTaskStatus.Completed;
            await Context.SaveChangesAsync();

            var volunteerTask = Context.VolunteerTasks.First();
            var user = Context.Users.First();
            var command = new ChangeVolunteerTaskStatusCommand
            {
                VolunteerTaskId = volunteerTask.Id,
                UserId = user.Id,
                VolunteerTaskStatus = VolunteerTaskStatus.Accepted,
                VolunteerTaskStatusDescription = $"{user.UserName} accepted task {volunteerTask.Name}"
            };

            await commandHandler.Handle(command);

            volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<VolunteerTaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == volunteerTaskSignup.Id)), Times.Once());
            volunteerTaskSignup.Status.ShouldBe(command.VolunteerTaskStatus);
            volunteerTaskSignup.User.Id.ShouldBe(command.UserId);
            volunteerTaskSignup.VolunteerTask.Id.ShouldBe(command.VolunteerTaskId);
            volunteerTaskSignup.StatusDescription.ShouldBe(command.VolunteerTaskStatusDescription);
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
            var volunteerTask = Context.VolunteerTasks.First();
            var user = Context.Users.First();
            var command = new ChangeVolunteerTaskStatusCommand
            {
                VolunteerTaskId = volunteerTask.Id,
                UserId = user.Id,
                VolunteerTaskStatus = VolunteerTaskStatus.Rejected,
                VolunteerTaskStatusDescription = $"{user.UserName} rejected task {volunteerTask.Name}"
            };
            await commandHandler.Handle(command);

            var volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<VolunteerTaskSignupStatusChanged>(notifiyCommand => notifiyCommand.SignupId == volunteerTaskSignup.Id)), Times.Once());
            volunteerTaskSignup.Status.ShouldBe(command.VolunteerTaskStatus);
            volunteerTaskSignup.VolunteerTask.Id.ShouldBe(command.VolunteerTaskId);
            volunteerTaskSignup.User.Id.ShouldBe(command.UserId);
            volunteerTaskSignup.StatusDescription.ShouldBe(command.VolunteerTaskStatusDescription);
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
            var volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            volunteerTaskSignup.Status = VolunteerTaskStatus.Accepted;
            await Context.SaveChangesAsync();

            var volunteerTask = Context.VolunteerTasks.First();
            var user = Context.Users.First();
            var command = new ChangeVolunteerTaskStatusCommand
            {
                VolunteerTaskId = volunteerTask.Id,
                UserId = user.Id,
                VolunteerTaskStatus = VolunteerTaskStatus.Completed,
                VolunteerTaskStatusDescription = $"{user.UserName} completed {volunteerTask.Name}"
            };
            await commandHandler.Handle(command);

            volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<VolunteerTaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == volunteerTaskSignup.Id)), Times.Once());
            volunteerTaskSignup.Status.ShouldBe(command.VolunteerTaskStatus);
            volunteerTaskSignup.VolunteerTask.Id.ShouldBe(command.VolunteerTaskId);
            volunteerTaskSignup.User.Id.ShouldBe(command.UserId);
            volunteerTaskSignup.StatusDescription.ShouldBe(command.VolunteerTaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerCompletesTaskFromAssignedStatus()
        {
            var volunteerTask = Context.VolunteerTasks.First();
            var user = Context.Users.First();
            var command = new ChangeVolunteerTaskStatusCommand
            {
                VolunteerTaskId = volunteerTask.Id,
                UserId = user.Id,
                VolunteerTaskStatus = VolunteerTaskStatus.Completed,
                VolunteerTaskStatusDescription = $"{user.UserName} completed task {volunteerTask.Name}"
            };

            await commandHandler.Handle(command);

            var volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<VolunteerTaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == volunteerTaskSignup.Id)), Times.Once());
            volunteerTaskSignup.Status.ShouldBe(command.VolunteerTaskStatus);
            volunteerTaskSignup.VolunteerTask.Id.ShouldBe(command.VolunteerTaskId);
            volunteerTaskSignup.User.Id.ShouldBe(command.UserId);
            volunteerTaskSignup.StatusDescription.ShouldBe(command.VolunteerTaskStatusDescription);
            volunteerTaskSignup.StatusDateTimeUtc.ShouldBe(dateTimeUtcNow);
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
            var volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            volunteerTaskSignup.Status = VolunteerTaskStatus.Accepted;
            await Context.SaveChangesAsync();

            var volunteerTask = Context.VolunteerTasks.First();
            var user = Context.Users.First();
            var command = new ChangeVolunteerTaskStatusCommand
            {
                VolunteerTaskId = volunteerTask.Id,
                UserId = user.Id,
                VolunteerTaskStatus = VolunteerTaskStatus.CanNotComplete,
                VolunteerTaskStatusDescription = $"{user.UserName} cannot complete {volunteerTask.Name}"
            };
            await commandHandler.Handle(command);

            volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<VolunteerTaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == volunteerTaskSignup.Id)), Times.Once());
            volunteerTaskSignup.Status.ShouldBe(command.VolunteerTaskStatus);
            volunteerTaskSignup.User.Id.ShouldBe(command.UserId);
            volunteerTaskSignup.VolunteerTask.Id.ShouldBe(command.VolunteerTaskId);
            volunteerTaskSignup.StatusDescription.ShouldBe(command.VolunteerTaskStatusDescription);
        }

        [Fact]
        public async Task VolunteerCannotCompleteTaskFromAssignedStatus()
        {
            var user = Context.Users.First();
            var volunteerTask = Context.VolunteerTasks.First();
            var command = new ChangeVolunteerTaskStatusCommand
            {
                VolunteerTaskId = volunteerTask.Id,
                UserId = user.Id,
                VolunteerTaskStatus = VolunteerTaskStatus.CanNotComplete,
                VolunteerTaskStatusDescription = $"{user.UserName} cannot complete task {volunteerTask.Name}"
            };

            await commandHandler.Handle(command);

            var volunteerTaskSignup = Context.VolunteerTaskSignups.First();
            mediator.Verify(b => b.PublishAsync(It.Is<VolunteerTaskSignupStatusChanged>(notifyCommand => notifyCommand.SignupId == volunteerTaskSignup.Id)), Times.Once());
            volunteerTaskSignup.Status.ShouldBe(command.VolunteerTaskStatus);
            volunteerTaskSignup.User.Id.ShouldBe(command.UserId);
            volunteerTaskSignup.VolunteerTask.Id.ShouldBe(command.VolunteerTaskId);
            volunteerTaskSignup.StatusDescription.ShouldBe(command.VolunteerTaskStatusDescription);
            volunteerTaskSignup.StatusDateTimeUtc.ShouldBe(dateTimeUtcNow);
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
            command.VolunteerTaskId = 90124;
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