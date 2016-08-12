using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Notifications
{
	public class IntineraryVolunteerListUpdatedHandlerTests : InMemoryContextTest
	{
		private ApplicationUser _user;
		private TaskSignup _taskSignup;
		private Itinerary _itinerary;
		private TaskSignup _taskSignupNoPreferences;

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
				PreferredEmail = "preferred@email.com",
				PreferredPhoneNumber = "456",
				User = _user
			};

			_taskSignupNoPreferences = new TaskSignup
			{
				Id = 2,
				User = _user
			};

			_itinerary = new Itinerary
			{
				Id = 1,
			};

			Context.Users.Add(_user);
			Context.TaskSignups.Add(_taskSignup);
			Context.TaskSignups.Add(_taskSignupNoPreferences);
			Context.Itineraries.Add(_itinerary);
			Context.SaveChanges();
		}

		[Fact]
		public async Task ShouldDispatchNotificationToThePreferredEmailAddressAndPhoneNumber()
		{
			var mockMediator = new Mock<IMediator>();
			var generalSettings = new Mock<IOptions<GeneralSettings>>();
			generalSettings.SetupGet(m => m.Value).Returns(new GeneralSettings { SiteBaseUrl = string.Empty });
			var handler = new IntineraryVolunteerListUpdatedHandler(Context, mockMediator.Object, generalSettings.Object);

			await handler.Handle(new IntineraryVolunteerListUpdated { TaskSignupId = 1, ItineraryId = 1 });

			mockMediator.Verify(m => m.SendAsync(It.Is<NotifyVolunteersCommand>((command) =>
				command.ViewModel.SmsRecipients.Count == 1 && 
				command.ViewModel.SmsRecipients[0] == _taskSignup.PreferredPhoneNumber &&
				command.ViewModel.EmailRecipients.Count == 1 &&
				command.ViewModel.EmailRecipients[0] == _taskSignup.PreferredEmail
			)), Times.Once);
		}

		[Fact]
		public async Task ShouldDispatchNotificationToTheUserEmailAddressAndPhoneNumberAsFallback()
		{
			var mockMediator = new Mock<IMediator>();
			var generalSettings = new Mock<IOptions<GeneralSettings>>();
			generalSettings.SetupGet(m => m.Value).Returns(new GeneralSettings { SiteBaseUrl = string.Empty });
			var handler = new IntineraryVolunteerListUpdatedHandler(Context, mockMediator.Object, generalSettings.Object);

			await handler.Handle(new IntineraryVolunteerListUpdated { TaskSignupId = 2, ItineraryId = 1 });

			mockMediator.Verify(m => m.SendAsync(It.Is<NotifyVolunteersCommand>((command) =>
				command.ViewModel.SmsRecipients.Count == 1 &&
				command.ViewModel.SmsRecipients[0] == _user.PhoneNumber &&
				command.ViewModel.EmailRecipients.Count == 1 &&
				command.ViewModel.EmailRecipients[0] == _user.Email
			)), Times.Once);
		}
		
		[Fact]
		public async Task ShouldDispatchVolunteerAssignedNotification()
		{
			var mockMediator = new Mock<IMediator>();
			var generalSettings = new Mock<IOptions<GeneralSettings>>();
			generalSettings.SetupGet(m => m.Value).Returns(new GeneralSettings { SiteBaseUrl = string.Empty });
			var handler = new IntineraryVolunteerListUpdatedHandler(Context, mockMediator.Object, generalSettings.Object);

			await handler.Handle(new IntineraryVolunteerListUpdated { TaskSignupId = 1, ItineraryId = 1 ,UpdateType = UpdateType.VolunteerAssigned});

			mockMediator.Verify(m => m.SendAsync(It.Is<NotifyVolunteersCommand>((command) =>
				command.ViewModel.Subject.IndexOf("assigned",StringComparison.OrdinalIgnoreCase) != -1 &&
				command.ViewModel.SmsMessage.IndexOf("assigned", StringComparison.OrdinalIgnoreCase) != -1
			)), Times.Once);
		}

		[Fact]
		public async Task ShouldDispatchVolunteerUnAssignedNotification()
		{
			var mockMediator = new Mock<IMediator>();
			var generalSettings = new Mock<IOptions<GeneralSettings>>();
			generalSettings.SetupGet(m => m.Value).Returns(new GeneralSettings { SiteBaseUrl = string.Empty });
			var handler = new IntineraryVolunteerListUpdatedHandler(Context, mockMediator.Object, generalSettings.Object);

			await handler.Handle(new IntineraryVolunteerListUpdated { TaskSignupId = 1, ItineraryId = 1, UpdateType = UpdateType.VolnteerUnassigned });

			mockMediator.Verify(m => m.SendAsync(It.Is<NotifyVolunteersCommand>((command) =>
				command.ViewModel.Subject.IndexOf("unassigned", StringComparison.OrdinalIgnoreCase) != -1 &&
				command.ViewModel.SmsMessage.IndexOf("unassigned", StringComparison.OrdinalIgnoreCase) != -1
			)), Times.Once);
		}
	}
}
