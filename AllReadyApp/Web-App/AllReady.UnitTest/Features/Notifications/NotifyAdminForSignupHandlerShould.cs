using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Configuration;
using Xunit;
using Moq;
using AllReady.Features.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using AllReady.Models;

namespace AllReady.UnitTest.Features.Notifications
{
    using Event = AllReady.Models.Event;

    public class NotifyAdminForSignupHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var testOrg = new Organization
            {
                Name = "My Test Campaign",
                LogoUrl = "http://www.htbox.org/testCampaign",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var testCampaign1 = new Campaign
            {
                Name = "Test Campaign 1",
                ManagingOrganization = testOrg,
                CampaignContacts = new List<CampaignContact>()
            };

            var testCampaign2 = new Campaign
            {
                Name = "Test Campaign 2",
                ManagingOrganization = testOrg,
                CampaignContacts = new List<CampaignContact>()
            };

            var primaryCampaignContact1 = new CampaignContact
            {
                ContactType = (int)ContactTypes.Primary,
                Contact = new Contact
                {
                    Email = "test@contactemail.com"
                }
            };

            var primaryCampaignContact2 = new CampaignContact
            {
                ContactType = (int)ContactTypes.Primary,
                Contact = new Contact()
            };

            var testEvent1 = new Event
            {
                Id = 5,
                Name = "Test Event Name",
                Campaign = testCampaign1,
                CampaignId = testCampaign1.Id,
                StartDateTime = DateTime.UtcNow.AddDays(10),
                EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(8),
                Location = new Location { Id = 2 },
                RequiredSkills = new List<EventSkill>()
            };

            var testEvent2 = new Event
            {
                Id = 7,
                Name = "Test Event 2",
                Campaign = testCampaign2,
                CampaignId = testCampaign2.Id,
                StartDateTime = DateTime.UtcNow.AddDays(3),
                EndDateTime = DateTime.UtcNow.AddDays(3).AddHours(4),
                Location = new Location { Id = 5 },
                RequiredSkills = new List<EventSkill>()
            };

            var username1 = $"user1@email.com";
            var user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true };
            Context.Users.Add(user1);

            var username2 = $"user2@email.com";
            var user2 = new ApplicationUser { UserName = username2, Email = username2, EmailConfirmed = true };
            Context.Users.Add(user2);

            testCampaign1.CampaignContacts.Add(primaryCampaignContact1);
            testCampaign2.CampaignContacts.Add(primaryCampaignContact2);

            testOrg.Campaigns.Add(testCampaign1);
            testOrg.Campaigns.Add(testCampaign2);

            Context.Organizations.Add(testOrg);
            Context.Events.Add(testEvent1);
            Context.Events.Add(testEvent2);

            var testTask1 = new VolunteerTask
            {
                Id = 7,
                Event = testEvent1,
                EventId = testEvent1.Id
                // Required Skills?
            };

            var testTask2 = new VolunteerTask
            {
                Id = 9,
                Event = testEvent2,
                EventId = testEvent2.Id
                // Required Skills?
            };

            Context.VolunteerTasks.Add(testTask1);
            Context.VolunteerTasks.Add(testTask2);

