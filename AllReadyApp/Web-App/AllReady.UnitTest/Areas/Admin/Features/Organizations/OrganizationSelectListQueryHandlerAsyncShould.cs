using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{
    public class OrganizationSelectListQueryHandlerAsyncShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnAllOrganizationsAsListOfSelectListItems()
        {
            var organizations = new List<Organization>
            {
                new Organization { Id = 1, Name = "org1" },
                new Organization { Id = 2, Name = "org2" },
            };

            var context = Context;
            context.Organizations.AddRange(organizations);
            context.SaveChanges();

            var sut = new OrganizationSelectListQueryHandlerAsync(context);
            var result = await sut.Handle(new OrganizationSelectListQueryAsync());

            Assert.IsType<List<SelectListItem>>(result);
        }
    }
}
