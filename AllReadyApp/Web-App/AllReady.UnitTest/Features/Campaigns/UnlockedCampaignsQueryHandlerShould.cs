using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Features.Campaigns;
using AllReady.Models;
using AllReady.ViewModels.Campaign;
using Xunit;

namespace AllReady.UnitTest.Features.Campaigns
{
    public class UnlockedCampaignsQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnTheCorrectData()
        {
            var campaigns = new List<Campaign>
            {
                new Campaign { Id = 1, Locked = false, Published = true, ManagingOrganization = new Organization() },
                new Campaign { Id = 2, Locked = true, Published = true, ManagingOrganization = new Organization() }
            };

            Context.Campaigns.AddRange(campaigns);
            Context.SaveChanges();

            var sut = new UnlockedCampaignsQueryHandler(Context);
            var results = await sut.Handle(new UnlockedCampaignsQuery());

            Assert.Equal(campaigns[0].Id, results[0].Id);
        }

        [Fact]
        public async Task ReturnTheCorrectViewModel()
        {
            var sut = new UnlockedCampaignsQueryHandler(Context);
            var results = await sut.Handle(new UnlockedCampaignsQuery());

            Assert.IsType<List<CampaignViewModel>>(results);
        }
    }
}