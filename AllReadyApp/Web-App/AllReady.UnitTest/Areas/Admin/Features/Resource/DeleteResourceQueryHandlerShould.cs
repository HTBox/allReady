using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Resource;
using Xunit;
using Shouldly;

namespace AllReady.UnitTest.Areas.Admin.Features.Resource
{
    public class DeleteResourceQueryHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var resource = new AllReady.Models.Resource { Id = 2, Name = "ResourceName" };

            Context.Resources.Add(resource);

            Context.SaveChanges();
        }

        [Fact] 
        public async Task ReturnCorrectResourceDeleteViewModel()
        {
            var sut = new DeleteResourceQueryHandler(Context);
            const int resourceId = 2;

            var resourceDeleteViewModel = await sut.Handle(new DeleteResourceQuery {ResourceId = resourceId });

            resourceDeleteViewModel.ShouldNotBeNull();
            resourceDeleteViewModel.Id.ShouldBe(resourceId);
        }

    }
}
