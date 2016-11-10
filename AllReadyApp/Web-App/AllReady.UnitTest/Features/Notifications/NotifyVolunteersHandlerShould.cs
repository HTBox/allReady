using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using AllReady.Services;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Notifications
{
    public class NotifyVolunteersHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var htb = new Organization
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var firePrev = new Campaign
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb
            };

            var queenAnne = new Models.Event
            {
                Id = 1,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<EventSkill>(),
            };

            var username1 = $"blah@1.com";
            var username2 = $"blah@2.com";

            var user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true };
            Context.Users.Add(user1);
            var user2 = new ApplicationUser { UserName = username2, Email = username2, EmailConfirmed = true };
            Context.Users.Add(user2);

            htb.Campaigns.Add(firePrev);
            Context.Organizations.Add(htb);
            Context.Events.Add(queenAnne);

            Context.SaveChanges();
        }

        [Fact]
        public async Task SendMessageToAssignedVolunteers()
        {
            var command = new NotifyVolunteersCommand
            {
                ViewModel = new NotifyVolunteersViewModel
                {
                    Subject = "This is my subject",
                    EmailRecipients = new List<string> { "blah@1.com", "blah@2.com" },
                    EmailMessage = "This is my message"
                }
            };

            var queueWriter = new Mock<IQueueStorageService>();

            var handler = new NotifyVolunteersCommandHandler(queueWriter.Object);
            await handler.Handle(command);

            queueWriter.Verify(q => q.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }
    }
}