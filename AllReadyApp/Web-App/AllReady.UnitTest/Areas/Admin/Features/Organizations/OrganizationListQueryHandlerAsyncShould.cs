using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{
    public class OrganizationListQueryHandlerAsyncShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnAllOrganizationsAsOrganizationSummaryModels()
        {
            var message = new OrganizationListQueryAysnc();

            var organizations = new[]
            {
                new Organization { Id = 1, LogoUrl = "LogoUrl1", Name = "Organization1", WebUrl = "WebUrl1" },
                new Organization { Id = 2, LogoUrl = "LogoUrl2", Name = "Organization2", WebUrl = "WebUrl2" }
            };

            var context = Context;
            context.Organizations.AddRange(organizations);
            context.SaveChanges();

            var sut = new OrganizationListQueryHandlerAsync(context);
            var result = await sut.Handle(message);

            Assert.IsType<List<OrganizationSummaryModel>>(result);
        }
    }
}