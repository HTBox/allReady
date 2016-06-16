using AllReady.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.ViewModels
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
            var sut = new HomePageViewModel()
            {
                FeaturedCampaign = new CampaignSummaryViewModel { Id = 1, Title = "Something" }
            };

            Assert.True(sut.HasFeaturedCampaign);
        }
    }
}