            Context.SaveChanges();
        }

        /// <summary>
        /// Verifies that an object of type NotifyVolunteersCommand 
        /// is sent to the mediator
        /// </summary>
        [Fact]
        public async void PassANotifyVolunteersCommandToTheMediator()
        {
            var taskDetailForNotificationModel = new VolunteerTaskDetailForNotificationModel
            {
                Volunteer = new ApplicationUser { Email = "VolunteerEmail", PhoneNumber = "VolunteerPhoneNumber" },
                CampaignContacts = new List<CampaignContact>
                {
                    new CampaignContact
                    {
                        ContactType = (int) ContactTypes.Primary,
                        Contact = new Contact {Email = "CampaignContactEmail"}
                    }
                }
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskDetailForNotificationQuery>()))
                .ReturnsAsync(taskDetailForNotificationModel);

            var options = new TestOptions<GeneralSettings>();
            options.Value.SiteBaseUrl = "localhost";

            var notification = new VolunteerSignedUpNotification
            {
                UserId = Context.Users.First().Id,
                VolunteerTaskId = Context.VolunteerTasks.First().Id
            };

            var target = new NotifyAdminForSignupHandler(Context, mediator.Object, options, null);
            await target.Handle(notification);

            mediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Once);
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
            const string adminEmail = "AdminEmail";

            var taskDetailForNotificationModel = new VolunteerTaskDetailForNotificationModel
            {
                Volunteer = new ApplicationUser { Email = "VolunteerEmail", PhoneNumber = "VolunteerPhoneNumber" },
                CampaignContacts = new List<CampaignContact>
                {
                    new CampaignContact
                    {
                        ContactType = (int) ContactTypes.Primary,
                        Contact = new Contact { Email = adminEmail }
                    }
                }
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskDetailForNotificationQuery>()))
                .ReturnsAsync(taskDetailForNotificationModel);

            var options = new TestOptions<GeneralSettings>();
            options.Value.SiteBaseUrl = "localhost";

            var notification = new VolunteerSignedUpNotification
            {
                UserId = Context.Users.First().Id,
                VolunteerTaskId = Context.VolunteerTasks.First().Id
            };

            var target = new NotifyAdminForSignupHandler(Context, mediator.Object, options, null);
            await target.Handle(notification);

            mediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(y => !string.IsNullOrWhiteSpace(y.ViewModel.Subject))), Times.Once);
        }

        /// <summary>
        /// Verifies that the correct admin email address is passed to 
        /// the mediator within the NotifyVolunteersCommand
        /// </summary>
        [Fact]
        public async void SendToTheAdminEmail()
        {
            const string adminEmail = "AdminEmail";

            var taskDetailForNotificationModel = new VolunteerTaskDetailForNotificationModel
            {
                Volunteer = new ApplicationUser { Email = "VolunteerEmail", PhoneNumber = "VolunteerPhoneNumber" },
                CampaignContacts = new List<CampaignContact>
                {
                    new CampaignContact
                    {
                        ContactType = (int) ContactTypes.Primary,
                        Contact = new Contact { Email = adminEmail }
                    }
                }
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<VolunteerTaskDetailForNotificationQuery>()))
                .ReturnsAsync(taskDetailForNotificationModel);

            var options = new TestOptions<GeneralSettings>();
            options.Value.SiteBaseUrl = "localhost";

            var notification = new VolunteerSignedUpNotification
            {
                UserId = Context.Users.First().Id,
                VolunteerTaskId = Context.VolunteerTasks.First().Id
            };

            var target = new NotifyAdminForSignupHandler(Context, mediator.Object, options, null);
            await target.Handle(notification);

            mediator.Verify(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(y => y.ViewModel.EmailRecipients.Contains(adminEmail))), Times.Once);
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
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Throws(new InvalidOperationException("Test Exception"));

            var logger = new Mock<ILogger<NotifyAdminForUserUnenrollsHandler>>();
            var options = new TestOptions<GeneralSettings>();
            options.Value.SiteBaseUrl = "localhost";

            var notification = new VolunteerSignedUpNotification
            {
                UserId = Context.Users.First().Id,
                VolunteerTaskId = Context.VolunteerTasks.First().Id
            };

            var target = new NotifyAdminForSignupHandler(Context, mediator.Object, options, logger.Object);
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
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Throws(new InvalidOperationException("Test Exception"));

            var logger = new Mock<ILogger<NotifyAdminForUserUnenrollsHandler>>();
            logger.Setup(x => x.Log(It.Is<LogLevel>(m => m == LogLevel.Error),
                It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()));

            var options = new TestOptions<GeneralSettings>();
            options.Value.SiteBaseUrl = "localhost";

            var notification = new VolunteerSignedUpNotification
            {
                UserId = Context.Users.First().Id,
                VolunteerTaskId = Context.VolunteerTasks.First().Id
            };

            var target = new NotifyAdminForSignupHandler(Context, mediator.Object, options, logger.Object);
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
            var mediator = new Mock<IMediator>();

            mediator.Setup(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .Throws(new InvalidOperationException("Test Exception"));

            var logger = new Mock<ILogger<NotifyAdminForUserUnenrollsHandler>>();
            var options = new TestOptions<GeneralSettings>();
            options.Value.SiteBaseUrl = "localhost";

            var notification = new VolunteerSignedUpNotification
            {
                UserId = Context.Users.First().Id,
                VolunteerTaskId = Context.VolunteerTasks.First().Id
            };

            var target = new NotifyAdminForSignupHandler(Context, mediator.Object, options, logger.Object);
            await target.Handle(notification);

            logger.Verify(x => x.Log(It.IsAny<LogLevel>(),
                It.IsAny<EventId>(), It.IsAny<object>(),
                It.IsAny<InvalidOperationException>(),
                It.IsAny<Func<object, Exception, string>>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Verifies that the send method is never called if
        /// the admin email address has not been specified
        /// </summary>
        [Fact]
        public async void SkipNotificationIfAdminEmailIsNotSpecified()
        {
            var mediator = new Mock<IMediator>();
            var logger = Mock.Of<ILogger<NotifyAdminForUserUnenrollsHandler>>();

            var options = new TestOptions<GeneralSettings>();
            options.Value.SiteBaseUrl = "localhost";

            var notification = new VolunteerSignedUpNotification
            {
                UserId = Context.Users.First().Id,
                VolunteerTaskId = Context.VolunteerTasks.Skip(1).First().Id
            };

            var target = new NotifyAdminForSignupHandler(Context, mediator.Object, options, logger);
            await target.Handle(notification);

            mediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }
    }
}
