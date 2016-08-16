using AllReady.ViewModels.Home;
using Xunit;

namespace AllReady.UnitTest.ViewModels.Home
{
    public class IndexViewModelTests
    {
        [Fact]
        public void HasFeaturedCampaignShouldReturnFalseIfViewModelHasNoFeaturedCampaign()
        {
            var sut = new IndexViewModel();
            Assert.False(sut.HasFeaturedCampaign);
        }

        [Fact]
        public void HasFeaturedCampaignShouldReturnTrueIfViewMModelHasAFeaturedCampaign()
        {
            var sut = new IndexViewModel
            {
                FeaturedCampaign = new CampaignSummaryViewModel { Id = 1, Title = "Something" }
            };

            Assert.True(sut.HasFeaturedCampaign);
        }
    }
}
