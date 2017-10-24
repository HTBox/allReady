using AllReady.Areas.Admin.Features.CampaignManagerInvites;
using AllReady.Models;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.CampaignManagerInvites
{
    public class AcceptDeclineCampaignManagerInviteDetailQueryHandlerShould : InMemoryContextTest
    {
        private int inviteId = 100;
        private int campaignId = 300;
        private string inviteeEmail = "test@test.com";
        private string campaignName = "My campaign";

        protected override void LoadTestData()
        {
            Context.CampaignManagerInvites.Add(new CampaignManagerInvite
            {
                Id = inviteId,
                CampaignId = campaignId,
                InviteeEmailAddress = inviteeEmail
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
            var handler = new AcceptDeclineCampaignManagerInviteDetailQueryHandler(Context);
            var viewModel = await handler.Handle(new AcceptDeclineCampaignManagerInviteDetailQuery { CampaignManagerInviteId = inviteId });
            viewModel.CampaignName.ShouldBe(campaignName);
            viewModel.InviteeEmailAddress.ShouldBe(inviteeEmail);
            viewModel.InviteId.ShouldBe(inviteId);
            viewModel.CampaignId.ShouldBe(campaignId);
        }
    }
}
