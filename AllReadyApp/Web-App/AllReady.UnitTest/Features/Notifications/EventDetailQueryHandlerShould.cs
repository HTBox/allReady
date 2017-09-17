using AllReady.Areas.Admin.Features.Events;
using AllReady.Models;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Notifications
{
    using Event = AllReady.Models.Event;

    public class EventDetailQueryHandlerShould : InMemoryContextTest
    {
        public EventDetailQueryHandlerShould()
        {
            var org = new Organization
            {
                Id = 1,
                Name = "Organization"
            };

            var campaign = new Campaign
            {
                Id = 1,
                Name = "Campaign",
                ManagingOrganization = org
            };

            var campaignEvent = new Event
            {
                Id = 1,
                Name = "Event",
                Campaign = campaign
            };

            var volunteerTask1 = new VolunteerTask
            {
                Id = 1,
                Name = "Task 1",
                Event = campaignEvent,
                NumberOfVolunteersRequired = 22
            };

            var volunteerTask2 = new VolunteerTask
            {
                Id = 2,
                Name = "Task 2",
                Event = campaignEvent,
                NumberOfVolunteersRequired = 8
            };

            var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), Email = "user@example.com " };
            var user2 = new ApplicationUser { Id = Guid.NewGuid().ToString(), Email = "user2@example.com " };

            var volunteerTaskSignup1 = new VolunteerTaskSignup
            {
                User = user,
                VolunteerTask = volunteerTask1,
                Status = VolunteerTaskStatus.Accepted
            };

            var volunteerTaskSignup2 = new VolunteerTaskSignup
            {
                User = user2,
                VolunteerTask = volunteerTask1,
                Status = VolunteerTaskStatus.Accepted
            };

            var volunteerTaskSignup3 = new VolunteerTaskSignup
            {
                User = user,
                VolunteerTask = volunteerTask2,
                Status = VolunteerTaskStatus.Accepted
            };

            Context.Add(campaign);
            Context.Add(campaignEvent);
            Context.Add(volunteerTask1);
            Context.Add(volunteerTask2);
            Context.Add(user);
            Context.Add(user2);
            Context.Add(volunteerTaskSignup1);
            Context.Add(volunteerTaskSignup2);
            Context.Add(volunteerTaskSignup3);
            Context.SaveChanges();
        }

        [Fact]
        public async Task EventDetailQueryHandler_ReturnsCorrectVolunteersRequiredValue()
        {
            var handler = new EventDetailQueryHandler(Context);
            var result = await handler.Handle(new EventDetailQuery { EventId = 1 });

            result.VolunteersRequired.ShouldBe(30);
        }

        [Fact]
        public async Task EventDetailQueryHandler_ReturnsCorrectAcceptedVolunteerValue()
        {
            var handler = new EventDetailQueryHandler(Context);
            var result = await handler.Handle(new EventDetailQuery { EventId = 1 });

            result.AcceptedVolunteers.ShouldBe(3);
        }
    }
}
