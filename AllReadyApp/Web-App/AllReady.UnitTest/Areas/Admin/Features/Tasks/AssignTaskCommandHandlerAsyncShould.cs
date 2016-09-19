using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Tasks
{
    public class AssignTaskCommandHandlerAsyncShould : InMemoryContextTest
    {
        //Seed Tasks to the context
        //Seed Users to the context
        //Should we check for whether volunteer already signed up? Whats the downside if we don't? How would a volunteer get signedup again for the same task? 
        // Checks whetehr that particular user has already signed up. 
        private readonly AssignTaskCommandAsync message;
        private readonly Mock<IMediator> mediator;
        private readonly AssignTaskCommandHandlerAsync sut;

        public AssignTaskCommandHandlerAsyncShould()
        {
            var task = new AllReadyTask
            {
                Id = 1
            };

            Context.Add(task);
            var users = new List<string> {"user1@abc.com", "user2@cd.com"};
            message = new AssignTaskCommandAsync { TaskId = task.Id, UserIds = users };
            mediator = new Mock<IMediator>();
            Context.SaveChanges();

            sut = new AssignTaskCommandHandlerAsync(Context, mediator.Object);
        }

        [Fact]
        public async Task AssignVolunteersToTask()
        {
            await sut.Handle(message);
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
        public async Task DoesNotSendsNotificationToNewVolunteers()
        {
            const string expectedMessage = "You've been assigned a task from AllReady.";

            await sut.Handle(message);

            mediator.Verify(b => b.SendAsync(It.Is<NotifyVolunteersCommand>(notifyCommand =>
                   notifyCommand.ViewModel.EmailMessage == expectedMessage &&
                   notifyCommand.ViewModel.Subject == expectedMessage &&
                   notifyCommand.ViewModel.EmailRecipients.Contains("user1@abc.com")
            )), Times.Once());
        }
    }
}