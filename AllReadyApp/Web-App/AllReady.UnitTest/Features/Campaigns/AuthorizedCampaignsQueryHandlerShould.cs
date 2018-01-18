using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Campaigns;
using AllReady.Models;
using AllReady.ViewModels.Campaign;
using Xunit;

namespace AllReady.UnitTest.Features.Campaigns
{
    public class AuthorizedCampaignsQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task GivenACampaignManagerUserId_ReturnsTheCorrectData()
        {
            var campaigns = new List<Campaign>
            {
                new Campaign
                {
                    Id = 1,
                    Locked = false,
                    Published = true,
                    ManagingOrganization = new Organization(),
                    CampaignManagers = new List<CampaignManager> {new CampaignManager {CampaignId = 1, UserId = "Eroica"}}
                },
                new Campaign
                {
                    Id = 2,
                    Locked = true,
                    Published = true,
                    ManagingOrganization = new Organization(),
                    CampaignManagers = new List<CampaignManager> {new CampaignManager {CampaignId = 2, UserId = "Eroica"}}
                }
            };

            Context.Campaigns.AddRange(campaigns);
            Context.SaveChanges();

            var sut = new AuthorizedCampaignsQueryHandler(Context);
            var results = await sut.Handle(new AuthorizedCampaignsQuery{ UserId = "Eroica" });

            Assert.Equal(campaigns[0].Id, results[0].Id);
            Assert.Contains(campaigns[0].CampaignManagers, c =>c.UserId == "Eroica");
        }

        [Fact]
        public async Task ReturnTheCorrectViewModel()
        {
            var sut = new AuthorizedCampaignsQueryHandler(Context);
            var results = await sut.Handle(new AuthorizedCampaignsQuery{ UserId = "Eroica" });

            Assert.IsType<List<ManageCampaignViewModel>>(results);
        }
    }
}
