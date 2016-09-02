using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Notifications
{
    public class NotifyVolunteerForUserUnenrollsShould
    {
        [Fact]
        public async Task SendEventDetailForNotificationQueryAsyncWithCorrectParameters()
        {
            var notification = new UserUnenrolls
            {
                UserId = "user1@example.com",
                EventId = 111
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(new EventDetailForNotificationModel());

            var handler = new NotifyVolunteerForUserUnenrolls(mockMediator.Object, null);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.Is<EventDetailForNotificationQueryAsync>(n => n.EventId == notification.EventId && n.UserId == notification.UserId)), Times.Once);
        }

        [Fact]
        public async Task NotSendNotifyVolunteersCommand_WhenEventDetailForNotificationModelIsNull()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(null);

            var handler = new NotifyVolunteerForUserUnenrolls(mockMediator.Object, null);
            await handler.Handle(new UserUnenrolls());

            mockMediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        [Fact]
        public async Task NotSendNotifyVolunteersCommand_WhenSignupIsNull()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(new EventDetailForNotificationModel());

            var handler = new NotifyVolunteerForUserUnenrolls(mockMediator.Object, null);
            await handler.Handle(new UserUnenrolls());

            mockMediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        [Fact]
        public async Task NotSendNotifyVolunteersCommand_WhenSignupNotFound()
        {
            var notification = new UserUnenrolls { UserId = "user1@example.com" };

            var model = new EventDetailForNotificationModel
            {
                UsersSignedUp = new List<EventSignup> { new EventSignup { User = new ApplicationUser { Id = "differentuser1@example.com" } } }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var handler = new NotifyVolunteerForUserUnenrolls(mockMediator.Object, null);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }
        
        [Fact]
        public async Task NotSendNotifyVolunteersCommand_WhenRecepientEmailIsNull()
        {
            var userId = "user1@example.com";

            var notification = new UserUnenrolls { UserId = userId };

            var model = new EventDetailForNotificationModel
            {
                UsersSignedUp = new List<EventSignup> { new EventSignup { User = new ApplicationUser { Id = userId } } }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var handler = new NotifyVolunteerForUserUnenrolls(mockMediator.Object, null);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        [Fact]
        public async Task NotSendNotifyVolunteersCommand_WhenRecepientEmailIsEmpty()
        {
            var userId = "user1@example.com";

            var notification = new UserUnenrolls { UserId = userId };

            var model = new EventDetailForNotificationModel
            {
                UsersSignedUp = new List<EventSignup> { new EventSignup { User = new ApplicationUser { Id = userId, Email = string.Empty } } }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var handler = new NotifyVolunteerForUserUnenrolls(mockMediator.Object, null);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendNotifyVolunteersCommand_ToPreferredEmail()
        {
            var userId = "user1@example.com";
            var preferredEmail = "preferredemail1@example.com";

            var notification = new UserUnenrolls { UserId = userId };

            var model = new EventDetailForNotificationModel
            {
                UsersSignedUp = new List<EventSignup> { new EventSignup { PreferredEmail = preferredEmail, User = new ApplicationUser { Id = userId } } }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var options = new Mock<IOptions<GeneralSettings>>();
            options.Setup(o => o.Value).Returns(new GeneralSettings());

            var handler = new NotifyVolunteerForUserUnenrolls(mockMediator.Object, options.Object);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(c => 
                c.ViewModel.EmailRecipients != null
                && c.ViewModel.EmailRecipients.Count == 1
                && c.ViewModel.EmailRecipients.First() == preferredEmail
            )), Times.Once);
        }

        [Fact]
        public async Task SendNotifyVolunteersCommand_ToUserEmail()
        {
            var userId = "user1@example.com";
            var userEmail = "useremail1@example.com";
            
            var notification = new UserUnenrolls { UserId = userId };

            var model = new EventDetailForNotificationModel
            {
                UsersSignedUp = new List<EventSignup> { new EventSignup { User = new ApplicationUser { Id = userId, Email = userEmail } } }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var options = new Mock<IOptions<GeneralSettings>>();
            options.Setup(o => o.Value).Returns(new GeneralSettings());

            var handler = new NotifyVolunteerForUserUnenrolls(mockMediator.Object, options.Object);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(c =>
                c.ViewModel.EmailRecipients != null
                && c.ViewModel.EmailRecipients.Count == 1
                && c.ViewModel.EmailRecipients.First() == userEmail
            )), Times.Once);
        }
        
        [Fact]
        public async Task SendNotifyVolunteersCommand_WithCorrectMessage()
        {
            var userId = "user1@example.com";

            var siteBaseUrl = $"http://www.htbox.org";
            var eventId = 111;
            var campaignName = "Compaign1";
            var eventName = "Event1";

            var eventLink = $"View event: {siteBaseUrl}Admin/Event/Details/{eventId}";

            var message = new StringBuilder();
            message.AppendLine("This is to confirm that you have elected to un-enroll from the following event:");
            message.AppendLine();
            message.AppendLine($"   Campaign: {campaignName}");
            message.AppendLine($"   Event: {eventName} ({eventLink})");
            message.AppendLine();
            message.AppendLine("Thanks for letting us know that you will not be participating.");

            var notification = new UserUnenrolls { UserId = userId };

            var model = new EventDetailForNotificationModel
            {
                EventId = eventId,
                EventName = eventName,
                CampaignName = campaignName,
                UsersSignedUp = new List<EventSignup> { new EventSignup { User = new ApplicationUser { Id = userId, Email = userId } } }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var options = new Mock<IOptions<GeneralSettings>>();
            options.Setup(o => o.Value).Returns(new GeneralSettings { SiteBaseUrl = siteBaseUrl });

            var handler = new NotifyVolunteerForUserUnenrolls(mockMediator.Object, options.Object);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(c =>
                !string.IsNullOrEmpty(c.ViewModel.EmailMessage)
                && c.ViewModel.EmailMessage == message.ToString()
                && !string.IsNullOrEmpty(c.ViewModel.HtmlMessage)
                && c.ViewModel.HtmlMessage == message.ToString()
            )), Times.Once);
        }

        [Fact]
        public async Task SendNotifyVolunteersCommand_WithCorrectSubject()
        {
            var userId = "user1@example.com";
            var subject = "allReady Event Un-enrollment Confirmation";

            var notification = new UserUnenrolls { UserId = userId };

            var model = new EventDetailForNotificationModel
            {
                UsersSignedUp = new List<EventSignup> { new EventSignup { User = new ApplicationUser { Id = userId, Email = userId } } }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var options = new Mock<IOptions<GeneralSettings>>();
            options.Setup(o => o.Value).Returns(new GeneralSettings());

            var handler = new NotifyVolunteerForUserUnenrolls(mockMediator.Object, options.Object);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(c =>
                c.ViewModel.Subject == subject
            )), Times.Once);
        }
    }
}
