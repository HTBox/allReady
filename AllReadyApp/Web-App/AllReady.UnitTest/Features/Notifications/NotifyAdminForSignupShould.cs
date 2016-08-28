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

            var testCampaign = new Campaign()
            {
                Name = "Test Campaign 1",
                ManagingOrganization = testOrg,
                CampaignContacts = new List<CampaignContact>()
            };

            var primaryCampaignContact = new CampaignContact()
            {
                ContactType = (int)ContactTypes.Primary,
                Contact = new Contact()
                {
                    Email = "test@contactemail.com"
                }
            };

            var testEvent = new Models.Event()
            {
                Id = 5,
                Name = "Test Event Name",
                Campaign = testCampaign,
                CampaignId = testCampaign.Id,
                StartDateTime = DateTime.UtcNow.AddDays(10),
                EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(8),
                Location = new Location { Id = 2 },
                RequiredSkills = new List<EventSkill>()
            };

            var username1 = $"user1@email.com";
            var user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true };
            Context.Users.Add(user1);

            var username2 = $"user2@email.com";
            var user2 = new ApplicationUser { UserName = username2, Email = username2, EmailConfirmed = true };
            Context.Users.Add(user2);

            testCampaign.CampaignContacts.Add(primaryCampaignContact);
            testOrg.Campaigns.Add(testCampaign);

            this.Context.Organizations.Add(testOrg);
            this.Context.Events.Add(testEvent);

            var eventSignups = new List<EventSignup>();
            eventSignups.Add(new EventSignup { Event = testEvent, User = user1, SignupDateTime = DateTime.UtcNow });
            eventSignups.Add(new EventSignup { Event = testEvent, User = user2, SignupDateTime = DateTime.UtcNow });
            this.Context.EventSignup.AddRange(eventSignups);

            var testTask = new AllReadyTask()
            {
                Id = 7,
                Event = testEvent,
                EventId = testEvent.Id
                // Required Skills?
            };

            this.Context.Tasks.Add(testTask);

            this.Context.SaveChanges();
        }

        [Fact]
        public async void PassANotifyVolunteersCommandToTheMediator()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<NotifyVolunteersCommand>())).ReturnsAsync(new Unit());

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

        [Fact(Skip = "NotImplemented")]
        public void SendToTheAdminEmail()
        {
            // TODO: Implement test
        }

        [Fact(Skip = "NotImplemented")]
        public void LogIfAnExceptionOccurs()
        {
            // TODO: Implement test
        }

        [Fact(Skip = "NotImplemented")]
        public void SkipNotificationIfAdminEmailIsNotSpecified()
        {
            // TODO: Implement test
        }


    }
}