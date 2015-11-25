using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
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

            var startDate = new DateTime(2014, 12, 10);
            var endDate = new DateTime(2015, 7, 3);
            var vm = new CampaignSummaryModel
            {
                Description = firePrev.Description,
                EndDate = endDate,
                FullDescription = firePrev.FullDescription,
                StartDate = startDate,
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

            Assert.Equal(2014, data.StartDateTime.Year);
            Assert.Equal(12, data.StartDateTime.Month);
            Assert.Equal(10, data.StartDateTime.Day);
            Assert.Equal(00, data.StartDateTime.Hour);
            Assert.Equal(00, data.StartDateTime.Minute);
            Assert.Equal(-5, data.StartDateTime.Offset.TotalHours);

            Assert.Equal(2015, data.EndDateTime.Year);
            Assert.Equal(7, data.EndDateTime.Month);
            Assert.Equal(3, data.EndDateTime.Day);
            Assert.Equal(23, data.EndDateTime.Hour);
            Assert.Equal(59, data.EndDateTime.Minute);
            Assert.Equal(-4, data.EndDateTime.Offset.TotalHours);
            
        }
    }
}
