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

            var task1 = new VolunteerTask
            {
                Id = 1,
                Name = "Task 1",
                Event = campaignEvent,
                NumberOfVolunteersRequired = 22
            };

            var task2 = new VolunteerTask
            {
                Id = 2,
                Name = "Task 2",
                Event = campaignEvent,
                NumberOfVolunteersRequired = 8
            };

            var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), Email = "user@example.com " };
            var user2 = new ApplicationUser { Id = Guid.NewGuid().ToString(), Email = "user2@example.com " };

            var taskSignup1 = new VolunteerTaskSignup
            {
                User = user,
                VolunteerTask = task1,
                Status = VolunteerTaskStatus.Accepted
            };

            var taskSignup2 = new VolunteerTaskSignup
            {
                User = user2,
                VolunteerTask = task1,
                Status = VolunteerTaskStatus.Accepted
            };

            var taskSignup3 = new VolunteerTaskSignup
            {
                User = user,
                VolunteerTask = task2,
                Status = VolunteerTaskStatus.Accepted
            };

            Context.Add(campaign);
            Context.Add(campaignEvent);
            Context.Add(task1);
            Context.Add(task2);
            Context.Add(user);
            Context.Add(user2);
            Context.Add(taskSignup1);
            Context.Add(taskSignup2);
            Context.Add(taskSignup3);
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
