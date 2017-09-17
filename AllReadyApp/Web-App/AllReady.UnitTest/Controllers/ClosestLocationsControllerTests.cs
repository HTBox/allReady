using System.Collections.Generic;
using System.Linq;
using AllReady.Controllers;
using AllReady.Features.ClosestLocation;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class ClosestLocationsControllerTests
    {
        [Fact]
        public void GetSendsClosestLocationsQueryWithCorrectLocationQuery()
        {
            const double latitude = 1;
            const double longitude = 1;
            const int distance = 1;
            const int count = 1;

            var mediator = new Mock<IMediator>();

            var sut = new ClosestLocationsController(mediator.Object);
            sut.Get(latitude, longitude, distance, count);

            mediator.Verify(x => x.Send(It.Is<ClosestLocationsQuery>(y =>
                y.LocationQuery.Longitude == longitude &&
                y.LocationQuery.Latitude == latitude &&
                y.LocationQuery.Distance == distance &&
                y.LocationQuery.MaxRecordsToReturn == count)));
        }

        [Fact]
        public void GetReturnsCorrectModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ClosestLocationsQuery>())).Returns(new List<ClosestLocation>());

            var sut = new ClosestLocationsController(mediator.Object);
            var results = sut.Get(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>()).ToList();

            Assert.IsType<List<ClosestLocation>>(results);
        }

        [Fact]
        public void GetHasHttpGetAttributeWithCorrectTemplate()
        {
            var sut = new ClosestLocationsController(null);
            var attribute = sut.GetAttributesOn(x => x.Get(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("{lat}/{lon}/{distance}/{count}", attribute.Template);
        }

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectTemplate()
        {
            var sut = new ClosestLocationsController(null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("api/closest", attribute.Template);
        }
    }
}
