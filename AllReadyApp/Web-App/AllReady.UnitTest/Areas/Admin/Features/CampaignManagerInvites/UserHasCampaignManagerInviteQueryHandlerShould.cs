using AllReady.Areas.Admin.Features.CampaignManagerInvites;
using AllReady.Models;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.CampaignManagerInvites
{
    public class UserHasCampaignManagerInviteQueryHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var campaignManagerInvite = new CampaignManagerInvite
            {
                Id = 100,
                InviteeEmailAddress = "test@test.com",
                CustomMessage = "test",
                CampaignId = 5,
            };

            Context.Campaigns.Add(new Campaign
            {
                Id = 5,
                Name = "testCampaign",
            });

            Context.CampaignManagerInvites.Add(campaignManagerInvite);
            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnFalse_WhenInviteDoesNotExist()
        {
            var handler = new UserHasCampaignManagerInviteQueryHandler(Context);
            bool result = await handler.Handle(new UserHasCampaignManagerInviteQuery { InviteeEmail = "invalid@email.com", CampaignId = 5 });
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task ReturnTrue_WhenInviteExists()
        {
            var handler = new UserHasCampaignManagerInviteQueryHandler(Context);
            bool result = await handler.Handle(new UserHasCampaignManagerInviteQuery { InviteeEmail = "test@test.com", CampaignId = 5 });
            result.ShouldBeTrue();
        }
    }
}
