using AllReady.Areas.Admin.Features.EventManagerInvites;
using AllReady.Models;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.EventManagerInvites
{
    public class AcceptDeclineEventManagerInviteQueryHandlerShould : InMemoryContextTest
    {
        private int inviteId = 100;
        private int eventId = 200;
        private int campaignId = 300;
        private string inviteeEmail = "test@test.com";
        private string eventName = "My event";
        private string campaignName = "My campaign";

        protected override void LoadTestData()
        {
            Context.EventManagerInvites.Add(new EventManagerInvite
            {
                Id = inviteId,
                EventId = eventId,
                InviteeEmailAddress = inviteeEmail
            });

            Context.Events.Add(new Event
            {
                Id = eventId,
                Name = eventName,
                CampaignId = campaignId
            });

            Context.Campaigns.Add(new Campaign
            {
                Id = campaignId,
                Name = campaignName
            });

            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnViewModel()
        {
            var handler = new AcceptDeclineEventManagerInviteDetailQueryHandler(Context);
            var viewModel = await handler.Handle(new AcceptDeclineEventManagerInviteDetailQuery { EventManagerInviteId = inviteId });
            viewModel.CampaignName.ShouldBe(campaignName);
            viewModel.EventName.ShouldBe(eventName);
            viewModel.InviteeEmailAddress.ShouldBe(inviteeEmail);
            viewModel.InviteId.ShouldBe(inviteId);
            viewModel.EventId.ShouldBe(eventId);
        }
    }
}
