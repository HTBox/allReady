using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using AllReady.Models;
using AllReady.Features.Volunteers;

namespace AllReady.UnitTest.Features.Event
{
    using Event = AllReady.Models.Event;

    public class GetVolunteerEventsHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnExpectedEvents()
        {
            var userId = "UserId1";
            var message = new GetVolunteerEventsQuery { UserId = userId };
            var sut = new GetVolunteerEventsQueryHandler(Context);
            var response = await sut.Handle(message);

            Assert.True(response.CurrentEvents.Count() ==1
                     && response.FutureEvents.Count() ==2);
        }

        protected override void LoadTestData()
        {
            var currentDateTime = new DateTimeOffset(DateTime.UtcNow);

            var campaign = new Campaign
            {
                Id = 1,
                Name = "TestCampaign",
                ManagingOrganization = new Organization { Id = 1, Name = "TestOrg" }
            };

            var campaignEvent1 = new Event
            {
                Id = 1,
                Name = "Event1",
                Campaign = campaign,
                StartDateTime = currentDateTime.AddDays(-7),
                EndDateTime = currentDateTime.AddDays(7)
            };

            var campaignEvent2 = new Event
            {
                Id = 2,
                Name = "Event2",
                Campaign = campaign,
                StartDateTime = currentDateTime.AddDays(7),
                EndDateTime = currentDateTime.AddDays(14)
            };

            var campaignEvent3 = new Event
            {
                Id = 3,
                Name = "Event3",
                Campaign = campaign,
                StartDateTime = currentDateTime.AddDays(7),
                EndDateTime = currentDateTime.AddDays(14)
            };

            var user1 = new ApplicationUser { Id = "UserId1" };
            var user2 = new ApplicationUser { Id = "UserId2" };

            var volunteerTaskSignup1 = new VolunteerTaskSignup
            {
                User = user1,
                VolunteerTask = new VolunteerTask { Id = 1, Name = "Task 1", Event = campaignEvent1 }
            };

            var volunteerTaskSignup2 = new VolunteerTaskSignup
            {
                User = user2,
                VolunteerTask = new VolunteerTask { Id = 2, Name = "Task 2", Event = campaignEvent2 }
            };

            var volunteerTaskSignup3 = new VolunteerTaskSignup
            {
                User = user1,
                VolunteerTask = new VolunteerTask { Id = 3, Name = "Task 3", Event = campaignEvent3 }
            };

            Context.Add(volunteerTaskSignup1);
            Context.Add(volunteerTaskSignup2);
            Context.Add(volunteerTaskSignup3);
            Context.SaveChanges();
        }
    }
}
