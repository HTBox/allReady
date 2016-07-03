using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class EventDetailQueryHandlerTests : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();

            var seattlePostalCode = "98117";
            var seattle = new Location
            {
                Id = 1,
                Address1 = "123 Main Street",
                Address2 = "Unit 2",
                City = "Seattle",
                PostalCode = seattlePostalCode,
                Country = "USA",
                State = "WA",
                Name = "Organizer name",
                PhoneNumber = "555-555-5555"
            };

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
            htb.Campaigns.Add(firePrev);

            var queenAnne = new Event
            {
                Id = 1,
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<EventSkill>(),
                EventType = EventType.Itinerary
            };

            var rallyEvent = new Event
            {
                Id = 2,
                Name = "Queen Anne Fire Prevention Day Rally",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = new Location { Id = 1 },
                RequiredSkills = new List<EventSkill>(),
                EventType = EventType.Rally
            };

            var task1 = new AllReadyTask
            {
                Id = 1,
                Event = rallyEvent,
                Name = "Task1",
                IsLimitVolunteers = false,
                StartDateTime = new DateTime(2016, 7, 5, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2016, 7, 5, 15, 0, 0).ToUniversalTime()
            };         

            var request1 = new Request
            {
                RequestId = Guid.NewGuid(),
                Name = "Request1",
                Status = RequestStatus.Assigned,
                Latitude = 50.768,
                Longitude = 0.2905
            };

            var request2 = new Request
            {
                RequestId = Guid.NewGuid(),
                Name = "Request2",
                Status = RequestStatus.Assigned,
                Latitude = 50.768,
                Longitude = 0.2905
            };

            var request3 = new Request
            {
                RequestId = Guid.NewGuid(),
                Name = "Request3",
                Status = RequestStatus.Assigned,
                Latitude = 50.768,
                Longitude = 0.2905
            };

            var itinerary1 = new Itinerary
            {
                Date = new DateTime(2015, 07, 5),
                Name = "Itinerary1",
                Id = 1,
                Event = queenAnne,
                EventId = 1
            };

            var itinerary2 = new Itinerary
            {
                Date = new DateTime(2016, 08, 01),
                Name = "Itinerary2",
                Id = 2,
                Event = queenAnne,
                EventId = 1
            };

            var itineraryReq1 = new ItineraryRequest
            {
                RequestId = request1.RequestId,
                ItineraryId = 1
            };

            var itineraryReq2 = new ItineraryRequest
            {
                RequestId = request2.RequestId,
                ItineraryId = 1
            };

            var itineraryReq3 = new ItineraryRequest
            {
                RequestId = request3.RequestId,
                ItineraryId = 2
            };

            var user1 = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "bgates@example.com"
            };

            var taskSignup = new TaskSignup
            {
                ItineraryId = 1,
                Itinerary = itinerary1,
                Task = task1,
            };

            context.Requests.AddRange(request1, request2, request3);
            context.Itineraries.AddRange(itinerary1, itinerary2);
            context.ItineraryRequests.AddRange(itineraryReq1, itineraryReq2, itineraryReq3);
            context.Locations.Add(seattle);
            context.Organizations.Add(htb);
            context.Events.Add(queenAnne);
            context.Events.Add(rallyEvent);
            context.Users.Add(user1);
            context.Tasks.Add(task1);
            context.TaskSignups.Add(taskSignup);

            context.SaveChanges();
        }

        [Fact]
        public async Task EventExists()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new EventDetailQuery { EventId = 1 };
            var handler = new EventDetailQueryHandler(context);
            var result = await handler.Handle(query);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task EventDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new EventDetailQuery();
            var handler = new EventDetailQueryHandler(context);
            var result = await handler.Handle(query);
            Assert.Null(result);
        }

        [Fact]
        public async Task EventIncludesAllLocationInformation()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new EventDetailQuery { EventId = 1 };
            var handler = new EventDetailQueryHandler(context);
            var result = await handler.Handle(query);

            Assert.NotNull(result.Location);
            Assert.NotNull(result.Location?.Id);
            Assert.NotNull(result.Location?.Address1);
            Assert.NotNull(result.Location?.Address2);
            Assert.NotNull(result.Location?.PostalCode);
            Assert.NotNull(result.Location?.State);
            Assert.NotNull(result.Location?.Name);
            Assert.NotNull(result.Location?.PhoneNumber);
            Assert.NotNull(result.Location?.Country);
        }

        [Fact]
        public async Task ItineraryEventIncludesCorrectItineraryDetails()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new EventDetailQuery { EventId = 1 };
            var handler = new EventDetailQueryHandler(context);
            var result = await handler.Handle(query);

            Assert.NotNull(result.Itineraries);
            Assert.Equal(2, result.Itineraries.Count());

            var firstItineraryResult = result.Itineraries.ToList()[0];

            Assert.Equal("Itinerary1", firstItineraryResult.Name);
            Assert.Equal(2, firstItineraryResult.RequestCount);
            Assert.Equal(1, firstItineraryResult.TeamSize);

            var secondItineraryResult = result.Itineraries.ToList()[1];

            Assert.Equal(1, secondItineraryResult.RequestCount);
        }
    }
}
