using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using AllReady.UnitTest.Features.Campaigns;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AllReady.UnitTest.Notifications
{
    public class ActivityDetailForNotificationQueryHandlerAsyncTests : InMemoryContextTest
    {
        private ApplicationUser _user1;
        private Organization _htb;
        private Campaign _firePrev;
        private Activity _queenAnne;
        private Contact _contact1;
        private AllReadyTask _task1;

        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();

            _user1 = new ApplicationUser
            {
                UserName = "johndoe@example.com",
                Name = "John Doe",
                Email = "johndoe@example.com"
            };

            _contact1 = new Contact
            {
                Id = 1,
                FirstName = "Jerry",
                LastName = "Rodgers",
                Email = "jerry@example.com",
                PhoneNumber = "555.555.1234"
            };

            _htb = new Organization
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            _firePrev = new Campaign
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = _htb,
                CampaignContacts = new List<CampaignContact>()
            };

            _firePrev.CampaignContacts.Add(new CampaignContact
            {
                Campaign = _firePrev,
                Contact = _contact1,
                ContactType = (int)ContactTypes.Primary
            });

            _htb.Campaigns.Add(_firePrev);

            _queenAnne = new Activity
            {
                Id = 1,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = _firePrev,
                CampaignId = _firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<ActivitySkill>(),
                Tasks = new List<AllReadyTask>(),
                UsersSignedUp = new List<ActivitySignup>
                {
                    new ActivitySignup
                    {
                        Id = 1,
                        PreferredEmail = "testuser@gmail.com",
                        PreferredPhoneNumber = "(555)555-1234",
                        User = _user1
                    }
                }
            };

            _task1 = new AllReadyTask
            {
               Id = 1,
               Activity = _queenAnne,
               Name = "Task 1",
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Organization = _htb
            };

            _task1.AssignedVolunteers = new List<TaskSignup>
            {
                new TaskSignup
                {
                    Id = 1,
                    User = _user1,
                    Task = _task1,
                    Status = "Assigned",
                    StatusDateTimeUtc = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime()
                }
            };

            _queenAnne.Tasks.Add(_task1);
            context.Users.Add(_user1);
            context.Contacts.Add(_contact1);
            context.Organizations.Add(_htb);
            context.Activities.Add(_queenAnne);
            context.SaveChanges();
        }

        [Fact]
        public async Task ModelCanBeCreatedFomExistingActivity()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new ActivityDetailForNotificationQueryAsync { ActivityId = 1, UserId = _user1.Id };
            var handler = new ActivityDetailForNotificationQueryHandlerAsync(context);

            var result = await handler.Handle(query);

            Assert.NotNull(result);
            Assert.True(_queenAnne.UsersSignedUp.Count == result.UsersSignedUp.Count, "Count of signed up users does not match");
            Assert.True(_queenAnne.Tasks.Count == result.Tasks.Count, "Count of tasks does not match");
            Assert.True(_queenAnne.Tasks[0].AssignedVolunteers.Count == result.Tasks[0].AssignedVolunteers.Count, "Count of volunteers assigned to tak does not match");
            Assert.True(_firePrev.CampaignContacts.Count == result.CampaignContacts.Count, "Count of campaign contacts does not match");
        }

        [Fact]
        public async Task ActivityDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new ActivityDetailForNotificationQueryAsync { ActivityId = 999, UserId = _user1.Id};
            var handler = new ActivityDetailForNotificationQueryHandlerAsync(context);

            var result = await handler.Handle(query);

            Assert.Null(result);
        }
    }
}
