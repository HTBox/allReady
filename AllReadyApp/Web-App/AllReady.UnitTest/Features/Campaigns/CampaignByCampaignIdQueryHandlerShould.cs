using System.Threading.Tasks;
using AllReady.Features.Campaigns;
using AllReady.Models;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Features.Campaigns
{
    public class CampaignByCampaignIdQueryHandlerShould : InMemoryContextTest
    {
        private readonly CampaignByCampaignIdQuery _message;
        private const int CampaignId = 1;
        private readonly CampaignByCampaignIdQueryHandler _sut;

        public CampaignByCampaignIdQueryHandlerShould()
        {
            _sut = new CampaignByCampaignIdQueryHandler(Context);
            _message = new CampaignByCampaignIdQuery { CampaignId = CampaignId };
        }

        protected override void LoadTestData()
        {
            var campaign = new Campaign { Id = CampaignId, Published = true, ManagingOrganization = new Organization(), Location = new Location()};

            Context.Add(campaign);
            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnCorrectData()
        {
            var result = await _sut.Handle(_message);

            result.ShouldNotBeNull();
            result.Id.ShouldBe(CampaignId);
        }

        [Fact]
        public async Task ReturnCorrectType()
        {
            var result = await _sut.Handle(_message);
            Assert.IsType<Campaign>(result);
        }
    }
}
