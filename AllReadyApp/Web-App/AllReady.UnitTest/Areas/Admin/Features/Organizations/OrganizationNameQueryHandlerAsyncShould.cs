using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{
    public class OrganizationNameQueryHandlerAsyncShould : InMemoryContextTestBase
    {
        [Fact(Skip = "RTM Broken Tests")]
        public async Task ReturnNullWhenThereIsNoMatchingOrganizationId()
        {
            var context = Context;
            context.SaveChanges();
            var sut = new OrganizationNameQueryHandlerAsync(context);
            var result = await sut.Handle(new OrganizationNameQueryAsync());

            Assert.Null(result);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task ReturnOrganziationNameWhenThereIsAMatchingOrganizationId()
        {
            var organization = new Organization { Id = 1, Name = "OrganizationName" };
            var context = Context;
            context.Organizations.Add(organization);
            context.SaveChanges();

            var sut = new OrganizationNameQueryHandlerAsync(context);
            var result = await sut.Handle(new OrganizationNameQueryAsync { Id = organization.Id });

            Assert.Equal(result, organization.Name);
        }
    }
}