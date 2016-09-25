using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Models;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Itineraries
{
    public class ItineraryDetailQueryHandlerAsyncTests : InMemoryContextTest
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

            var itinerary = new Itinerary
            {
                Event = queenAnne,
                Name = "1st Itinerary",
                Id = 1,
                Date = new DateTime(2016, 07, 01)
            };

            var task = new AllReadyTask
            {
                Id = 1,
                Event = queenAnne,
                Name = "A Task",
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 7, 4, 18, 0, 0).ToUniversalTime()
            };

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "text@example.com"
            };

            var taskSignup = new TaskSignup
            {
                Id = 1,
                User = user,
                Task = task,
                Itinerary = itinerary
            };

            var request = new Request
            {
                RequestId = Guid.NewGuid(),
                Name = "Request 1",
                Address = "Street Name 1",
                City = "Seattle"
            };

            var itineraryReq = new ItineraryRequest
            {
                Request = request,
                Itinerary = itinerary
            };
            
            Context.Organizations.Add(htb);
            Context.Campaigns.Add(firePrev);
            Context.Events.Add(queenAnne);
            Context.Itineraries.Add(itinerary);
            Context.Tasks.Add(task);
            Context.Users.Add(user);
            Context.Requests.Add(request);
            Context.ItineraryRequests.Add(itineraryReq);
            Context.TaskSignups.Add(taskSignup);
            Context.SaveChanges();
        }

        [Fact]
        public async Task ItineraryExists_ReturnsItinerary()
        {           
            var query = new ItineraryDetailQuery { ItineraryId = 1 };
            var handler = new ItineraryDetailQueryHandler(Context, Mock.Of<IMediator>());
            var result = await handler.Handle(query);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ItineraryDoesNotExist_ReturnsNull()
        {        
            var query = new ItineraryDetailQuery();
            var handler = new ItineraryDetailQueryHandler(Context, Mock.Of<IMediator>());
            var result = await handler.Handle(query);
            Assert.Null(result);
        }
        
        [Fact]
        public async Task ItineraryQueryLoadsItineraryDetails()
        {
            var query = new ItineraryDetailQuery { ItineraryId = 1 };
            var handler = new ItineraryDetailQueryHandler(Context, Mock.Of<IMediator>());
            var result = await handler.Handle(query);
            Assert.Equal(1, result.Id);
            Assert.Equal("1st Itinerary", result.Name);
            Assert.Equal(new DateTime(2016, 07, 01), result.Date);
        }

        [Fact]
        public async Task ItineraryQueryLoadsEventDetails()
        {
            var query = new ItineraryDetailQuery { ItineraryId = 1 };
            var handler = new ItineraryDetailQueryHandler(Context, Mock.Of<IMediator>());
            var result = await handler.Handle(query);
            Assert.Equal("Queen Anne Fire Prevention Day", result.EventName);
        }

        [Fact]
        public async Task ItineraryQueryLoadsOrganizationId()
        {
            var query = new ItineraryDetailQuery { ItineraryId = 1 };
            var handler = new ItineraryDetailQueryHandler(Context, Mock.Of<IMediator>());
            var result = await handler.Handle(query);
            Assert.Equal(1, result.OrganizationId);
        }

        [Fact]
        public async Task ItineraryQueryLoadsCampaignName()
        {
            var query = new ItineraryDetailQuery { ItineraryId = 1 };
            var handler = new ItineraryDetailQueryHandler(Context, Mock.Of<IMediator>());
            var result = await handler.Handle(query);
            Assert.Equal("Neighborhood Fire Prevention Days", result.CampaignName);
        }

        [Fact]
        public async Task ItineraryQueryLoadsTeamMembers()
        {
            var query = new ItineraryDetailQuery { ItineraryId = 1 };
            var handler = new ItineraryDetailQueryHandler(Context, Mock.Of<IMediator>());
            var result = await handler.Handle(query);
            Assert.Equal(1, result.TeamMembers.Count);
            Assert.Equal("text@example.com", result.TeamMembers[0].VolunteerEmail);
        }

        [Fact]
        public async Task ItineraryQueryLoadsRequests()
        {
            var query = new ItineraryDetailQuery { ItineraryId = 1 };
            var handler = new ItineraryDetailQueryHandler(Context, Mock.Of<IMediator>());
            var result = await handler.Handle(query);
            Assert.Equal(1, result.Requests.Count);
            Assert.Equal("Request 1", result.Requests[0].Name);
        }
    }
}
