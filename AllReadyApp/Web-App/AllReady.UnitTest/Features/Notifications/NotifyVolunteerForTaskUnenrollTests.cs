using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Notifications
{
    public class NotifyVolunteerForTaskUnenrollShould
    {
        [Fact]
        public async Task SendTaskDetailForNotificationQueryAsyncWithCorrectParameters()
        {
            var notification = new UserUnenrolls
            {
                UserId = "user1@example.com",
                TaskId = 111
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<TaskDetailForNotificationQueryAsync>())).ReturnsAsync(new TaskDetailForNotificationModel());

            var handler = new NotifyVolunteerForTaskUnenroll(mockMediator.Object, null);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.Is<TaskDetailForNotificationQueryAsync>(n => n.TaskId == notification.TaskId && n.UserId == notification.UserId)), Times.Once);
        }

        [Fact]
        public async Task NotSendNotifyVolunteersCommand_WhenTaskDetailForNotificationModelIsNull()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<TaskDetailForNotificationQueryAsync>())).ReturnsAsync(null);

            var handler = new NotifyVolunteerForTaskUnenroll(mockMediator.Object, null);
            await handler.Handle(new UserUnenrolls());

            mockMediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        [Fact]
        public async Task NotSendNotifyVolunteersCommand_WhenSignupIsNull()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<TaskDetailForNotificationQueryAsync>())).ReturnsAsync(new TaskDetailForNotificationModel());

            var handler = new NotifyVolunteerForTaskUnenroll(mockMediator.Object, null);
            await handler.Handle(new UserUnenrolls());

            mockMediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        [Fact]
        public async Task NotSendNotifyVolunteersCommand_WhenVolunteerIsNull()
        {
            var notification = new UserUnenrolls { UserId = "user1@example.com" };

            var model = new TaskDetailForNotificationModel
            {
               Volunteer = null
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<TaskDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var handler = new NotifyVolunteerForTaskUnenroll(mockMediator.Object, null);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }
        
        [Fact]
        public async Task NotSendNotifyVolunteersCommand_WhenRecepientEmailIsNull()
        {
            var userId = "user1@example.com";

            var notification = new UserUnenrolls { UserId = userId };

            var model = new TaskDetailForNotificationModel
            {
                Volunteer = new ApplicationUser { Email = null }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<TaskDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var handler = new NotifyVolunteerForTaskUnenroll(mockMediator.Object, null);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        [Fact]
        public async Task NotSendNotifyVolunteersCommand_WhenRecepientEmailIsEmpty()
        {
            var userId = "user1@example.com";

            var notification = new UserUnenrolls { UserId = userId };

            var model = new TaskDetailForNotificationModel
            {
                Volunteer = new ApplicationUser { Email = string.Empty }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<TaskDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var handler = new NotifyVolunteerForTaskUnenroll(mockMediator.Object, null);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendNotifyVolunteersCommand_ToUserEmail()
        {
            var userId = "user1@example.com";
            var userEmail = "useremail1@example.com";
            
            var notification = new UserUnenrolls { UserId = userId };

            var model = new TaskDetailForNotificationModel
            {
                Volunteer = new ApplicationUser { Id = userId, Email = userEmail }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<TaskDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var options = new Mock<IOptions<GeneralSettings>>();
            options.Setup(o => o.Value).Returns(new GeneralSettings());

            var handler = new NotifyVolunteerForTaskUnenroll(mockMediator.Object, options.Object);
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
            var taskId = 111;
            var eventId = 111;
            var campaignName = "Compaign1";
            var eventName = "Event1";
            var taskName = "Task1";

            var eventLink = $"View event: {siteBaseUrl}Event/Details/{eventId}";

            var message = new StringBuilder();
            message.AppendLine($"This is to confirm that you have elected to un-enroll from the following task:");
            message.AppendLine();
            message.AppendLine($"   Campaign: {campaignName}");
            message.AppendLine($"   Event: {eventName} ({eventLink})");
            message.AppendLine($"   Task: {taskName}");
            message.AppendLine();
            message.AppendLine("Thanks for letting us know that you will not be participating.");

            var notification = new UserUnenrolls { UserId = userId };

            var model = new TaskDetailForNotificationModel
            {
                TaskId = taskId,
                TaskName = taskName,
                EventId = eventId,
                EventName = eventName,
                CampaignName = campaignName,
                Volunteer = new ApplicationUser { Id = userId, Email = userId }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<TaskDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var options = new Mock<IOptions<GeneralSettings>>();
            options.Setup(o => o.Value).Returns(new GeneralSettings { SiteBaseUrl = siteBaseUrl });

            var handler = new NotifyVolunteerForTaskUnenroll(mockMediator.Object, options.Object);
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
            var subject = "allReady Task Un-enrollment Confirmation";

            var notification = new UserUnenrolls { UserId = userId };

            var model = new TaskDetailForNotificationModel
            {
                Volunteer = new ApplicationUser { Id = userId, Email = userId }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<TaskDetailForNotificationQueryAsync>())).ReturnsAsync(model);

            var options = new Mock<IOptions<GeneralSettings>>();
            options.Setup(o => o.Value).Returns(new GeneralSettings());

            var handler = new NotifyVolunteerForTaskUnenroll(mockMediator.Object, options.Object);
            await handler.Handle(notification);

            mockMediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(c =>
                c.ViewModel.Subject == subject
            )), Times.Once);
        }
    }
}
