using AllReady.Features.ClosestLocation;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.ClosestLocation
{
    public class ClosestLocationsQueryHandlerTests
    {
        [Fact]
        public void HandleInvokesGetClosestLocationsWithCorrectData()
        {
            var message = new ClosestLocationsQuery { LocationQuery = new LocationQuery { Distance = 1, Latitude = 1, Longitude = 1, MaxRecordsToReturn = 1 }};

            var dataAccess = new Mock<IAllReadyDataAccess>();
            var sut = new ClosestLocationsQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.GetClosestLocations(It.Is<LocationQuery>(y => 
                y.Distance == message.LocationQuery.Distance &&
                y.Latitude == message.LocationQuery.Latitude &&
                y.Longitude == message.LocationQuery.Longitude &&
                y.MaxRecordsToReturn == message.LocationQuery.MaxRecordsToReturn)));
        }
    }
}
