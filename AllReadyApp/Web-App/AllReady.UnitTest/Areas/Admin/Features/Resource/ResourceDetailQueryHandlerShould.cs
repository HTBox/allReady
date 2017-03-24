using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Resource;
using AllReady.Models;
using Xunit;
using Shouldly;

namespace AllReady.UnitTest.Areas.Admin.Features.Resource
{
    public class ResourceDetailQueryHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var campaign = new Campaign {Id = 2, Name = "CampaIgnName"};
            var resource = new AllReady.Models.Resource { Id = 2, Name = "ResourceName", Campaign = campaign };

            Context.Resources.Add(resource);

            Context.SaveChanges();
        }

        [Fact] 
        public async Task ReturnCorrectResource()
        {
            var sut = new ResourceDetailQueryHandler(Context);
            const int resourceId = 2;

            var resourceDetailViewModel = await sut.Handle(new ResourceDetailQuery { ResourceId = resourceId });

            resourceDetailViewModel.ShouldNotBeNull();
            resourceDetailViewModel.Id.ShouldBe(resourceId);
            resourceDetailViewModel.CampaignId.ShouldNotBeNull();
            resourceDetailViewModel.CampaignName.ShouldNotBeNull();
        }

    }
}
