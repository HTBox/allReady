using System.Threading.Tasks;
using AllReady.Features.Campaigns;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Features.Campaigns
{
    public class CampaignByCampaignIdQueryHandlerShould : InMemoryContextTest
    {
        private readonly CampaignByCampaignIdQuery message;
        private readonly Campaign campaign;
        private readonly CampaignByCampaignIdQueryHandler sut;

        public CampaignByCampaignIdQueryHandlerShould()
        {
            message = new CampaignByCampaignIdQuery { CampaignId = 1 };
            campaign = new Campaign { Id = message.CampaignId, Published = true };

            Context.Add(campaign);
            Context.SaveChanges();

            sut = new CampaignByCampaignIdQueryHandler(Context);
        }

        [Fact]
        public async Task ReturnCorrectData()
        {
            var result = await sut.Handle(message);
            Assert.Same(campaign, result);
        }

        [Fact]
        public async Task ReturnCorrectType()
        {
            var result = await sut.Handle(message);
            Assert.IsType<Campaign>(result);
        }
    }
}