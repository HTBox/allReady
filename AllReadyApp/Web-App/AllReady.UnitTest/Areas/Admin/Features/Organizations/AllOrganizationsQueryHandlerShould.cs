using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{

    public class AllOrganizationsQueryHandlerShould : InMemoryContextTest
    {
        private readonly List<Organization> organizations = new List<Organization>
        {
            new Organization { Id = 1 },
            new Organization { Id = 2 },
        };

        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            context.Organizations.AddRange(organizations);
            context.SaveChanges();
        }

        [Fact(Skip = "RTM Broken Tests")]
        public void InvokeOrganizations()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var sut = new AllOrganizationsQueryHandler(context);
            var results = sut.Handle(new AllOrganizationsQuery());

            Assert.NotNull(results);
            var resultList = results.OrderBy(s => s.Id).ToList();
            Assert.Equal(resultList[0].Id, organizations[0].Id);
            Assert.Equal(resultList[1].Id, organizations[1].Id);
        }
    }
}
