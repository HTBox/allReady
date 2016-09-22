using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class DeleteQueryHandlerAsyncShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnTheCorrectVieModel()
        {
            const int campaignId = 1;

            Context.Campaigns.Add(new Campaign { Id = campaignId, ManagingOrganization = new Organization() });
            Context.SaveChanges();

            var message = new DeleteQueryAsync { CampaignId = campaignId };

            var sut = new DeleteQueryHandlerAsync(Context);
            var result = await sut.Handle(message);

            Assert.IsType<DeleteViewModel>(result);
        }

        [Fact]
        public void ReturnTheCorrectData()
        {

        }
    }
}
