using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Services.Mapping.Routing;
using Shouldly;
using Xunit;
using Microsoft.Extensions.Caching.Memory;
using AllReady.Caching;

namespace AllReady.UnitTest.Areas.Admin.Features.Itineraries
{
    public class OptimizeRouteCommandHandlerShould : InMemoryContextTest
    {
        private const string EncodedFullAddress = "1%20Some%20Road,%20A%20town,%20A%20state,%20postalcode,%20United%20Kingdom";

        private static readonly Guid Request1Id = new Guid("de4f4639-86ea-419f-96c8-509defa4d9a3");
        private static readonly Guid Request2Id = new Guid("602b3f58-c8e0-4f59-82b0-f940c1aa1caa");
        private static readonly Guid Request3Id = new Guid("602b3f58-c8e0-4f59-82b0-f940c1aa1e4a");
        private static readonly Guid Request4Id = new Guid("602b3f58-c8e0-4f59-82b0-f940c1aa1111");

        protected override void LoadTestData()
        {
            var location = new Location
            {
                Address1 = "1 Some Road",
                City = "A town",
                State = "A state",
                PostalCode = "postalcode",
                Country = "United Kingdom"
            };

            var itinerary1 = new Itinerary
            {
                Id = 1,
                Name = "Test Itinerary 1",
                StartLocation = location,
                EndLocation = location
            };

            var itinerary2 = new Itinerary
            {
                Id = 2,
                Name = "Test Itinerary 2"
            };

            var itinerary3 = new Itinerary
            {
                Id = 3,
                Name = "Test Itinerary 3",
                StartLocation = location,
                UseStartAddressAsEndAddress = false
            };

            var request1 = new Request
            {
                RequestId = Request1Id,
                Name = "Request 1",
                Latitude = 50.8225,
                Longitude = -0.1372
            };

            var request2 = new Request
            {
                RequestId = Request2Id,
                Name = "Request 2",
                Latitude = 10.0000,
                Longitude = -5.0000
            };

            var request3 = new Request
            {
                RequestId = Request3Id,
                Name = "Request 3",
                Latitude = 10.0000,
                Longitude = -5.0000
            };

            var request4 = new Request
            {
                RequestId = Request4Id,
                Name = "Request 4",
                Latitude = 10.0000,
                Longitude = -5.0000
            };

            var itineraryRequest1 = new ItineraryRequest
            {
                Request = request1,
                Itinerary = itinerary1,
                OrderIndex = 1
            };

            var itineraryRequest2 = new ItineraryRequest
            {
                Request = request2,
                Itinerary = itinerary2
            };

            var itineraryRequest3 = new ItineraryRequest
            {
                Request = request3,
                Itinerary = itinerary3
            };

            var itineraryRequest4 = new ItineraryRequest
            {
                Request = request4,
                Itinerary = itinerary1,
                OrderIndex = 2
            };

            Context.Locations.Add(location);
            Context.Itineraries.Add(itinerary1);
            Context.Itineraries.Add(itinerary2);
            Context.Itineraries.Add(itinerary3);
            Context.Requests.Add(request1);
            Context.Requests.Add(request2);
            Context.Requests.Add(request3);
            Context.Requests.Add(request4);

            Context.SaveChanges();

            Context.Add(itineraryRequest1);
            Context.Add(itineraryRequest2);
            Context.Add(itineraryRequest3);
            Context.Add(itineraryRequest4);

            Context.SaveChanges();
        }

        [Fact]
        public async Task NotCallOptimizeRouteService_WhenItineraryNotFound()
        {
            var optimizeRouteService = new Mock<IOptimizeRouteService>();
            optimizeRouteService.Setup(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>())).Verifiable();

            var sut = new OptimizeRouteCommandHandler(Context, optimizeRouteService.Object, Mock.Of<IMemoryCache>());

            await sut.Handle(new OptimizeRouteCommand { ItineraryId = 200 });

