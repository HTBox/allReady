using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Campaigns
{
    public class IndexQueryHandlerAsyncShould : InMemoryContextTest
    {
        [Fact]
        public async Task ReturnTheCorrectViewModel()
        {
            Context.Campaigns.Add(new Campaign { ManagingOrganization = new Organization() });
            Context.SaveChanges();

            var sut = new IndexQueryHandlerAsync(Context);
            var result = await sut.Handle(new IndexQueryAsync());

            Assert.IsType<List<IndexViewModel>>(result);
        }

        [Fact]
        public async Task ReturnTheCampaignsTheUserIsAnOrgAdminFor_WhenOrganizationIdOnMessageIsNotNull()
        {
            const int orgId1 = 1;
            const int orgId2 = 2;

            Context.Campaigns.Add(new Campaign { Id = 1, ManagingOrganization = new Organization { Id = orgId1 }});
            Context.Campaigns.Add(new Campaign { Id = 2, ManagingOrganization = new Organization { Id = orgId2 }});
            Context.SaveChanges();

            var message = new IndexQueryAsync { OrganizationId = orgId1 };

            var sut = new IndexQueryHandlerAsync(Context);
            var result = await sut.Handle(message);

            Assert.Equal(result.Single().Id, orgId1);
        }

        [Fact]
        public async Task ReturnsAllCampaigns_WhenOrganizationIdOnMessageIsNull()
        {
            const int orgId1 = 1;
            const int orgId2 = 2;

            Context.Campaigns.Add(new Campaign { Id = 1, ManagingOrganization = new Organization { Id = orgId1 } });
            Context.Campaigns.Add(new Campaign { Id = 2, ManagingOrganization = new Organization { Id = orgId2 } });
            Context.SaveChanges();

            var sut = new IndexQueryHandlerAsync(Context);
            var result = await sut.Handle(new IndexQueryAsync());

            Assert.Equal(result.Count(), 2);
        }
    }
}