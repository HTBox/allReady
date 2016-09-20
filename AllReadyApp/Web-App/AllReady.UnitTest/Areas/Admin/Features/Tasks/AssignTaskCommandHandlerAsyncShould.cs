using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Moq;
using Xunit;
using System.Linq;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class AssignTaskCommandHandlerAsyncShould : InMemoryContextTest
    {
        private readonly AssignTaskCommandAsync message;
        private readonly Mock<IMediator> mediator;
        private readonly AssignTaskCommandHandlerAsync sut;
        private readonly AllReadyTask task;
        private readonly ApplicationUser newVolunteer;
        private ApplicationUser existingVolunteer;

        public AssignTaskCommandHandlerAsyncShould()
        {
            task = new AllReadyTask
            {
                Id = 1,
                AssignedVolunteers = new List<TaskSignup> { new TaskSignup { User = new ApplicationUser { Id = "user3@abc.com" } } }
            };

            newVolunteer = new ApplicationUser {Id = "user1@abc.com" };
            existingVolunteer = new ApplicationUser {Id = "user2@abc.com"};
            Context.Add(newVolunteer);
            Context.Add(existingVolunteer);
            Context.Add(task);

            message = new AssignTaskCommandAsync
            {
                TaskId = task.Id,
                UserIds = new List<string> { newVolunteer.Id, existingVolunteer.Id }
            };

            mediator = new Mock<IMediator>();
            Context.SaveChanges();

            sut = new AssignTaskCommandHandlerAsync(Context, mediator.Object);
        }

        [Fact]
        public async Task SignsupVolunteersToTask()
        {
            await sut.Handle(message);

            var assignedVolunteers = Context.Tasks.Single(x => x.Id == task.Id).AssignedVolunteers.Select(x => x.User.Id);
            Assert.Equal(assignedVolunteers, new[] { newVolunteer.Id, existingVolunteer.Id});
        }

        [Fact]
        public async Task RemoveUsersThatAreNotInTheCurrentSignUpList()
        {
            await sut.Handle(message);
        }

        [Fact]
        public async Task NotifyNewVolunteers()
        {
            const string expectedMessage = "You've been assigned a task from AllReady.";

            await sut.Handle(message);

            mediator.Verify(b => b.SendAsync(It.Is<NotifyVolunteersCommand>(notifyCommand =>
                   notifyCommand.ViewModel.EmailMessage == expectedMessage &&
                   notifyCommand.ViewModel.Subject == expectedMessage &&
                   notifyCommand.ViewModel.EmailRecipients.Contains("user1@abc.com")
            )), Times.Once());
        }

        [Fact]
        public async Task DoesNotNotifyVolunteersPreviouslySignedUp()
        {
            await sut.Handle(message);

            mediator.Verify(b => b.SendAsync(It.Is<NotifyVolunteersCommand>(notifyCommand =>
                   notifyCommand.ViewModel.EmailRecipients.Contains("user2@abc.com")
            )), Times.Never);
        }
    }
}