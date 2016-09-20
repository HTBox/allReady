using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using AllReady.Features.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using AllReady.Models;
using AllReady.Areas.Admin.ViewModels.Task;

namespace AllReady.UnitTest.Features.Notifications
{
    public class NotifyAdminForUserUnenrollShould : InMemoryContextTest
    {
        /// <summary>
        /// Verifies that an object of type NotifyVolunteersCommand 
        /// is sent to the mediator
        /// </summary>
        [Fact]
        public async void PassANotifyVolunteersCommandToTheMediator()
        {
            int testTaskId = 29;
            int testEventId = 15;

            var mediator = new Mock<IMediator>();

            // Setup mock data load
            mediator
                .Setup(x => x.SendAsync<EventDetailForNotificationModel>(It.IsAny<EventDetailForNotificationQueryAsync>()))
                .ReturnsAsync(GetEventDetailForNotificationModel(testTaskId, testEventId));

            // Setup action call
            mediator.Setup(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .ReturnsAsync(new Unit());

            var logger = Mock.Of<ILogger<NotifyAdminForUserUnenrolls>>();
            TestOptions<GeneralSettings> options = GetSettings();
            UserUnenrolls notification = GetUserUnenrolls(testTaskId, testEventId);

            var target = new NotifyAdminForUserUnenrolls(mediator.Object, options, logger);
            await target.Handle(notification);

            mediator.VerifyAll();
        }

        /// <summary>
        /// Verifies that a non-empty subject is passed to the mediator
        /// within the NotifyVolunteersCommand
        /// </summary>
        /// <remarks>We are only checking for a non-empty value here since
        /// the actual content of the subject is not a requirement, just
        /// that there be one.  Adding a requirement for a specific subject
        /// line can be done, but would likely make this test more fragile</remarks>
        [Fact]
        public async void PassAnEmailSubjectToTheMediator()
        {
            int testTaskId = 29;
            int testEventId = 15;

            var mediator = new Mock<IMediator>();

            // Setup mock data load
            mediator
                .Setup(x => x.SendAsync<EventDetailForNotificationModel>(It.IsAny<EventDetailForNotificationQueryAsync>()))
                .ReturnsAsync(GetEventDetailForNotificationModel(testTaskId, testEventId));

            // Setup action call
            mediator.Setup(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(n => !string.IsNullOrWhiteSpace(n.ViewModel.Subject))))
                .ReturnsAsync(new Unit());

            var logger = Mock.Of<ILogger<NotifyAdminForUserUnenrolls>>();
            TestOptions<GeneralSettings> options = GetSettings();
            UserUnenrolls notification = GetUserUnenrolls(testTaskId, testEventId);

            var target = new NotifyAdminForUserUnenrolls(mediator.Object, options, logger);
            await target.Handle(notification);

            mediator.VerifyAll();
        }

        /// <summary>
        /// Verifies that the correct admin email address is passed to 
        /// the mediator within the NotifyVolunteersCommand
        /// </summary>
        [Fact]
        public async void SendToTheAdminEmail()
        {
            int testTaskId = 29;
            int testEventId = 15;

            var eventDetail = GetEventDetailForNotificationModel(testTaskId, testEventId);
            var expectedEmail = eventDetail.CampaignContacts.First().Contact.Email;

            var mediator = new Mock<IMediator>();

            // Setup mock data load
            mediator
                .Setup(x => x.SendAsync<EventDetailForNotificationModel>(It.IsAny<EventDetailForNotificationQueryAsync>()))
                .ReturnsAsync(eventDetail);

            // Setup action call
            mediator.Setup(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(n => n.ViewModel.EmailRecipients.Contains(expectedEmail))))
                .ReturnsAsync(new Unit());

            var logger = Mock.Of<ILogger<NotifyAdminForUserUnenrolls>>();
            TestOptions<GeneralSettings> options = GetSettings();
            UserUnenrolls notification = GetUserUnenrolls(testTaskId, testEventId);

