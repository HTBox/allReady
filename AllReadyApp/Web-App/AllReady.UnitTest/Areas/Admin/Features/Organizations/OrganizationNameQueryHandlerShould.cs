using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{
    public class OrganizationNameQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnNullWhenThereIsNoMatchingOrganizationId()
        {
            var context = Context;
            context.SaveChanges();
            var sut = new OrganizationNameQueryHandler(context);
            var result = await sut.Handle(new OrganizationNameQuery());

            Assert.Null(result);
        }

        [Fact]
        public async Task ReturnOrganziationNameWhenThereIsAMatchingOrganizationId()
        {
            var organization = new Organization { Id = 1, Name = "OrganizationName" };
            var context = Context;
            context.Organizations.Add(organization);
            context.SaveChanges();

            var sut = new OrganizationNameQueryHandler(context);
            var result = await sut.Handle(new OrganizationNameQuery { Id = organization.Id });

            Assert.Equal(result, organization.Name);
        }
    }
}
