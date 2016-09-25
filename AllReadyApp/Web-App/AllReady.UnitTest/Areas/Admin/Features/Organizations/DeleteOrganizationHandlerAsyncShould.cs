using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Models;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{
    public class DeleteOrganizationHandlerAsyncShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            Context.Organizations.Add(new Organization { Id = 1, Name = "Org 1" });
            Context.Organizations.Add(new Organization { Id = 2, Name = "Org 2" });
            Context.SaveChanges();
        }

        [Fact]
        public async Task DeleteOrganizationThatMatchesOrganizationIdOnCommand()
        {
            var command = new DeleteOrganization { Id = 1 };
            var handler = new DeleteOrganizationHandler(Context);
            await handler.Handle(command);
            var data = Context.Organizations.Count(_ => _.Id == 1);
            Assert.Equal(0, data);
        }

        [Fact]
        public async Task NotDeleteOrganizationsThatDoNotMatchOrganizationIdOnCommand()
        {
            var command = new DeleteOrganization { Id = 999 };
            var handler = new DeleteOrganizationHandler(Context);
            await handler.Handle(command);
            Assert.Equal(2, Context.Organizations.Count());
        }
    }
}
