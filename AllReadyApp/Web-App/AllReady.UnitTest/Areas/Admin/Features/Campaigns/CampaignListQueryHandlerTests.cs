using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Models;
using AllReady.UnitTest.Features.Campaigns;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class CampaignListQueryHandlerTests : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();

            var htb = new Organization
            {
                Id = 1,
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>(),                
            };

            var other = new Organization
            {
                Id = 2,
                Name = "Other Org",
                Campaigns = new List<Campaign>(),
            };

            var firePrev = new Campaign
            {
                Id = 1,
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb
            };

            var otherCampaign = new Campaign
            {
                Id = 2,
                Name = "Some other campaign",
                ManagingOrganization = other
            };

            htb.Campaigns.Add(firePrev);
            context.Organizations.Add(htb);
            other.Campaigns.Add(otherCampaign);
            context.Organizations.Add(other);
            context.SaveChanges();
        }

        [Fact]
        public void GetCampaignsWithoutOrgIdSet()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignListQuery();
            var handler = new CampaignListQueryHandler(context);
            var result = handler.Handle(query);
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.Count(c => c.Id == 1));
            Assert.Equal(1, result.Count(c => c.Id == 2));
        }

        [Fact]
        public void GetCampaignsWithOrgIdSet()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new CampaignListQuery();
            query.OrganizationId = 1;
            var handler = new CampaignListQueryHandler(context);
            var result = handler.Handle(query);
            Assert.Equal(1, result.Count());
            Assert.Equal(1, result.Count(c => c.Id == 1));
            Assert.Equal(0, result.Count(c => c.Id == 2));
        }
    }
}
