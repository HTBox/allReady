﻿using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Notifications
{
    public class IntineraryVolunteerListUpdatedHandlerShould : InMemoryContextTest
    {
        private ApplicationUser _user;
        private TaskSignup _taskSignup;
        private Itinerary _itinerary;
        private TaskSignup _taskSignupNoContactPreferences;
        private DateTime _itineraryDate;
        private TaskSignup _taskSignupNoContacts;

        protected override void LoadTestData()
        {
            _user = new ApplicationUser
            {
                PhoneNumber = "123",
                Email = "johndoe@example.com"
            };

            _taskSignup = new TaskSignup
            {
                Id = 1,
                User = _user
            };

            _taskSignupNoContactPreferences = new TaskSignup
            {
                Id = 2,
                User = _user
            };

            _taskSignupNoContacts = new TaskSignup
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
            Context.TaskSignups.Add(_taskSignup);
            Context.TaskSignups.Add(_taskSignupNoContactPreferences);
            Context.TaskSignups.Add(_taskSignupNoContacts);
            Context.Itineraries.Add(_itinerary);
            Context.SaveChanges();
        }

        [Fact]
        public async Task DispatchNotificationToTheEmailAddressAndPhoneNumber()
        {
            var mockMediator = new Mock<IMediator>();
            var mockGeneralSettings = new Mock<IOptions<GeneralSettings>>();
            mockGeneralSettings.SetupGet(m => m.Value).Returns(new GeneralSettings { SiteBaseUrl = string.Empty });

            var handler = new IntineraryVolunteerListUpdatedHandler(Context, mockMediator.Object, mockGeneralSettings.Object);
            await handler.Handle(new IntineraryVolunteerListUpdated { TaskSignupId = 1, ItineraryId = 1 });

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

            var handler = new IntineraryVolunteerListUpdatedHandler(Context, mockMediator.Object, mockGeneralSettings.Object);
            await handler.Handle(new IntineraryVolunteerListUpdated { TaskSignupId = 1, ItineraryId = 1, UpdateType = UpdateType.VolunteerAssigned });

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

            var handler = new IntineraryVolunteerListUpdatedHandler(Context, mockMediator.Object, mockGeneralSettings.Object);
            await handler.Handle(new IntineraryVolunteerListUpdated { TaskSignupId = 1, ItineraryId = 1, UpdateType = UpdateType.VolnteerUnassigned });

            mockMediator.Verify(m => m.SendAsync(It.Is<NotifyVolunteersCommand>(command =>
                string.Equals(command.ViewModel.SmsMessage, $"You’ve been unassigned from a team for {_itineraryDate} http://localhost/v") &&
                string.Equals(command.ViewModel.Subject, $"You've been unassigned from a team for {_itineraryDate}") &&
                string.Equals(command.ViewModel.HtmlMessage, $"The volunteer organizer has unassigned you from a team for {_itineraryDate}. See your http://localhost/v for more information.")
            )), Times.Once);
        }
    }
}