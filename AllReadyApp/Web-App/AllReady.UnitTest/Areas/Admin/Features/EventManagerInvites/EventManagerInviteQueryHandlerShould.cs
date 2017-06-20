using AllReady.Areas.Admin.Features.EventManagerInvites;
using AllReady.Models;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.EventManagerInvites
{
    public class EventManagerInviteQueryHandlerShould : InMemoryContextTest
    {
        private const int eventId = 5;
        private const string eventName = "testEvent";
        private const int campaignId = 55;
        private const string campaignName = "testCampaign";
        private const int organizationId = 33;

        protected override void LoadTestData()
        {
            Context.Events.Add(new Event
            {
                Id = eventId,
                Name = eventName,
                CampaignId = campaignId
            });

            Context.Campaigns.Add(new Campaign
            {
                Id = campaignId,
                Name = campaignName,
                ManagingOrganizationId = organizationId
            });
            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnViewModel()
        {
            var handler = new EventManagerInviteQueryHandler(Context);
            var viewModel = await handler.Handle(new EventManagerInviteQuery { EventId = eventId });
            viewModel.CampaignId.ShouldBe(campaignId);
            viewModel.CampaignName.ShouldBe(campaignName);
            viewModel.EventId.ShouldBe(eventId);
            viewModel.EventName.ShouldBe(eventName);
            viewModel.OrganizationId.ShouldBe(organizationId);
        }
    }
}
