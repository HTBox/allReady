using System.Collections.Generic;
using AllReady.Features.Campaigns;
using AllReady.Models;
using AllReady.ViewModels.Campaign;
using Xunit;

namespace AllReady.UnitTest.Features.Campaigns
{
    public class UnlockedCampaignsQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public void ReturnTheCorrectData()
        {
            var campaigns = new List<Campaign>
            {
                new Campaign { Id = 1, Locked = false, ManagingOrganization = new Organization() },
                new Campaign { Id = 2, Locked = true, ManagingOrganization = new Organization() }
            };

            Context.Campaigns.AddRange(campaigns);
            Context.SaveChanges();

            var sut = new UnlockedCampaignsQueryHandler(Context);
            var results = sut.Handle(new UnlockedCampaignsQuery());

            Assert.Equal(campaigns[0].Id, results[0].Id);
        }

        [Fact]
        public void ReturnTheCorrectViewModel()
        {
            var sut = new UnlockedCampaignsQueryHandler(Context);
            var results = sut.Handle(new UnlockedCampaignsQuery());

            Assert.IsType<List<CampaignViewModel>>(results);
        }
    }
}