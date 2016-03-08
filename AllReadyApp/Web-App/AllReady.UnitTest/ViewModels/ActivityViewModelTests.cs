using AllReady.Models;
using AllReady.ViewModels;
using Xunit;

namespace AllReady.UnitTest.ViewModels
{
    public class ActivityViewModelTests
    {
        [Fact]
        public void NoArgumentConstructorInstantiatesTasks()
        {
            var sut = new ActivityViewModel();
            Assert.NotNull(sut.Tasks);
        }

        [Fact]
        public void ActivityArgumentConstructorSetsOrganizationNameAndOrganizaitonIdWhenCampaignAndCampaignsManagagingOrganizationIsNotNull()
        {
            var activity = new Activity { Campaign = new Campaign { ManagingOrganization = new Organization { Id = 1, Name = "OrganizationName" }}};
            var sut = new ActivityViewModel(activity);

            Assert.Equal(activity.Campaign.ManagingOrganization.Id, sut.OrganizationId);
            Assert.Equal(activity.Campaign.ManagingOrganization.Name, sut.OrganizationName);
        }

        [Fact]
        public void ActivityArgumentConstructorSetsCampainIdAndCampianNameWhenCampaingIsNotNull()
        {
            var activity = new Activity { Campaign = new Campaign { Id = 1, Name = "CampaignName", ManagingOrganization = new Organization() }};
            var sut = new ActivityViewModel(activity);

            Assert.Equal(activity.Campaign.Id, sut.CampaignId);
            Assert.Equal(activity.Campaign.Name, sut.CampaignName);
        }
    }
}
