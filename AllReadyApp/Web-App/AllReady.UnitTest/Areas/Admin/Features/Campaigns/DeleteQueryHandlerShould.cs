using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class DeleteQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnTheCorrectVieModel()
        {
            const int campaignId = 1;

            Context.Campaigns.Add(new Campaign { Id = campaignId, ManagingOrganization = new Organization() });
            Context.SaveChanges();

            var message = new DeleteViewModelQuery { CampaignId = campaignId };

            var sut = new DeleteViewModelQueryHandler(Context);
            var result = await sut.Handle(message);

            Assert.IsType<DeleteViewModel>(result);
        }

        [Fact]
        public async Task ReturnTheCorrectData()
        {
            var campaign1 = new Campaign { Id = 1, Name = "Campaign1Name", Description = "Campaign1Description",
                ManagingOrganizationId = 1, ManagingOrganization = new Organization { Id = 1, Name = "ManagingOrgName "}};

            Context.Campaigns.Add(campaign1);
            Context.Campaigns.Add(new Campaign { Id = 2, ManagingOrganization = new Organization { Id = 2 }});
            Context.SaveChanges();

            var message = new DeleteViewModelQuery { CampaignId = campaign1.Id };

            var sut = new DeleteViewModelQueryHandler(Context);
            var result = await sut.Handle(message);

            Assert.Equal(result.Id, campaign1.Id);
            Assert.Equal(result.Name, campaign1.Name);
            Assert.Equal(result.Description, campaign1.Description);
            Assert.Equal(result.OrganizationId, campaign1.ManagingOrganization.Id);
            Assert.Equal(result.OrganizationName, campaign1.ManagingOrganization.Name);
        }
    }
}