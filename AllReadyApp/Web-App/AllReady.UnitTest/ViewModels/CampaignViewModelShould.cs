using AllReady.ViewModels.Campaign;
using Xunit;

namespace AllReady.UnitTest.ViewModels
{
    public class CampaignViewModelShould
    {
        [Fact]
        public void ReturnImmediately_WhenConstructingWithNullCampaign()
        {
            var sut = new CampaignViewModel(null);
            Assert.Equal(sut.Id, 0);
        }

        [Fact]
        public void InitializeEventsPropertyToEmptyList_WhenCallingDefaultConstructor()
        {
            var sut = new CampaignViewModel();
            Assert.NotNull(sut.Events);
        }

        //test happy constructor path
        [Fact(Skip = "Not Implemented")]
        public void HavePropertiesPopulatedCorrectly_WhenConstructingWithCampaign()
        {
        }

        //test non-happy constructor path
        [Fact]
        public void SetManagingOrganizationNameToEmptyString_WhenCampaignsManagingOrganizationIsNull()
        {
            var sut = new CampaignViewModel(new Models.Campaign());
            Assert.Equal(sut.ManagingOrganizationName, string.Empty);
        }

        [Fact]
        public void SetManagingOrganizationIdToZero_WhenCampaignsManagingOrganizationIsNull()
        {
            var sut = new CampaignViewModel(new Models.Campaign());
            Assert.Equal(sut.ManagingOrganizationId, 0);
        }

        [Fact]
        public void SetManagingOrganizationLogoToEmptyString_WhenCampaignsManagingOrganizationIsNull()
        {
            var sut = new CampaignViewModel(new Models.Campaign());
            Assert.Equal(sut.ManagingOrganizationLogo, string.Empty);
        }

        [Fact]
        public void SetEventsToEmptyListOfEventViewModel_WhenCampaignsEventsIsNull()
        {
            var sut = new CampaignViewModel(new Models.Campaign());
            Assert.Empty(sut.Events);
        }

        [Fact]
        public void SetHasPrivacyPolicyToFalse_WhenCampaignsManagingOrganizationIsNull()
        {
            var sut = new CampaignViewModel(new Models.Campaign());
            Assert.False(sut.HasPrivacyPolicy);
        }

        [Fact]
        public void SetHasPrivacyPolicyToFalse_WhenCampaignsManagingOrganizationIsNotNull_AndPrivacyPolicyIsNullOrEmpty()
        {
            var sut = new CampaignViewModel(new Models.Campaign { ManagingOrganization = new Models.Organization()});
            Assert.False(sut.HasPrivacyPolicy);
        }

        [Fact]
        public void SetPrivacyPolicyUrlToNull_WhenCampaignsManagingOrganizationIsNull()
        {
            var sut = new CampaignViewModel(new Models.Campaign());
            Assert.Null(sut.PrivacyPolicyUrl);
        }

        [Fact]
        public void SetLocationSummaryToCorrectValue_WhenLocationIsNotNull_AndLocationsCityAndStateAreNotNullOrEmpty()
        {
            var location = new Models.Location { City = "city", State = "state" };
            var sut = new CampaignViewModel { Location = location };
            Assert.Equal(sut.LocationSummary, $"{location.City}, {location.State}");
        }

        [Fact]
        public void SetLocationSummaryToCorrectValue_WhenLocationIsNotNull_AndLocationsCityIsNotNullOrEmpty()
        {
            var location = new Models.Location { City = "city" };
            var sut = new CampaignViewModel { Location = location };
            Assert.Equal(sut.LocationSummary, location.City);
        }

        [Fact]
        public void SetLocationSummaryToCorrectValue_WhenLocationIsNotNull_AndLocationsStateIsNotNullOrEmpty()
        {
            var location = new Models.Location { State = "state" };
            var sut = new CampaignViewModel { Location = location };
            Assert.Equal(sut.LocationSummary, location.State);
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

        [Fact]
        public void ReturnCityAndStateWhenBothArePresent()
        {
            // arrange
            var model = new CampaignViewModel();

            // act
            model.Location = new Models.Location
            {
                City = "HappyTown",
                State = "Utopia"
            };

            // assert
            Assert.Equal("HappyTown, Utopia", model.LocationSummary);

        }

        [Fact]
        public void ReturnCityWhenStateNotPresent()
        {
            // arrange
            var model = new CampaignViewModel();

            // act
            model.Location = new Models.Location
            {
                City = "HappyTown"
            };

            // assert
            Assert.Equal("HappyTown", model.LocationSummary);
        }

        [Fact]
        public void ReturnStateWhenCityNotPresent()
        {
            // arrange
            var model = new CampaignViewModel();

            // act
            model.Location = new Models.Location
            {
                State = "Utopia"
            };

            // assert
            Assert.Equal("Utopia", model.LocationSummary);
        }
    }
}
