using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Resource;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Resource
{
    public class DeleteResourceCommandHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var resource = new AllReady.Models.Resource {Id = 2, Name = "ResourceName"};

            Context.Resources.Add(resource);

            Context.SaveChanges();
        }

        [Fact] 
        public async Task DeleteTheCorrectResource()
        {
            var sut = new DeleteResourceCommandHandler(Context);
            const int resourceId = 2;

            await sut.Handle(new DeleteResourceCommand {ResourceId = resourceId });

            Assert.False(Context.Resources.Any(r => r.Id == resourceId));
        }

    }
}
