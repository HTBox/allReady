using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllReady.UnitTest.Campaigns
{
    public class EditCampaign : TestBase
    {
        [Fact]
        public void CampaignDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var vm = new CampaignSummaryModel
            {
                TimeZoneId = "Eastern Standard Time"
            };
            var query = new EditCampaignCommand { Campaign = vm };
            var handler = new EditCampaignCommandHandler(context);
            var result = handler.Handle(query);
            Assert.True(result > 0);

            var data = context.Campaigns.Count(_ => _.Id == result);
            Assert.True(data == 1);
        }

        [Fact]
        public void ExistingCampaign()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            Tenant htb = new Tenant()
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };
            Campaign firePrev = new Campaign()
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingTenant = htb,
                TimeZoneId = "Eastern Standard Time"
            };
            htb.Campaigns.Add(firePrev);
            context.Tenants.Add(htb);
            context.SaveChanges();

            const string NEW_NAME = "Some new name value";

            var vm = new CampaignSummaryModel
            {
                Description = firePrev.Description,
                EndDate = firePrev.EndDateTime,
                FullDescription = firePrev.FullDescription,
                StartDate = firePrev.StartDateTime,
                Id = firePrev.Id,
                ImageUrl = firePrev.ImageUrl,
                Name = NEW_NAME,
                TenantId = firePrev.ManagingTenantId,
                TenantName = firePrev.ManagingTenant.Name,
                TimeZoneId = "Eastern Standard Time"
            };
            var query = new EditCampaignCommand { Campaign = vm };
            var handler = new EditCampaignCommandHandler(context);
            var result = handler.Handle(query);
            Assert.Equal(1, result); // should get back the Campaign id

            var data = context.Campaigns.Single(_ => _.Id == 1);
            Assert.Equal(NEW_NAME, data.Name);
        }
    }
}
