using System.Collections.Generic;
using System.Linq;
using AllReady.Controllers;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class ClosestLocationsControllerTests
    {
        [Fact]
        public void GetInvokesGetClosestLocationsWithCorrectLocationQuery()
        {
            const double latitude = 1;
            const double longitude = 1;
            const int distance = 1;
            const int count = 1;

            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new ClosestLocationsController(dataAccess.Object);
            sut.Get(latitude, longitude, distance, count);

            dataAccess.Verify(x => x.GetClosestLocations(
                It.Is<LocationQuery>(y => 
                    y.Longitude == longitude &&
                    y.Latitude == latitude &&
                    y.Distance == distance &&
                    y.MaxRecordsToReturn == count)));
        }

        [Fact]
        public void GetReturnsCorrectModel()
        {
            var dataAccess = new Mock<IAllReadyDataAccess>();
            dataAccess.Setup(x => x.GetClosestLocations(It.IsAny<LocationQuery>())).Returns(new List<ClosestLocation>());

            var sut = new ClosestLocationsController(dataAccess.Object);
            var results = sut.Get(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>()).ToList();

            Assert.IsType<List<ClosestLocation>>(results);
        }

        [Fact]
        public void GetHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new ClosestLocationsController(null);
            var attribute = sut.GetAttributesOn(x => x.Get(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "{lat}/{lon}/{distance}/{count}");
        }

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectTemplate()
        {
            var sut = new ClosestLocationsController(null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "api/closest");
        }
    }
}
