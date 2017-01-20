using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using AllReady.Services.Routing;
using Microsoft.Extensions.Options;

namespace AllReady.UnitTest.Services.Routing
{
    public class GoogleOptimizeRouteServiceShould
    {
        [Fact]
        public async Task ReturnNull_WhenMappingIsEmpty()
        {
            var mappingSettings = new MappingSettings() { GoogleDirectionsApiKey = string.Empty };
            var options = Options.Create(mappingSettings);
            var mockedLogger = new Mock<ILogger<GoogleOptimizeRouteService>>();
            var mockedHttpClient = new Mock<IHttpClient>();
            var service = new GoogleOptimizeRouteService(
                options,
                mockedLogger.Object,
                mockedHttpClient.Object);

            var optimiseRouteCriteria = new OptimizeRouteCriteria(
                "some starting address",
                "test end address",
                new List<OptimizeRouteWaypoint>()
                    {
                        new OptimizeRouteWaypoint(100.0, 100.0, new Guid())
                    }
                );

            var result = await service.OptimizeRoute(optimiseRouteCriteria);
            Assert.Null(result);
        }
    }
}
