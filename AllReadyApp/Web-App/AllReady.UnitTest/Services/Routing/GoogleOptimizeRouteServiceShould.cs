using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using AllReady.Services.Routing;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.IO;
using System.Text;

namespace AllReady.UnitTest.Services.Routing
{
    public class GoogleOptimizeRouteServiceShould
    {
        private readonly OptimizeRouteCriteria _optimiseRouteCriteria;
        private readonly Guid _requestId1 = new Guid("1d582827-af66-4eea-b405-9c1feaaee68d");
        private readonly Guid _requestId2 = new Guid("aa14b9ba-8ed4-428f-9413-f69d2b832795");



        public GoogleOptimizeRouteServiceShould()
        {
            _optimiseRouteCriteria = new OptimizeRouteCriteria(
                "24 Sussex Drive Ottawa ON",
                "200 Sussex Drive Ottawa ON",
                new List<OptimizeRouteWaypoint>()
                    {
                        new OptimizeRouteWaypoint(100.0, 100.0, _requestId1),
                        new OptimizeRouteWaypoint(200.0, 200.0, _requestId2)
                    }
                );
        }
        [Fact]
        public async Task ReturnsOptimisedRoutes()
        {
            var mappingSettings = new MappingSettings() { GoogleDirectionsApiKey = "somekey" };
            var options = Options.Create(mappingSettings);
            var mockedHttpClient = new Mock<IHttpClient>();

            var workingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            // on the build server the folder structure doesn't get respected so we won't find the file in "Mock\"
            var fullPath = Path.Combine(workingDirectory, "Services", "Routing", "mock_Geocoded_waypoints.json");

            var data = File.ReadAllText(fullPath);
            var mockedHttpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            mockedHttpClient.Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(mockedHttpResponse));
            var service = new GoogleOptimizeRouteService(
                options,
                Mock.Of<ILogger<GoogleOptimizeRouteService>>(),
                mockedHttpClient.Object);

            var result = await service.OptimizeRoute(_optimiseRouteCriteria);
            Assert.Equal(8897961, result.Distance);
            Assert.Equal(284665, result.Duration);
            //check the order of the requests 
            Assert.Equal(0, result.RequestIds[1].CompareTo(_requestId1));
            Assert.Equal(0, result.RequestIds[0].CompareTo(_requestId2));
        } 

        [Fact]
        public async Task GeneratesCorrectApiCall()
        {
            string expectedApiCall = "https://maps.googleapis.com/maps/api/directions/json?origin=24%20Sussex%20Drive%20Ottawa%20ON&destination=200%20Sussex%20Drive%20Ottawa%20ON&waypoints=optimize:true|100,100|200,200&key=somekey";
            var mappingSettings = new MappingSettings() { GoogleDirectionsApiKey = "somekey" };
            var options = Options.Create(mappingSettings);
            var mockedHttpClient = new Mock<IHttpClient>();
            var service = new GoogleOptimizeRouteService(
                options,
                Mock.Of<ILogger<GoogleOptimizeRouteService>>(),
                mockedHttpClient.Object);
    
            var result = await service.OptimizeRoute(_optimiseRouteCriteria);

            mockedHttpClient.Verify(x => x.GetAsync(It.Is<string>(y => y == expectedApiCall)));
        }

        [Fact]
        public async Task ReturnsNull_WhenOnBadAPIRequest()
        {
            var mappingSettings = new MappingSettings() { GoogleDirectionsApiKey = "some key" };
            var options = Options.Create(mappingSettings);
            var mockedHttpClient = new Mock<IHttpClient>();
            mockedHttpClient.Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)));
            var service = new GoogleOptimizeRouteService(
                options,
                Mock.Of<ILogger<GoogleOptimizeRouteService>>(),
                mockedHttpClient.Object);

            var result = await service.OptimizeRoute(_optimiseRouteCriteria);

            Assert.Null(result);
        }

        [Fact]
        public async Task ReturnNull_WhenApiCallTimesOut()
        {
            var mappingSettings = new MappingSettings() { GoogleDirectionsApiKey = "some key" };
            var options = Options.Create(mappingSettings);
            var mockedHttpClient = new Mock<IHttpClient>();
            mockedHttpClient.Setup(x => x.GetAsync(It.IsAny<string>())).Throws(new TaskCanceledException());
            var service = new GoogleOptimizeRouteService(
                options,
                Mock.Of<ILogger<GoogleOptimizeRouteService>>(),
                mockedHttpClient.Object);

            var result = await service.OptimizeRoute(_optimiseRouteCriteria);

            Assert.Null(result);
        }

        [Fact]
        public async Task ReturnNull_WhenGoogleDirectionsAPIKeyIsEmpty()
        {
            var mappingSettings = new MappingSettings() { GoogleDirectionsApiKey = string.Empty };
            var options = Options.Create(mappingSettings);
            var mockedHttpClient = new Mock<IHttpClient>();
            var service = new GoogleOptimizeRouteService(
                options,
                Mock.Of<ILogger<GoogleOptimizeRouteService>>(),
                mockedHttpClient.Object);

            var result = await service.OptimizeRoute(_optimiseRouteCriteria);

            Assert.Null(result);
        }
    }
}
