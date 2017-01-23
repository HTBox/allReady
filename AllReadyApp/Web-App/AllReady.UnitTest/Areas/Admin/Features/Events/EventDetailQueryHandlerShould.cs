using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Events
{
    public class EventDetailQueryHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var seattlePostalCode = "98117";
            var seattle = new Location
            {
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
                Name = "Queen Anne Fire Prevention Day",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = seattle,
                RequiredSkills = new List<EventSkill>(),
                EventType = EventType.Itinerary
            };

            var rallyEvent = new Event
            {
                Name = "Queen Anne Fire Prevention Day Rally",
                Campaign = firePrev,
                CampaignId = firePrev.Id,
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 12, 31, 15, 0, 0).ToUniversalTime(),
                Location = seattle,
                RequiredSkills = new List<EventSkill>(),
                EventType = EventType.Rally
            };

            var task1 = new VolunteerTask
            {
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
                Event = queenAnne
            };

            var itinerary2 = new Itinerary
            {
                Date = new DateTime(2016, 08, 01),
                Name = "Itinerary2",
                Event = queenAnne
            };

            var itineraryReq1 = new ItineraryRequest
            {
                RequestId = request1.RequestId,
                Itinerary = itinerary1
            };

            var itineraryReq2 = new ItineraryRequest
            {
                RequestId = request2.RequestId,
                Itinerary = itinerary1
            };

            var itineraryReq3 = new ItineraryRequest
            {
                RequestId = request3.RequestId,
                Itinerary = itinerary2
            };

            var user1 = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "bgates@example.com"
            };

            var taskSignup = new VolunteerTaskSignup
            {
                Itinerary = itinerary1,
                VolunteerTask = task1,
            };
            Context.Locations.Add(seattle);
            Context.Requests.AddRange(request1, request2, request3);
            Context.Itineraries.AddRange(itinerary1, itinerary2);
            Context.ItineraryRequests.AddRange(itineraryReq1, itineraryReq2, itineraryReq3);
            Context.Organizations.Add(htb);
            Context.Events.Add(queenAnne);
            Context.Events.Add(rallyEvent);
            Context.Users.Add(user1);
            Context.Tasks.Add(task1);
            Context.TaskSignups.Add(taskSignup);

            Context.SaveChanges();
        }

        [Fact]
        public async Task EventExists()
        {
            var query = new EventDetailQuery { EventId = 1 };
            var handler = new EventDetailQueryHandler(Context);
            var result = await handler.Handle(query);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task EventDoesNotExist()
        {
            var query = new EventDetailQuery();
            var handler = new EventDetailQueryHandler(Context);
            var result = await handler.Handle(query);
            Assert.Null(result);
        }

        [Fact]
        public async Task EventIncludesAllLocationInformation()
        {
            var query = new EventDetailQuery { EventId = 1 };
            var handler = new EventDetailQueryHandler(Context);
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
            var query = new EventDetailQuery { EventId = 1 };
            var handler = new EventDetailQueryHandler(Context);
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