using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{
    public class OrganizationListQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public void ReturnAllOrganizationsAsOrganizationSummaryModels()
        {
            var message = new OrganizationListQuery();

            var organizations = new[]
            {
                new Organization { Id = 1, LogoUrl = "LogoUrl1", Name = "Organization1", WebUrl = "WebUrl1" },
                new Organization { Id = 2, LogoUrl = "LogoUrl2", Name = "Organization2", WebUrl = "WebUrl2" }
            };

            var context = Context;
            context.Organizations.AddRange(organizations);
            context.SaveChanges();

            var sut = new OrganizationListQueryHandler(context);
            var result = sut.Handle(message).ToList();

            Assert.IsType<List<OrganizationSummaryModel>>(result);
        }
    }
}