using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Models;
using AllReady.Providers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class EditCampaignCommandHandlerTests1 : TestBase
    {
        [Fact]
        public async Task ModelIsCreated()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var sut = new EditCampaignCommandHandler(context, Mock.Of<IConvertDateTimeOffset>());
            var actual = await sut.Handle(new EditCampaignCommand { Campaign = new CampaignSummaryViewModel { TimeZoneId = "Eastern Standard Time" } });
            Assert.NotEqual(0, actual);
        }

        [Fact]
        public async Task CampaignDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var vm = new CampaignSummaryViewModel
            {
                TimeZoneId = "Eastern Standard Time"
            };
            var query = new EditCampaignCommand { Campaign = vm };
            var handler = new EditCampaignCommandHandler(context, Mock.Of<IConvertDateTimeOffset>());
            var result = await handler.Handle(query);
            Assert.True(result > 0);

            var data = context.Campaigns.Count(_ => _.Id == result);
            Assert.True(data == 1);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task ExistingCampaign()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();

            var htb = new Organization
            {
                Name = "Humanitarian Toolbox",
                LogoUrl = "http://www.htbox.org/upload/home/ht-hero.png",
                WebUrl = "http://www.htbox.org",
                Campaigns = new List<Campaign>()
            };

            var firePrev = new Campaign
            {
                Name = "Neighborhood Fire Prevention Days",
                ManagingOrganization = htb,
                TimeZoneId = "Eastern Standard Time"
            };
            htb.Campaigns.Add(firePrev);
            context.Organizations.Add(htb);
            context.SaveChanges();

            const string newName = "Some new name value";

            var startDate = new DateTime(2014, 12, 10);
            var endDate = new DateTime(2015, 7, 3);
            var vm = new CampaignSummaryViewModel
            {
                Description = firePrev.Description,
                EndDate = endDate,
                FullDescription = firePrev.FullDescription,
                StartDate = startDate,
                Id = firePrev.Id,
                ImageUrl = firePrev.ImageUrl,
                Name = newName,
                OrganizationId = firePrev.ManagingOrganizationId,
                OrganizationName = firePrev.ManagingOrganization.Name,
                TimeZoneId = "Eastern Standard Time"
            };
            var query = new EditCampaignCommand { Campaign = vm };
            var handler = new EditCampaignCommandHandler(context, Mock.Of<IConvertDateTimeOffset>());
            var result = await handler.Handle(query);
            Assert.Equal(1, result); // should get back the Campaign id

            var data = context.Campaigns.Single(_ => _.Id == 1);
            Assert.Equal(newName, data.Name);

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