using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Xunit;
using System.Linq;

namespace AllReady.UnitTest.Areas.Admin.Features.Itineraries
{
    public class AddTeamMemberCommandHandlerShould : InMemoryContextTest
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

            var volunteerTaskSignUp = new VolunteerTaskSignup { Id = 1, ItineraryId = 2, VolunteerTaskId = 1 };

            Context.Organizations.Add(htb);
            Context.Campaigns.Add(firePrev);
            Context.Events.Add(queenAnne);
            Context.Itineraries.Add(itinerary);
            Context.VolunteerTaskSignups.Add(volunteerTaskSignUp);

            Context.SaveChanges();
        }

        [Fact]
        public async Task AddTeamMemberCommandHandlerReturnsFalseWhenItineraryDoesNotExist()
        {
            var query = new AddTeamMemberCommand
            {
                ItineraryId = 0,
                VolunteerTaskSignupId = 1
            };

            var handler = new AddTeamMemberCommandHandler(Context, null);
            var result = await handler.Handle(query);

            Assert.False(result);
        }

        [Fact]
        public async Task AddTeamMemberCommandHandlerReturnsTrueWhenItineraryExists()
        {
            var query = new AddTeamMemberCommand
            {
                ItineraryId = 1,
                VolunteerTaskSignupId = 1
            };

            var handler = new AddTeamMemberCommandHandler(Context, Mock.Of<IMediator>());
            var result = await handler.Handle(query);

            Assert.True(result);
        }

        [Fact]
        public async Task AddTeamMemberCommandHandlerSendsPotentialItineraryTeamMemberQueryWithCorrectEventId()
        {
            var query = new AddTeamMemberCommand
            {
                ItineraryId = 1,
                VolunteerTaskSignupId = 1
            };

            var mockMediator = new Mock<IMediator>();
            var handler = new AddTeamMemberCommandHandler(Context, mockMediator.Object);

            await handler.Handle(query);

            mockMediator.Verify(x => x.SendAsync(It.Is<PotentialItineraryTeamMembersQuery>(y => y.EventId == 1)), Times.Once);
        }

        [Fact]
        public async Task AddTeamMemberCommandHandlerPublishesItineraryVolunteerListUpdatedWhenMatchedOnTaskSignupId()
        {
            var query = new AddTeamMemberCommand
            {
                ItineraryId = 1,
                VolunteerTaskSignupId = 1
            };

            var potentialTaskSignups = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "user@domain.tld : Test TaskName",
                    Value = query.VolunteerTaskSignupId.ToString()
                }
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<PotentialItineraryTeamMembersQuery>())).ReturnsAsync(potentialTaskSignups);

            var volunteerTaskSignUp = Context.VolunteerTaskSignups.Single(t => t.Id == 1);
            Context.Entry(volunteerTaskSignUp).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var handler = new AddTeamMemberCommandHandler(Context, mockMediator.Object);
            await handler.Handle(query);

            mockMediator.Verify(x => x.PublishAsync(It.Is<ItineraryVolunteerListUpdated>(y => y.VolunteerTaskSignupId == query.VolunteerTaskSignupId && y.ItineraryId == query.ItineraryId && y.UpdateType == UpdateType.VolunteerAssigned)), Times.Once);
        }
    }
}
