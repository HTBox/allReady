using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Resource;
using AllReady.Areas.Admin.ViewModels.Resource;
using AllReady.Models;
using Xunit;
using Shouldly;

namespace AllReady.UnitTest.Areas.Admin.Features.Resource
{
    public class CreateOrEditResourceCommandHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var campaign = new Campaign { Name = "CampaIgnName" };
            var resource = new AllReady.Models.Resource { Name = "ResourceName", Campaign = campaign };

            Context.Resources.Add(resource);

            Context.SaveChanges(); 
        }

        [Fact]
        public async Task CreateResource()
        {
            var resourceEditViewModel = new ResourceEditViewModel
            {
                CampaignId = 1,
                CampaignName = "CampaignName",
                Name = "ResourceName",
                Description = "TestDescription",
                ResourceUrl = "/url/"
            };

            var createOrEditResourceCommand = new CreateOrEditResourceCommand { Resource = resourceEditViewModel };

            var sut = new CreateOrEditResourceCommandHandler(Context);


            var result = await sut.Handle(createOrEditResourceCommand);

            result.ShouldNotBeNull();
            Context.Resources.Any(r => r.Id == result).ShouldBeTrue();
        }

        [Fact]
        public async Task EditResource()
        {
            var resource = Context.Resources.First();

            var resourceEditViewModel = new ResourceEditViewModel { Id = resource.Id, Name = resource.Name };
            resourceEditViewModel.Name = "ChangedName";

            var createOrEditResourceCommand = new CreateOrEditResourceCommand { Resource = resourceEditViewModel };

            var sut = new CreateOrEditResourceCommandHandler(Context);


            var result = await sut.Handle(createOrEditResourceCommand);

            result.ShouldNotBeNull();
            Context.Resources.Single(r => r.Id == result).Name.ShouldBe(resourceEditViewModel.Name);
        }
    }
}