            optimizeRouteService.Verify(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>()), Times.Never);
        }

        [Fact]
        public async Task NotCallOptimizeRouteService_WhenItineraryHasNoStartAddress()
        {
            var optimizeRouteService = new Mock<IOptimizeRouteService>();
            optimizeRouteService.Setup(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>())).Verifiable();

            var sut = new OptimizeRouteCommandHandler(Context, optimizeRouteService.Object, Mock.Of<IMemoryCache>());

            await sut.Handle(new OptimizeRouteCommand { ItineraryId = 2 });

            optimizeRouteService.Verify(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>()), Times.Never);
        }

        [Fact]
        public async Task NotCallOptimizeRouteService_WhenItineraryHasNoEndAddress_AndNotSetToUseStartAddressAsEndAddress()
        {
            var optimizeRouteService = new Mock<IOptimizeRouteService>();
            optimizeRouteService.Setup(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>())).Verifiable();

            var sut = new OptimizeRouteCommandHandler(Context, optimizeRouteService.Object, Mock.Of<IMemoryCache>());

            await sut.Handle(new OptimizeRouteCommand { ItineraryId = 3 });

            optimizeRouteService.Verify(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>()), Times.Never);
        }

        [Fact]
        public async Task CallOptimizeRouteService_WhenStartAndEndAddressPresent_WithCorrectCriteria()
        {
            OptimizeRouteCriteria criteria = null;

            var optimizeRouteService = new Mock<IOptimizeRouteService>();
            optimizeRouteService.Setup(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>()))
                .ReturnsAsync(new OptimizeRouteResult())
                .Callback<OptimizeRouteCriteria>(x => criteria = x)
                .Verifiable();

            var sut = new OptimizeRouteCommandHandler(Context, optimizeRouteService.Object, Mock.Of<IMemoryCache>());

            await sut.Handle(new OptimizeRouteCommand { ItineraryId = 1 });

            optimizeRouteService.Verify(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>()), Times.Once);
            criteria.StartAddress.ShouldBe(EncodedFullAddress);
            criteria.EndAddress.ShouldBe(EncodedFullAddress);
            criteria.Waypoints.Count.ShouldBe(2);
        }

        [Fact]
        public async Task NotChangeRequestOrder_WhenOptimizeRouteResultIsNotSuccess()
        {
            var optimizeRouteService = new Mock<IOptimizeRouteService>();
            optimizeRouteService.Setup(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>()))
                .ReturnsAsync(OptimizeRouteResult.FailedOptimizeRouteResult("failure"));

            var sut = new OptimizeRouteCommandHandler(Context, optimizeRouteService.Object, Mock.Of<IMemoryCache>());

            await sut.Handle(new OptimizeRouteCommand { ItineraryId = 1 });

            var requests = Context.ItineraryRequests.Where(x => x.ItineraryId == 1).OrderBy(x => x.OrderIndex).ToList();

            requests[0].Request.Name.ShouldBe("Request 1");
            requests[1].Request.Name.ShouldBe("Request 4");
        }

        [Fact]
        public async Task NotChangeRequestOrder_WhenOptimizeRouteResultRequestIdCountDoesNotMatchWaypointCount()
        {
            var optimizeRouteService = new Mock<IOptimizeRouteService>();
            optimizeRouteService.Setup(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>()))
                .ReturnsAsync(new OptimizeRouteResult { Distance = 10, Duration = 10, RequestIds = new List<Guid> { Guid.NewGuid() } });

            var sut = new OptimizeRouteCommandHandler(Context, optimizeRouteService.Object, Mock.Of<IMemoryCache>());

            await sut.Handle(new OptimizeRouteCommand { ItineraryId = 1 });

            var requests = Context.ItineraryRequests.Where(x => x.ItineraryId == 1).OrderBy(x => x.OrderIndex).ToList();

            requests[0].Request.Name.ShouldBe("Request 1");
            requests[1].Request.Name.ShouldBe("Request 4");
        }

        [Fact]
        public async Task NotChangeRequestOrder_WhenOptimizeRouteResultDoesNotReturnMatchingRequestIds()
        {
            var optimizeRouteService = new Mock<IOptimizeRouteService>();
            optimizeRouteService.Setup(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>()))
                .ReturnsAsync(new OptimizeRouteResult { Distance = 10, Duration = 10, RequestIds = new List<Guid> { Request4Id, Guid.Empty } });

            var sut = new OptimizeRouteCommandHandler(Context, optimizeRouteService.Object, Mock.Of<IMemoryCache>());

            await sut.Handle(new OptimizeRouteCommand { ItineraryId = 1 });

            var requests = Context.ItineraryRequests.Where(x => x.ItineraryId == 1).OrderBy(x => x.OrderIndex).ToList();

            requests.First(x => x.RequestId == Request1Id).OrderIndex.ShouldBe(1);
            requests.First(x => x.RequestId == Request4Id).OrderIndex.ShouldBe(2);
        }

        [Fact]
        public async Task ChangeRequestOrder_WhenOptimizeRouteResultReturnsUpdatedWaypointsOrder()
        {
            var optimizeRouteService = new Mock<IOptimizeRouteService>();
            optimizeRouteService.Setup(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>()))
                .ReturnsAsync(new OptimizeRouteResult { Distance = 10, Duration = 10, RequestIds = new List<Guid> { Request4Id, Request1Id } });

            var sut = new OptimizeRouteCommandHandler(Context, optimizeRouteService.Object, Mock.Of<IMemoryCache>());

            await sut.Handle(new OptimizeRouteCommand { ItineraryId = 1 });

            var requests = Context.ItineraryRequests.Where(x => x.ItineraryId == 1).OrderBy(x => x.OrderIndex).ToList();

            requests[0].Request.Name.ShouldBe("Request 4");
            requests[1].Request.Name.ShouldBe("Request 1");
        }

        [Fact]
        public async Task SetsCorrectCacheEntry_WhenOptimizeRouteResultIsNotSuccess()
        {
            const string statusMsg = "failure";

            var optimizeRouteService = new Mock<IOptimizeRouteService>();
            optimizeRouteService.Setup(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>()))
                .ReturnsAsync(OptimizeRouteResult.FailedOptimizeRouteResult(statusMsg));

            var cache = new MemoryCache(new MemoryCacheOptions());

            var sut = new OptimizeRouteCommandHandler(Context, optimizeRouteService.Object, cache);

            await sut.Handle(new OptimizeRouteCommand { ItineraryId = 1, UserId = "123" });

            var expectedCacheKey = CacheKeyBuilder.BuildOptimizeRouteCacheKey("123", 1);

            OptimizeRouteResultStatus cachedStatus = null;
            var keyExists = cache.TryGetValue(expectedCacheKey, out cachedStatus);

            keyExists.ShouldBeTrue();
            cachedStatus.IsSuccess.ShouldBeFalse();
            cachedStatus.StatusMessage.ShouldBe(statusMsg);
        }

        [Fact]
        public async Task SetsCorrectCacheEntry_WhenOptimizeRouteResultValidationFails()
        {
            var optimizeRouteService = new Mock<IOptimizeRouteService>();
            optimizeRouteService.Setup(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>()))
                .ReturnsAsync(new OptimizeRouteResult { Distance = 10, Duration = 10, RequestIds = new List<Guid> { Guid.NewGuid() } });

            var cache = new MemoryCache(new MemoryCacheOptions());

            var sut = new OptimizeRouteCommandHandler(Context, optimizeRouteService.Object, cache);

            await sut.Handle(new OptimizeRouteCommand { ItineraryId = 1, UserId = "123" });

            var expectedCacheKey = CacheKeyBuilder.BuildOptimizeRouteCacheKey("123", 1);

            OptimizeRouteResultStatus cachedStatus = null;
            var keyExists = cache.TryGetValue(expectedCacheKey, out cachedStatus);

            keyExists.ShouldBeTrue();
            cachedStatus.IsSuccess.ShouldBeFalse();
            cachedStatus.StatusMessage.ShouldBe(OptimizeRouteStatusMessages.GeneralOptimizeFailure);            
        }

        [Fact]
        public async Task SetsCorrectCacheEntry_WhenOptimizeRouteResultIsSuccess_AndRequestOrderDoesNotChange()
        {
            var optimizeRouteService = new Mock<IOptimizeRouteService>();
            optimizeRouteService.Setup(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>()))
                .ReturnsAsync(new OptimizeRouteResult { Distance = 10, Duration = 10, RequestIds = new List<Guid> { Request1Id, Request4Id } });

            var cache = new MemoryCache(new MemoryCacheOptions());

            var sut = new OptimizeRouteCommandHandler(Context, optimizeRouteService.Object, cache);

            await sut.Handle(new OptimizeRouteCommand { ItineraryId = 1, UserId = "123" });

            var expectedCacheKey = CacheKeyBuilder.BuildOptimizeRouteCacheKey("123", 1);

            OptimizeRouteResultStatus cachedStatus = null;
            var keyExists = cache.TryGetValue(expectedCacheKey, out cachedStatus);

            keyExists.ShouldBeTrue();
            cachedStatus.IsSuccess.ShouldBeTrue();
            cachedStatus.StatusMessage.ShouldBe(OptimizeRouteStatusMessages.AlreadyOptimized);
        }

        [Fact]
        public async Task SetsCorrectCacheEntry_WhenOptimizeRouteResultIsSuccess_AndRequestOrderIsUpdated()
        {
            var optimizeRouteService = new Mock<IOptimizeRouteService>();
            optimizeRouteService.Setup(x => x.OptimizeRoute(It.IsAny<OptimizeRouteCriteria>()))
                .ReturnsAsync(new OptimizeRouteResult { Distance = 10, Duration = 10, RequestIds = new List<Guid> { Request4Id, Request1Id } });

            var cache = new MemoryCache(new MemoryCacheOptions());

            var sut = new OptimizeRouteCommandHandler(Context, optimizeRouteService.Object, cache);

            await sut.Handle(new OptimizeRouteCommand { ItineraryId = 1, UserId = "123" });

            var expectedCacheKey = CacheKeyBuilder.BuildOptimizeRouteCacheKey("123", 1);

            OptimizeRouteResultStatus cachedStatus = null;
            var keyExists = cache.TryGetValue(expectedCacheKey, out cachedStatus);

            keyExists.ShouldBeTrue();
            cachedStatus.IsSuccess.ShouldBeTrue();
            cachedStatus.StatusMessage.ShouldBe(OptimizeRouteStatusMessages.OptimizeSucess);
        }
    }
}
