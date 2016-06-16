using AllReady.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.ViewModels.Campaign;
using Xunit;

namespace AllReady.UnitTest.ViewModels
{
    public class CampaignViewModelShould
    {
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
