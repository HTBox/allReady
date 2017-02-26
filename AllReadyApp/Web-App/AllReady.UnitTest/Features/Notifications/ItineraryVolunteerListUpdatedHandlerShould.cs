using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using AllReady.Configuration;
using Xunit;

namespace AllReady.UnitTest.Features.Notifications
{
    public class ItineraryVolunteerListUpdatedHandlerShould : InMemoryContextTest
    {
        private ApplicationUser _user;
        private VolunteerTaskSignup _taskSignup;
        private Itinerary _itinerary;
        private VolunteerTaskSignup _taskSignupNoContactPreferences;
        private DateTime _itineraryDate;
        private VolunteerTaskSignup _taskSignupNoContacts;

        protected override void LoadTestData()
        {
            _user = new ApplicationUser
            {
                PhoneNumber = "123",
                Email = "johndoe@example.com"
            };

            _taskSignup = new VolunteerTaskSignup
            {
                Id = 1,
                User = _user
            };

            _taskSignupNoContactPreferences = new VolunteerTaskSignup
            {
                Id = 2,
                User = _user
            };

            _taskSignupNoContacts = new VolunteerTaskSignup
            {
                Id = 3,
                User = new ApplicationUser()
            };

            _itinerary = new Itinerary
            {
                Id = 1,
                Date = _itineraryDate = new DateTime(2016, 1, 1, 10, 30, 0)
            };

            Context.Users.Add(_user);
            Context.VolunteerTaskSignups.Add(_taskSignup);
            Context.VolunteerTaskSignups.Add(_taskSignupNoContactPreferences);
            Context.VolunteerTaskSignups.Add(_taskSignupNoContacts);
            Context.Itineraries.Add(_itinerary);
            Context.SaveChanges();
        }

        [Fact]
        public async Task DispatchNotificationToTheEmailAddressAndPhoneNumber()
        {
            var mockMediator = new Mock<IMediator>();
            var mockGeneralSettings = new Mock<IOptions<GeneralSettings>>();
            mockGeneralSettings.SetupGet(m => m.Value).Returns(new GeneralSettings { SiteBaseUrl = string.Empty });

            var handler = new ItineraryVolunteerListUpdatedHandler(Context, mockMediator.Object, mockGeneralSettings.Object);
            await handler.Handle(new ItineraryVolunteerListUpdated { VolunteerTaskSignupId = 1, ItineraryId = 1 });

            mockMediator.Verify(m => m.SendAsync(It.Is<NotifyVolunteersCommand>(command =>
                command.ViewModel.SmsRecipients.Count == 1 &&
                command.ViewModel.SmsRecipients[0] == _taskSignup.User.PhoneNumber &&
                command.ViewModel.EmailRecipients.Count == 1 &&
                command.ViewModel.EmailRecipients[0] == _taskSignup.User.Email
            )), Times.Once);
        }

        [Fact]
        public async Task DispatchVolunteerAssignedNotification()
        {
            var mockMediator = new Mock<IMediator>();
            var mockGeneralSettings = new Mock<IOptions<GeneralSettings>>();
            mockGeneralSettings.SetupGet(m => m.Value).Returns(new GeneralSettings { SiteBaseUrl = "http://localhost/" });

            var handler = new ItineraryVolunteerListUpdatedHandler(Context, mockMediator.Object, mockGeneralSettings.Object);
            await handler.Handle(new ItineraryVolunteerListUpdated { VolunteerTaskSignupId = 1, ItineraryId = 1, UpdateType = UpdateType.VolunteerAssigned });

            mockMediator.Verify(m => m.SendAsync(It.Is<NotifyVolunteersCommand>(command =>
                string.Equals(command.ViewModel.SmsMessage, $"You’ve been assigned to a team for {_itineraryDate} http://localhost/v") &&
                string.Equals(command.ViewModel.Subject, $"You've been assigned to a team for {_itineraryDate}") &&
                string.Equals(command.ViewModel.HtmlMessage, $"The volunteer organizer has assigned you to a team for {_itineraryDate}. See your http://localhost/v for more information.")
            )), Times.Once);
        }

        [Fact]
        public async Task DispatchVolunteerUnAssignedNotification()
        {
            var mockMediator = new Mock<IMediator>();
            var mockGeneralSettings = new Mock<IOptions<GeneralSettings>>();
            mockGeneralSettings.SetupGet(m => m.Value).Returns(new GeneralSettings { SiteBaseUrl = "http://localhost/" });

            var handler = new ItineraryVolunteerListUpdatedHandler(Context, mockMediator.Object, mockGeneralSettings.Object);
            await handler.Handle(new ItineraryVolunteerListUpdated { VolunteerTaskSignupId = 1, ItineraryId = 1, UpdateType = UpdateType.VolnteerUnassigned });

            mockMediator.Verify(m => m.SendAsync(It.Is<NotifyVolunteersCommand>(command =>
                string.Equals(command.ViewModel.SmsMessage, $"You’ve been unassigned from a team for {_itineraryDate} http://localhost/v") &&
                string.Equals(command.ViewModel.Subject, $"You've been unassigned from a team for {_itineraryDate}") &&
                string.Equals(command.ViewModel.HtmlMessage, $"The volunteer organizer has unassigned you from a team for {_itineraryDate}. See your http://localhost/v for more information.")
            )), Times.Once);
        }
    }
}