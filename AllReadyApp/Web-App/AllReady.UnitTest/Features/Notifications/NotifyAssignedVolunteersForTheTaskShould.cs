using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Notifications
{
    public class NotifyAssignedVolunteersForTheTaskShould : InMemoryContextTest
    {
        private readonly Mock<IMediator> mediator;
        private readonly NotifyAssignedVolunteersForTheTask sut;

        public NotifyAssignedVolunteersForTheTaskShould()
        {
            mediator = new Mock<IMediator>();
            sut = new NotifyAssignedVolunteersForTheTask(Context, mediator.Object);
        }

        [Fact]
        public async Task SendNotificationToVolunteersWithCorrectMessage()
        {
            const string expectedMessage = "You've been assigned a task from AllReady.";
            var theTask = new AllReadyTask { Id = 1 };
            var volunteer = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@abc.com",
                PhoneNumber = "1234",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            Context.Add(volunteer);
            Context.Add(theTask);
            Context.SaveChanges();

            var message = new TaskAssignedToVolunteersNotification { TaskId = theTask.Id, NewlyAssignedVolunteers = new List<string> { volunteer.Id } };
            await sut.Handle(message);

            mediator.Verify(b => b.SendAsync(It.Is<NotifyVolunteersCommand>(notifyCommand =>
                   notifyCommand.ViewModel.EmailMessage == expectedMessage &&
                   notifyCommand.ViewModel.Subject == expectedMessage &&
                   notifyCommand.ViewModel.EmailRecipients.Contains(volunteer.Email) &&
                   notifyCommand.ViewModel.SmsRecipients.Contains(volunteer.PhoneNumber) &&
                   notifyCommand.ViewModel.SmsMessage == expectedMessage
            )), Times.Once());
        }

        [Fact]
        public async Task DoesNotSendNotificationToUsersWhoHasntVerifiedPhoneNumber()
        {
            var theTask = new AllReadyTask { Id = 1 };
            var volunteer = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@abc.com",
                PhoneNumber = "1234",
                PhoneNumberConfirmed = false
            };

            Context.Add(volunteer);
            Context.Add(theTask);
            Context.SaveChanges();

            var message = new TaskAssignedToVolunteersNotification { TaskId = theTask.Id, NewlyAssignedVolunteers = new List<string> { volunteer.Id } };
            await sut.Handle(message);

            mediator.Verify(b => b.SendAsync(It.Is<NotifyVolunteersCommand>(notifyCommand =>
                   !notifyCommand.ViewModel.SmsRecipients.Contains(volunteer.PhoneNumber)
            )), Times.Once());
        }

        [Fact]
        public async Task DoesNotSendNotificationToUsersWhoHasntVerifiedEmail()
        {
            var theTask = new AllReadyTask { Id = 1 };
            var volunteer = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@abc.com",
                PhoneNumber = "1234",
                EmailConfirmed = false
            };

            Context.Add(volunteer);
            Context.Add(theTask);
            Context.SaveChanges();

            var message = new TaskAssignedToVolunteersNotification { TaskId = theTask.Id, NewlyAssignedVolunteers = new List<string> { volunteer.Id } };
            await sut.Handle(message);

            mediator.Verify(b => b.SendAsync(It.Is<NotifyVolunteersCommand>(notifyCommand =>
                   !notifyCommand.ViewModel.EmailRecipients.Contains(volunteer.Email)
            )), Times.Once());
        }
    }
}