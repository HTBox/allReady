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
        private int htb_id;
        private int other_id;
        private int firePrev_id;
        private int otherCampaign_id;
        public Organization Htb()
        {
            return new Organization {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>(),
            };
        }
        public Organization Other()
        {
            return new Organization
            {
                Name = "Other Org",
                Campaigns = new List<Campaign>(),
            };
        }
        public Campaign FirePrev()
        {
            return new Campaign
            {
                Name = "Neighborhood Fire Prevention Days",
            };
        }
        public Campaign OtherCampaign()
        {
            return new Campaign
            {
                Name = "Some other campaign",
            };
        }
        protected override void LoadTestData()
        {
            var htb = Htb();
            var other = Other();
            var firePrev = FirePrev();
            var otherCampaign = OtherCampaign();

            Context.Organizations.Add(htb);
            Context.Organizations.Add(other);
            Context.SaveChanges();
            htb_id = htb.Id;
            other_id = other.Id;
            firePrev.ManagingOrganization = htb;
            otherCampaign.ManagingOrganization = other;
            Context.Campaigns.Add(firePrev);
            Context.Campaigns.Add(otherCampaign);
            Context.SaveChanges();
            firePrev_id = firePrev.Id;
            otherCampaign_id = otherCampaign.Id;
        }

        [Fact(Skip = "RTM Broken Tests")]
        public void GetCampaignsWithoutOrgIdSet()
        {
            var query = new CampaignListQuery();
            var handler = new CampaignListQueryHandler(Context);
            var result = handler.Handle(query);
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.Count(c => c.Id == firePrev_id));
            Assert.Equal(1, result.Count(c => c.Id == otherCampaign_id));
        }

        [Fact(Skip = "RTM Broken Tests")]
        public void GetCampaignsWithOrgIdSet()
        {
            var query = new CampaignListQuery();
            query.OrganizationId = firePrev_id;
            var handler = new CampaignListQueryHandler(Context);
            var result = handler.Handle(query);
            Assert.Equal(1, result.Count());
            Assert.Equal(1, result.Count(c => c.Id == firePrev_id));
            Assert.Equal(0, result.Count(c => c.Id == otherCampaign_id));
            Assert.Equal(result.First().OrganizationName, "Humanitarian Toolbox");
        }
    }
}
