using AllReady.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.ViewModels
{
    public class LocationViewModelShould
    {
        [Fact]
        public void ReturnCityAndStateWhenBothArePresent()
        {
            // arrange
            var model = new LocationViewModel{
                City = "HappyTown",
                State = "Utopia"
            };

            // assert
            Assert.Equal("HappyTown, Utopia", model.Summary);

        }

        [Fact]
        public void ReturnCityWhenStateNotPresent()
        {
            // arrange
            var model = new LocationViewModel { City = "HappyTown" };

            // assert
            Assert.Equal("HappyTown", model.Summary);

        }

        [Fact]
        public void ReturnStateWhenCityNotPresent()
        {
            // arrange
            var model = new LocationViewModel { State = "Utopia" };

            // assert
            Assert.Equal("Utopia", model.Summary);

        }
    }
}
