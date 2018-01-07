using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.TeamLead;
using AllReady.Areas.Admin.ViewModels.TeamLead;
using AllReady.Models;
using Moq;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Features.TeamLead
{
    public class TeamLeadItineraryListViewModelQueryHandlerTests : InMemoryContextTest
    {
        [Fact]
        public async Task Handle_ReturnsEmptyViewModel_WhenUserIsNotFound()
        {
            // arrange
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((ApplicationUser)null);

            var sut = new TeamLeadItineraryListViewModelQueryHandler(Context, userManager.Object);

            // act
            var result = await sut.Handle(new TeamLeadItineraryListViewModelQuery(new ClaimsPrincipal()));
            
            // assert
            result.ShouldBeOfType<TeamLeadItineraryListerViewModel>().HasItineraries.ShouldBeFalse();
        }

        [Fact]
        public async Task Handle_ReturnsCorrectViewModel_WhenUserIsFound()
        {
            // arrange
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);

            var sut = new TeamLeadItineraryListViewModelQueryHandler(Context, userManager.Object);

            // act
            var result = await sut.Handle(new TeamLeadItineraryListViewModelQuery(new ClaimsPrincipal()));

            // assert
            result.ShouldBeOfType<TeamLeadItineraryListerViewModel>();
            result.Campaigns.Count.ShouldBe(2);
            result.Campaigns[0].CampaignEvents.Count.ShouldBe(1);
            result.Campaigns[1].CampaignEvents.Count.ShouldBe(1);
            result.Campaigns[0].CampaignEvents[0].Itineraries.Count.ShouldBe(2);
        }

        private ApplicationUser _user;

        protected override void LoadTestData()
        {
            var user = new ApplicationUser();

            var org = new Organization
            {
                Name = "Test Org"
            };

            var campaign1 = new Campaign
            {
                Name = "Campaign 1",
                ManagingOrganization = org
            };

            var campaign2 = new Campaign
            {
                Name = "Campaign 2",
                ManagingOrganization = org
            };

            var campaignEvent1 = new AllReady.Models.Event
            {
                Name = "Event 1",
                Campaign = campaign1,
                TimeZoneId = "enGB"
            };

            var campaignEvent2 = new AllReady.Models.Event
            {
                Name = "Event 2",
                Campaign = campaign2,
                TimeZoneId = "enGB"
            };

            var itinerary1 = new Itinerary
            {
                Name = "Itinerary 1",
                Event = campaignEvent1,
                Date = DateTime.UtcNow
            };

            var itinerary2 = new Itinerary
            {
                Name = "Itinerary 2",
                Event = campaignEvent1,
                Date = DateTime.UtcNow
            };

            var itinerary3 = new Itinerary
            {
                Name = "Itinerary 3",
                Event = campaignEvent2,
                Date = DateTime.UtcNow
            };

            var taskSignup1 = new VolunteerTaskSignup
            {
                User = user,
                Itinerary = itinerary1,
                IsTeamLead = true
            };

            var taskSignup2 = new VolunteerTaskSignup
            {
                User = user,
                Itinerary = itinerary2,
                IsTeamLead = true
            };

            var taskSignup3 = new VolunteerTaskSignup
            {
                User = user,
                Itinerary = itinerary3,
                IsTeamLead = true
            };

            Context.VolunteerTaskSignups.Add(taskSignup1);
            Context.VolunteerTaskSignups.Add(taskSignup2);
            Context.VolunteerTaskSignups.Add(taskSignup3);

            Context.SaveChanges();

            _user = user;
        }
    }
}
