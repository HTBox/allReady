using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AllReady.Features.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AllReady.Models;

namespace AllReady.UnitTest.Features.Notifications
{
    public class NotifyAdminForSignupShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var testOrg = new Organization()
            {
                Name = "My Test Campaign",
                LogoUrl = "http://www.htbox.org/testCampaign",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var testCampaign1 = new Campaign()
            {
                Name = "Test Campaign 1",
                ManagingOrganization = testOrg,
                CampaignContacts = new List<CampaignContact>()
            };

            var testCampaign2 = new Campaign()
            {
                Name = "Test Campaign 2",
                ManagingOrganization = testOrg,
                CampaignContacts = new List<CampaignContact>()
            };

            var primaryCampaignContact1 = new CampaignContact()
            {
                ContactType = (int)ContactTypes.Primary,
                Contact = new Contact()
                {
                    Email = "test@contactemail.com"
                }
            };

            var primaryCampaignContact2 = new CampaignContact()
            {
                ContactType = (int)ContactTypes.Primary,
                Contact = new Contact()
            };

            var testEvent1 = new Models.Event()
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

            var testEvent2 = new Models.Event()
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

            this.Context.Organizations.Add(testOrg);
            this.Context.Events.Add(testEvent1);
            this.Context.Events.Add(testEvent2);

            var eventSignups = new List<EventSignup>();
            eventSignups.Add(new EventSignup { Event = testEvent1, User = user1, SignupDateTime = DateTime.UtcNow });
            eventSignups.Add(new EventSignup { Event = testEvent1, User = user2, SignupDateTime = DateTime.UtcNow });
            eventSignups.Add(new EventSignup { Event = testEvent2, User = user1, SignupDateTime = DateTime.UtcNow.AddMinutes(-25) });
            this.Context.EventSignup.AddRange(eventSignups);

            var testTask1 = new AllReadyTask()
            {
                Id = 7,
                Event = testEvent1,
                EventId = testEvent1.Id
                // Required Skills?
            };

            var testTask2 = new AllReadyTask()
            {
                Id = 9,
                Event = testEvent2,
                EventId = testEvent2.Id
                // Required Skills?
            };

            this.Context.Tasks.Add(testTask1);
            this.Context.Tasks.Add(testTask2);

            this.Context.SaveChanges();
        }

        [Fact]
        public async void PassANotifyVolunteersCommandToTheMediator()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()))
                .ReturnsAsync(new Unit());

            var logger = Mock.Of<ILogger<NotifyAdminForUserUnenrolls>>();

            var options = new TestOptions<GeneralSettings>();
            options.Value.SiteBaseUrl = "localhost";

            var notification = new VolunteerSignupNotification()
            {
                UserId = Context.Users.First().Id,
                EventId = Context.Events.First().Id,
                TaskId = Context.Tasks.First().Id
            };

            var target = new NotifyAdminForSignup(this.Context, mediator.Object, options, logger);
            await target.Handle(notification);

            mediator.VerifyAll();
        }

        [Fact]
        public async void PassAnEmailSubjectToTheMediator()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(c => !String.IsNullOrWhiteSpace(c.ViewModel.Subject))))
                .ReturnsAsync(new Unit());

            var logger = Mock.Of<ILogger<NotifyAdminForUserUnenrolls>>();

            var options = new TestOptions<GeneralSettings>();
            options.Value.SiteBaseUrl = "localhost";

            var notification = new VolunteerSignupNotification()
            {
                UserId = Context.Users.First().Id,
                EventId = Context.Events.First().Id,
                TaskId = Context.Tasks.First().Id
            };

            var target = new NotifyAdminForSignup(this.Context, mediator.Object, options, logger);
            await target.Handle(notification);

            mediator.VerifyAll();
        }

        [Fact]
        public async void SendToTheAdminEmail()
        {
            var testEvent = Context.Events.First();
            var testCampaign = testEvent.Campaign;
            var testContact = testCampaign.CampaignContacts.First().Contact;
            var expected = testContact.Email;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<NotifyVolunteersCommand>(c => c.ViewModel.EmailRecipients.ToArray()[0] == expected)))
                .ReturnsAsync(new Unit());

            var logger = Mock.Of<ILogger<NotifyAdminForUserUnenrolls>>();

            var options = new TestOptions<GeneralSettings>();
            options.Value.SiteBaseUrl = "localhost";

            var notification = new VolunteerSignupNotification()
            {
                UserId = Context.Users.First().Id,
                EventId = testEvent.Id,
                TaskId = Context.Tasks.First().Id
            };

            var target = new NotifyAdminForSignup(this.Context, mediator.Object, options, logger);
            await target.Handle(notification);

            mediator.VerifyAll();
        }

        [Fact(Skip = "NotImplemented")]
        public void LogIfAnExceptionOccurs()
        {
            // TODO: Implement test
        }

        [Fact]
        public async void SkipNotificationIfAdminEmailIsNotSpecified()
        {
            var testEvent = Context.Events.Skip(1).First();
            //var testCampaign = testEvent.Campaign;
            //var testContact = testCampaign.CampaignContacts.First().Contact;
            //var expected = testContact.Email;

            var mediator = new Mock<IMediator>();
            var logger = Mock.Of<ILogger<NotifyAdminForUserUnenrolls>>();

            var options = new TestOptions<GeneralSettings>();
            options.Value.SiteBaseUrl = "localhost";

            var notification = new VolunteerSignupNotification()
            {
                UserId = Context.Users.First().Id,
                EventId = testEvent.Id,
                TaskId = Context.Tasks.Skip(1).First().Id
            };

            var target = new NotifyAdminForSignup(this.Context, mediator.Object, options, logger);
            await target.Handle(notification);

            mediator.Verify(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>()), Times.Never);
        }
    }
}