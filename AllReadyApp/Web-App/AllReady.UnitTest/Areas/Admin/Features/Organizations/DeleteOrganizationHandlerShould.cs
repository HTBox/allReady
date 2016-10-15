using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Models;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{
    public class DeleteOrganizationHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            Context.Organizations.Add(new Organization { Id = 1, Name = "Org 1" });
            Context.SaveChanges();
        }

        [Fact]
        public async Task DeleteTheOrganization()
        {
            var command = new DeleteOrganization { Id = 1 };

            var handler = new DeleteOrganizationHandler(Context);
            await handler.Handle(command);

            Assert.False(Context.Organizations.Any(x => x.Id == command.Id));
        }

    }
}
