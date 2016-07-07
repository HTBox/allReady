using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Organizations
{

    public class OrganizationByIdQueryHandlerShould : InMemoryContextTest
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
        public void InvokeGetOrganizationWithCorrectOrganizationId()
        {
            var THE_ID = 1;
            var context = ServiceProvider.GetService<AllReadyContext>();
            var message = new OrganizationByIdQuery { OrganizationId = THE_ID };
            var sut = new OrganizationByIdQueryHandler(context);
            var result = sut.Handle(message);

            Assert.Equal(result.Id, THE_ID);
        }
    }
}
