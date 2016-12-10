using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;
using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using AllReady.Services.Routing;

namespace AllReady.UnitTest.Services
{
    public class GoogleOptimizeRouteServiceTest : InMemoryContextTest
    {
        private const string EncodedStartAddress = "252 Dundas St, London, ON N6A 1H3";
        private const string EncodedEndAddress = "1750 Crumlin Road, London, ON N5V 3B6";

        [Fact]
        public async Task GetOptimizeRouteResult() { 
            var mSettings = new MappingSettings();
            // get key from https://developers.google.com/maps/documentation/directions/get-api-key
            mSettings.GoogleDirectionsApiKey = "AIzaSyAKxawFmQcX_NNjfbgRUajdZeGM0Yu_nDE";

            var mapSettings = new Mock<IOptions<MappingSettings>>();
            mapSettings.Setup(x => x.Value).Returns(mSettings);

            var logger = Mock.Of<ILogger<GoogleOptimizeRouteService>>();

            var gService = new GoogleOptimizeRouteService(mapSettings.Object, logger);

            var wayPoints = new List<OptimizeRouteWaypoint>();
            var wPoint = new OptimizeRouteWaypoint(-81.2293342, 43.003086, Guid.NewGuid());
            wayPoints.Add(wPoint);
            OptimizeRouteCriteria criteria = new OptimizeRouteCriteria(EncodedStartAddress, EncodedEndAddress, wayPoints);

            var oResult = await gService.OptimizeRoute(criteria);

            Assert.True(oResult.Duration > 800);
            Assert.True(oResult.Distance > 10000);
        }
    }
}
