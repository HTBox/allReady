using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels.Campaign;
using Xunit;

namespace AllReady.UnitTest.ViewModels.Campaign
{
    using Campaign = AllReady.Models.Campaign;
    using Event = AllReady.Models.Event;

    public class CampaignViewModelShould
    {
        [Fact]
        public void ReturnImmediately_WhenConstructingWithNullCampaign()
        {
            var sut = new CampaignViewModel((Campaign)null);
            Assert.Equal(0, sut.Id);
        }

        [Fact]
        public void InitializeEventsPropertyToEmptyList_WhenCallingDefaultConstructor()
        {
            var sut = new CampaignViewModel();
            Assert.NotNull(sut.Events);
        }

        [Fact]
        public void SetManagingOrganizationNameToEmptyString_WhenCampaignsManagingOrganizationIsNull()
        {
            var sut = new CampaignViewModel(new Campaign());
            Assert.Equal(sut.ManagingOrganizationName, string.Empty);
        }

        [Fact]
        public void SetManagingOrganizationNameToEmptyString_WhenCampaignsManagingOrganizationIsNotNull_AndNameIsAnEmptyString()
        {
            var sut = new CampaignViewModel(new Campaign { ManagingOrganization = new Organization()});
            Assert.Equal(sut.ManagingOrganizationName, string.Empty);
        }

        [Fact]
        public void SetManagingOrganizationNameToCampaignsOrganizationsName_WhenCampaignsManagingOrganizationIsNotNull_AndNameIsNotAnEmptyString()
        {
            var organization = new Organization { Name = "organizationNamee" };
            var sut = new CampaignViewModel(new Campaign { ManagingOrganization = organization });
            Assert.Equal(sut.ManagingOrganizationName, organization.Name);
        }

        [Fact]
        public void SetManagingOrganizationIdToZero_WhenCampaignsManagingOrganizationIsNotNull_AndIdIsZero()
        {
            var sut = new CampaignViewModel(new Campaign { ManagingOrganization = new Organization() });
            Assert.Equal(0, sut.ManagingOrganizationId);
        }

        [Fact]
        public void SetManagingOrganizationIdToCampaignsManagingOrganizationId_WhenCampaignsManagingOrganizationIsNotNull_AndIdIsNotZero()
        {
            var organization = new Organization { Id = 1 };
            var sut = new CampaignViewModel(new Campaign { ManagingOrganization = organization });
            Assert.Equal(sut.ManagingOrganizationId, organization.Id);
        }

        [Fact]
        public void SetManagingOrganizationIdToZero_WhenCampaignsManagingOrganizationIsNull()
        {
            var sut = new CampaignViewModel(new Campaign());
            Assert.Equal(0, sut.ManagingOrganizationId);
        }

        [Fact]
        public void SetManagingOrganizationLogoToEmptyString_WhenCampaignsManagingOrganizationIsNull()
        {
            var sut = new CampaignViewModel(new Campaign());
            Assert.Equal(sut.ManagingOrganizationLogo, string.Empty);
        }

        [Fact]
        public void SetManagingOrganizationLogoToEmptyString_WhenCampaignsManagingOrganizationIsNotNull_AndLogoUrlIsAnEmptyString()
        {
            var sut = new CampaignViewModel(new Campaign { ManagingOrganization = new Organization() });
            Assert.Equal(sut.ManagingOrganizationLogo, string.Empty);
        }

        [Fact]
        public void SetManagingOrganizationLogoToCampaignsOrganizationsName_WhenCampaignsManagingOrganizationIsNotNull_AndLogoUrlIsNotAnEmptyString()
        {
            var organization = new Organization { LogoUrl = "logoUrl" };
            var sut = new CampaignViewModel(new Campaign { ManagingOrganization = organization });
            Assert.Equal(sut.ManagingOrganizationLogo, organization.LogoUrl);
        }

        [Fact]
        public void SetEventsToEmptyListOfEventViewModel_WhenCampaignsEventsIsNull()
        {
            var sut = new CampaignViewModel(new Campaign());
            Assert.Empty(sut.Events);
        }

