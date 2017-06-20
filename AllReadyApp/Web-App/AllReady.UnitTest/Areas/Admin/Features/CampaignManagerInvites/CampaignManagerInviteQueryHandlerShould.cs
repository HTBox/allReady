using AllReady.Areas.Admin.Features.CampaignManagerInvites;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.CampaignManagerInvites
{
    public class CampaignManagerInviteQueryHandlerShould : InMemoryContextTest
    {
        private const int orgId = 3;
        private const string campaignName = "testCampaign";
        private const int campaignId = 55;

        protected override void LoadTestData()
        {
            Context.Campaigns.Add(new AllReady.Models.Campaign
            {
                Id = campaignId,
                Name = campaignName,
                ManagingOrganizationId = orgId
            });

            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnCampaignInviteViewModel()
        {
            var handler = new CampaignManagerInviteQueryHandler(Context);

            var result = await handler.Handle(new CampaignManagerInviteQuery { CampaignId = campaignId });

            result.CampaignId.ShouldBe(campaignId);
            result.CampaignName.ShouldBe(campaignName);
            result.OrganizationId.ShouldBe(orgId);
        }
    }
}
