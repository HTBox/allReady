using AllReady.Features.ClosestLocation;
using AllReady.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AllReady.UnitTest.Features.ClosestLocation
{

    public class ClosestLocationsQueryHandlerShould : InMemoryContextTest
    {

        // TODO: Investigate EF Geolocation features

        /*
        protected override void LoadTestData()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
        }
        */

        [Fact(Skip = "RTM Broken Tests")]
        public void HandleInvokesGetClosestLocationsWithCorrectData()
        {
            var message = new ClosestLocationsQuery { LocationQuery = new LocationQuery { Distance = 1, Latitude = 1, Longitude = 1, MaxRecordsToReturn = 1 }};

            var context = ServiceProvider.GetService<AllReadyContext>();
            var sut = new ClosestLocationsQueryHandler(context);
            var results = sut.Handle(message);

            Assert.Empty(results);

            // TODO: validate something meaningful
            /*
            dataAccess.Verify(x => x.GetClosestLocations(It.Is<LocationQuery>(y =>
                y.Distance == message.LocationQuery.Distance &&
                y.Latitude == message.LocationQuery.Latitude &&
                y.Longitude == message.LocationQuery.Longitude &&
                y.MaxRecordsToReturn == message.LocationQuery.MaxRecordsToReturn)));
            */
        }
    }
}
