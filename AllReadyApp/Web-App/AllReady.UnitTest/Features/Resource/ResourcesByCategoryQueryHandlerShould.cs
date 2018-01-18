using AllReady.Features.Resource;
using AllReady.Models;
using Xunit;
using System.Threading.Tasks;

namespace AllReady.UnitTest.Features.Resource
{
    using Resource = AllReady.Models.Resource;

    public class ResourcesByCategoryQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task HandleInvokesGetResourcesByCategoryWithCorrectCategory()
        {
            var options = this.CreateNewContextOptions();

            const string category = "category";
            var message = new ResourcesByCategoryQuery { Category = category };

            using (var context = new AllReadyContext(options)) {
                context.Resources.Add(new Resource {
                    CategoryTag = category
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new ResourcesByCategoryQueryHandler(context);
                var resource = sut.Handle(message);

                Assert.Single(resource);
            }
        }
    }
}
