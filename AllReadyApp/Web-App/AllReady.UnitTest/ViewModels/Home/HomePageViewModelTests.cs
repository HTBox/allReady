using AllReady.ViewModels.Home;
using Xunit;

namespace AllReady.UnitTest.ViewModels.Home
{
    public class HomePageViewModelTests
    {
        [Fact]
        public void HasFeaturedCampaignShouldReturnFalseIfViewModelHasNoFeaturedCampaign()
        {
            var sut = new HomePageViewModel();
            Assert.False(sut.HasFeaturedCampaign);
        }

        [Fact]
        public void HasFeaturedCampaignShouldReturnTrueIfViewMModelHasAFeaturedCampaign()
        {
            var sut = new HomePageViewModel
            {
                FeaturedCampaign = new CampaignSummaryViewModel { Id = 1, Title = "Something" }
            };

            Assert.True(sut.HasFeaturedCampaign);
        }
    }
}
