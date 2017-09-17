using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Features.Notifications
{
    using Event = AllReady.Models.Event;

    public class VolunteerTaskDetailForNotificationQueryHandlerShould : InMemoryContextTest
    {
        private ApplicationUser _user1;
        private Organization _htb;
        private Campaign _firePrev;
        private Event _queenAnne;
        private Contact _contact1;
        private VolunteerTask _task1;

        protected override void LoadTestData()
        {
            _user1 = new ApplicationUser
            {
                UserName = "johndoe@example.com",
                FirstName = "John",
                LastName = "Doe",
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

            _queenAnne = new Event
            {
                Id = 1,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = _firePrev,
                CampaignId = _firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<EventSkill>(),
                VolunteerTasks = new List<VolunteerTask>()
            };

            _task1 = new VolunteerTask
            {
                Id = 1,
                Event = _queenAnne,
                Name = "Task 1",
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Organization = _htb
            };

            _task1.AssignedVolunteers = new List<VolunteerTaskSignup>
            {
                new VolunteerTaskSignup
                {
                    Id = 1,
                    User = _user1,
                    VolunteerTask = _task1,
                    Status = VolunteerTaskStatus.Assigned,
                    StatusDateTimeUtc = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime()
                }
            };

            _queenAnne.VolunteerTasks.Add(_task1);
            Context.Users.Add(_user1);
            Context.Contacts.Add(_contact1);
            Context.Organizations.Add(_htb);
            Context.Events.Add(_queenAnne);
            Context.SaveChanges();
        }

        [Fact]
        public async Task EventDoesNotExist()
        {
            var query = new VolunteerTaskDetailForNotificationQuery { VolunteerTaskId = 999, UserId = _user1.Id };
            var handler = new VolunteerTaskDetailForNotificationQueryHandler(Context);

            var result = await handler.Handle(query);

            Assert.Null(result);
        }
    }
}