            var target = new NotifyAdminForUserUnenrolls(mediator.Object, options, logger);
            await target.Handle(notification);

            mediator.VerifyAll();
        }

        /// <summary>
        /// Verifies that a log entry is written if an error occurs during 
        /// the processing of a notification by the mediator
        /// </summary>
        /// <remarks>No verification of the content of the log entry is done 
        /// here. The few requirements that there are for logging will be 
        /// verified in other tests.</remarks>
        [Fact]
        public async void LogIfAnExceptionOccurs()
        {
            int testTaskId = 29;
            int testEventId = 15;

            var mediator = new Mock<IMediator>();

            // Setup mock data load
            mediator
                .Setup(x => x.SendAsync<EventDetailForNotificationModel>(It.IsAny<EventDetailForNotificationQueryAsync>()))
                .ReturnsAsync(GetEventDetailForNotificationModel(testTaskId, testEventId));

            // Setup exception
            mediator.Setup(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Throws(new InvalidOperationException("Test Exception"));

            var logger = new Mock<ILogger<NotifyAdminForUserUnenrolls>>();

            TestOptions<GeneralSettings> options = GetSettings();
            UserUnenrolls notification = GetUserUnenrolls(testTaskId, testEventId);

            var target = new NotifyAdminForUserUnenrolls(mediator.Object, options, logger.Object);
            await target.Handle(notification);

            logger.Verify(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(),
                        It.IsAny<object>(), It.IsAny<Exception>(),
                        It.IsAny<Func<object, Exception, string>>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Verifies that the log entry that is written when an error
        /// occurs during notification processing is done at a LogLevel
        /// of "Error".
        /// </summary>
        [Fact]
        public async void LogAnErrorIfAnExceptionOccurs()
        {
            int testTaskId = 29;
            int testEventId = 15;

            var mediator = new Mock<IMediator>();

            // Setup mock data load
            mediator
                .Setup(x => x.SendAsync<EventDetailForNotificationModel>(It.IsAny<EventDetailForNotificationQueryAsync>()))
                .ReturnsAsync(GetEventDetailForNotificationModel(testTaskId, testEventId));

            // Setup exception
            mediator.Setup(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Throws(new InvalidOperationException("Test Exception"));

            // Setup expected logging
            var logger = new Mock<ILogger<NotifyAdminForUserUnenrolls>>();
            logger.Setup(x => x.Log(It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(),
                        It.IsAny<Func<object, Exception, string>>()));

            TestOptions<GeneralSettings> options = GetSettings();
            UserUnenrolls notification = GetUserUnenrolls(testTaskId, testEventId);

            var target = new NotifyAdminForUserUnenrolls(mediator.Object, options, logger.Object);
            await target.Handle(notification);

            logger.VerifyAll();
        }

        /// <summary>
        /// Verifies that the log entry that is written when an error
        /// occurs during notification processing receives the correct
        /// Exception type.
        /// </summary>
        [Fact]
        public async void LogTheExceptionIfAnExceptionOccurs()
        {
            int testTaskId = 29;
            int testEventId = 15;

            var mediator = new Mock<IMediator>();

            // Setup mock data load
            mediator
                .Setup(x => x.SendAsync<EventDetailForNotificationModel>(It.IsAny<EventDetailForNotificationQueryAsync>()))
                .ReturnsAsync(GetEventDetailForNotificationModel(testTaskId, testEventId));

            // Setup exception
            mediator.Setup(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Throws(new InvalidOperationException("Test Exception"));

            // Setup expected logging
            var logger = new Mock<ILogger<NotifyAdminForUserUnenrolls>>();
            logger.Setup(x => x.Log(It.IsAny<LogLevel>(),
                It.IsAny<EventId>(), It.IsAny<object>(),
                It.IsAny<NullReferenceException>(),
                It.IsAny<Func<object, Exception, string>>()));

            TestOptions<GeneralSettings> options = GetSettings();
            UserUnenrolls notification = GetUserUnenrolls(testTaskId, testEventId);

            var target = new NotifyAdminForUserUnenrolls(mediator.Object, options, logger.Object);
            await target.Handle(notification);

            logger.VerifyAll();
        }

        /// <summary>
        /// Verifies that the send process is never called if
        /// the admin email address has not been specified
        /// </summary>
        [Fact]
        public async void SkipNotificationIfAdminEmailIsNotSpecified()
        {
            int testTaskId = 29;
            int testEventId = 15;
            var eventDetails = GetEventDetailForNotificationModel(testTaskId, testEventId, string.Empty);

            var mediator = new Mock<IMediator>();

            // Setup mock data load
            mediator
                .Setup(x => x.SendAsync<EventDetailForNotificationModel>(It.IsAny<EventDetailForNotificationQueryAsync>()))
                .ReturnsAsync(eventDetails);

            var logger = Mock.Of<ILogger<NotifyAdminForUserUnenrolls>>();
            TestOptions<GeneralSettings> options = GetSettings();
            UserUnenrolls notification = GetUserUnenrolls(testTaskId, testEventId);

            var target = new NotifyAdminForUserUnenrolls(mediator.Object, options, logger);
            await target.Handle(notification);

            // Verify the action call never occurs
            mediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }


        private static TestOptions<GeneralSettings> GetSettings()
        {
            var options = new TestOptions<GeneralSettings>();
            options.Value.SiteBaseUrl = "localhost";
            return options;
        }

        private static UserUnenrolls GetUserUnenrolls(int taskId, int eventId)
        {
            return new UserUnenrolls()
            {
                EventId = eventId,
                TaskIds = new List<int>() { taskId }
            };
        }

        private static EventDetailForNotificationModel GetEventDetailForNotificationModel(int taskId, int eventId)
        {
            string adminEmail = "test@contactemail.com";
            return GetEventDetailForNotificationModel(taskId, eventId, adminEmail);
        }

        private static EventDetailForNotificationModel GetEventDetailForNotificationModel(int taskId, int eventId, string adminEmail)
        {
            string testUserId = Guid.NewGuid().ToString();
            string testUserEmail = $"{Guid.NewGuid().ToString()}@email.com";
            string testUserFirstName = "Joe";
            string testUserLastName = "Volunteer";
            string testCampaignName = Guid.NewGuid().ToString();
            string testEventName = "Test Event";
            string testTaskName = "Test Task 1";
            string testTaskDescription = "Task Description 1";
            DateTime testTaskStart = DateTime.UtcNow.AddDays(5);
            return GetEventDetailForNotificationModel(taskId, eventId, adminEmail, testUserId, testUserEmail, testUserFirstName, testUserLastName, testCampaignName, testEventName, testTaskName, testTaskDescription, testTaskStart);
        }

        private static EventDetailForNotificationModel GetEventDetailForNotificationModel(int taskId, int eventId, string adminEmail, string testUserId, string testUserEmail, string testUserFirstName, string testUserLastName, string testCampaignName, string testEventName, string testTaskName, string testTaskDescription, DateTime testTaskStart)
        {
            return new EventDetailForNotificationModel()
            {
                CampaignContacts = new List<CampaignContact>()
                        {
                            new CampaignContact()
                                    {
                                        ContactType = (int)ContactTypes.Primary,
                                        Contact = new Contact()
                                        {
                                            Email = adminEmail
                                        }
                                    }
                        },
                Tasks = new List<TaskSummaryViewModel>()
                    {
                        new TaskSummaryViewModel()
                        {
                            Id = taskId,
                            Name = testTaskName,
                            Description = testTaskDescription,
                            StartDateTime = testTaskStart
                        }
                    },
                Volunteer = new ApplicationUser()
                {
                    Id = testUserId,
                    FirstName = testUserFirstName,
                    LastName = testUserLastName,
                    Email = testUserEmail
                },
                CampaignName = testCampaignName,
                EventName = testEventName,
                EventId = eventId
            };
        }
    }
}