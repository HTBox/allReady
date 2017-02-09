using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Notifications
{
    public class NotifyAssignedVolunteersForTheTaskHandlerShould : InMemoryContextTest
    {
        private readonly Mock<IMediator> mediator;
        private readonly NotifyAssignedVolunteersForTheVolunteerTaskHandler sut;

        public NotifyAssignedVolunteersForTheTaskHandlerShould()
        {
            mediator = new Mock<IMediator>();
            sut = new NotifyAssignedVolunteersForTheVolunteerTaskHandler(Context, mediator.Object);
        }

        [Fact]
        public async Task SendNotificationToVolunteersWithCorrectMessage()
        {
            const string expectedMessage = "You've been assigned a task from AllReady.";
            var volunteerTask = new VolunteerTask { Id = 1 };
            var volunteer = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@abc.com",
                PhoneNumber = "1234",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            Context.Add(volunteer);
            Context.Add(volunteerTask);
            Context.SaveChanges();

            var message = new VolunteerTaskAssignedToVolunteersNotification { VolunteerTaskId = volunteerTask.Id, NewlyAssignedVolunteers = new List<string> { volunteer.Id } };
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
            var volunteerTask = new VolunteerTask { Id = 1 };
            var volunteer = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@abc.com",
                PhoneNumber = "1234",
                PhoneNumberConfirmed = false
            };

            Context.Add(volunteer);
            Context.Add(volunteerTask);
            Context.SaveChanges();

            var message = new VolunteerTaskAssignedToVolunteersNotification { VolunteerTaskId = volunteerTask.Id, NewlyAssignedVolunteers = new List<string> { volunteer.Id } };
            await sut.Handle(message);

            mediator.Verify(b => b.SendAsync(It.Is<NotifyVolunteersCommand>(notifyCommand =>
                   !notifyCommand.ViewModel.SmsRecipients.Contains(volunteer.PhoneNumber)
            )), Times.Once());
        }

        [Fact]
        public async Task DoesNotSendNotificationToUsersWhoHasntVerifiedEmail()
        {
            var volunteerTask = new VolunteerTask { Id = 1 };
            var volunteer = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@abc.com",
                PhoneNumber = "1234",
                EmailConfirmed = false
            };

            Context.Add(volunteer);
            Context.Add(volunteerTask);
            Context.SaveChanges();

            var message = new VolunteerTaskAssignedToVolunteersNotification { VolunteerTaskId = volunteerTask.Id, NewlyAssignedVolunteers = new List<string> { volunteer.Id } };
            await sut.Handle(message);

            mediator.Verify(b => b.SendAsync(It.Is<NotifyVolunteersCommand>(notifyCommand =>
                   !notifyCommand.ViewModel.EmailRecipients.Contains(volunteer.Email)
            )), Times.Once());
        }
    }
}