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
    public class ItineraryDetailQueryHandlerShould : InMemoryContextTest
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
                Date = new DateTime(2016, 07, 01),
                StartLocation = new Location{ Id = 1 },
                EndLocation = new Location {  Id = 2}
            };

            var volunteerTask = new VolunteerTask
            {
                Id = 1,
                Event = queenAnne,
                Name = "A Task",
                StartDateTime = new DateTime(2015, 7, 4, 10, 0, 0).ToUniversalTime(),
                EndDateTime = new DateTime(2015, 7, 4, 18, 0, 0).ToUniversalTime()
            };

            var user1 = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "Smith@example.com",
                LastName = "Smith",
                FirstName = "Bob"
            };

            var user2 = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "Jones@example.com",
                LastName = "Jones",
                FirstName = "Carol"
            };

            var user3 = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "Gordon@example.com",
                LastName = "Gordon",
                FirstName = "Steve"
            };

            var volunteerTaskSignup1 = new VolunteerTaskSignup
            {
                Id = 1,
                User = user1,
                VolunteerTask = volunteerTask,
                Itinerary = itinerary,
                IsTeamLead = true
            };

            var volunteerTaskSignup2 = new VolunteerTaskSignup
            {
                Id = 2,
                User = user2,
                VolunteerTask = volunteerTask,
                Itinerary = itinerary
            };

            var volunteerTaskSignup3 = new VolunteerTaskSignup
            {
                Id = 3,
                User = user3,
                VolunteerTask = volunteerTask,
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
            Context.VolunteerTasks.Add(volunteerTask);
            Context.Users.Add(user1);
            Context.Users.Add(user2);
            Context.Users.Add(user3);
            Context.Requests.Add(request);
            Context.ItineraryRequests.Add(itineraryReq);
            Context.VolunteerTaskSignups.Add(volunteerTaskSignup1);
            Context.VolunteerTaskSignups.Add(volunteerTaskSignup2);
            Context.VolunteerTaskSignups.Add(volunteerTaskSignup3);
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
            Assert.Equal(3, result.TeamMembers.Count);
            // Team lead is first
            Assert.True(result.TeamMembers[0].IsTeamLead);
            // then ordered by LastName and FirstName
            Assert.Equal("Gordon, Steve", result.TeamMembers[1].DisplayName);
            Assert.Equal("Jones, Carol", result.TeamMembers[2].DisplayName);
        }

        [Fact]
        public async Task ItineraryQueryLoadsRequests()
        {
            var query = new ItineraryDetailQuery { ItineraryId = 1 };
            var handler = new ItineraryDetailQueryHandler(Context, Mock.Of<IMediator>());
            var result = await handler.Handle(query);
            Assert.Single(result.Requests);
            Assert.Equal("Request 1", result.Requests[0].Name);
        }
    }
}