        [Fact]
        public void SetEventsToCampaignsEventsProjectedToEventViewModel_WhenCampaignsEventsIsNotNull()
        {
            var campaign = new Campaign { Events = new List<Event> { new Event { Id = 1} }};
            var eventViewModels = campaign.Events.ToViewModel();
            var sut = new CampaignViewModel(campaign);
            Assert.Equal(sut.Events.First().Id, eventViewModels.First().Id);
        }

        [Fact]
        public void SetHasPrivacyPolicyToFalse_WhenCampaignsManagingOrganizationIsNull()
        {
            var sut = new CampaignViewModel(new Campaign());
            Assert.False(sut.HasPrivacyPolicy);
        }

        [Fact]
        public void SetHasPrivacyPolicyToFalse_WhenCampaignsManagingOrganizationIsNotNull_AndPrivacyPolicyIsNullOrEmpty()
        {
            var sut = new CampaignViewModel(new Campaign { ManagingOrganization = new Organization()});
            Assert.False(sut.HasPrivacyPolicy);
        }

        [Fact]
        public void SetHasPrivacyPolicyToTrue_WhenCampaignsManagingOrganizationIsNotNull_AndPrivacyPolicyIsNotNullOrEmpty()
        {
            var sut = new CampaignViewModel(new Campaign { ManagingOrganization = new Organization { PrivacyPolicy = "privacyPolicy" }});
            Assert.True(sut.HasPrivacyPolicy);
        }

        [Fact]
        public void SetPrivacyPolicyUrlToNull_WhenCampaignsManagingOrganizationIsNull()
        {
            var sut = new CampaignViewModel(new Campaign());
            Assert.Null(sut.PrivacyPolicyUrl);
        }

        [Fact]
        public void SetPrivacyPolicyUrlToCampaignsManagingOrganizationsPrivacyPolicyUrl_WhenCampaignsManagingOrganizationIsNotNull()
        {
            var organization = new Organization { PrivacyPolicy = "privacyPolicy" };
            var sut = new CampaignViewModel(new Campaign { ManagingOrganization = organization });
            Assert.Equal(sut.PrivacyPolicyUrl, organization.PrivacyPolicyUrl);
        }

        [Fact]
        public void ReturnCityAndStateWhenBothArePresent()
        {
            // arrange
            var model = new CampaignViewModel { Location = new Location { City = "HappyTown", State = "Utopia" }};
            
            //act 
            var result = model.LocationSummary;

            //aswert
            Assert.Equal("HappyTown, Utopia", result);
        }

        [Fact]
        public void ReturnCityWhenStateNotPresent()
        {
            // arrange
            var model = new CampaignViewModel { Location = new Location { City = "HappyTown" }};

            // act
            var result = model.LocationSummary;

            // assert
            Assert.Equal("HappyTown", result);
        }

        [Fact]
        public void ReturnStateWhenCityNotPresent()
        {
            // arrange
            var model = new CampaignViewModel { Location = new Location { State = "Utopia" }};

            // act
            var result = model.LocationSummary;

            // assert
            Assert.Equal("Utopia", result);
        }

        [Fact]
        public void SetHasPrivacyPolicyUrlToTrue_WhenPrivacyPolicyUrlIsNotNullOrEmpty()
        {
            var sut = new CampaignViewModel { PrivacyPolicyUrl = "privacyPolicyUrl" };
            Assert.True(sut.HasPrivacyPolicyUrl);
        }

        [Fact]
        public void SetHasPrivacyPolicyUrlToFalse_WhenPrivacyPolicyUrlIsNullOrEmpty()
        {
            var sut = new CampaignViewModel();
            Assert.False(sut.HasPrivacyPolicyUrl);
        }

        [Fact]
        public void SetHasHeadlineToTrue_WhenHeadlineIsNotNullOrEmpty()
        {
            var sut = new CampaignViewModel { Headline = "headline" };
            Assert.True(sut.HasHeadline);
        }

        [Fact]
        public void SetHasHeadlineToFalse_WhenHeadlineIsNullOrEmpty()
        {
            var sut = new CampaignViewModel();
            Assert.False(sut.HasHeadline);
        }
    }
}
