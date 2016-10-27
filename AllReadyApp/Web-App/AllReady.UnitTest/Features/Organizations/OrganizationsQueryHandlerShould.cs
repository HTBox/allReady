using System.Collections.Generic;
using AllReady.Features.Organizations;
using AllReady.Models;
using AllReady.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using AllReady.ViewModels.Organization;
using Xunit;

namespace AllReady.UnitTest.Features.Organizations
{
    public class OrganizationsQueryHandlerShould : InMemoryContextTest
    {
        private readonly List<Organization> organizations = new List<Organization>
        {
            new Organization { Id = 1 },
            new Organization { Id = 2 },
        };

        protected override void LoadTestData() {
            Context.Organizations.AddRange(organizations);
            Context.SaveChanges();
        }

        [Fact]
        public void HandleReturnsAllOrganizations()
        {
            var message = new OrganizationsQuery();

            var sut = new OrganizationsQueryHandler(Context);
            var results = sut.Handle(message);
            var resultList = results.OrderBy(s => s.Id).ToList();

            Assert.Equal(resultList[0].Id, organizations[0].Id);
            Assert.Equal(resultList[1].Id, organizations[1].Id);
        }

        [Fact]
        public void HandleReturnsListOfOrganizationViewModels()
        {
            var message = new OrganizationsQuery();
            var sut = new OrganizationsQueryHandler(Context);
            var results = sut.Handle(message);

            Assert.IsType<List<OrganizationViewModel>>(results);
        }
    }
}
