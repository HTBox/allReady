using AllReady.Models;
using AllReady.Security;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using System;

namespace AllReady.UnitTest.Security
{
    public class AuthorizableRequestBuilderTests : InMemoryContextTest
    {
        public static Guid RequestId = new Guid("42fe0870-6c47-4b7a-81bb-f4d5d4d6da5d");
        public static Guid RequestIdForRequestWithoutLinks = new Guid("15dd26b7-8014-4387-9feb-561a2aaf5912");
        public static Guid RequestIdForRequestWithPartialLinks = new Guid("7abaf53a-35eb-40e4-b9fc-05c60ce05456");

        [Fact]
        public async Task Build_WithAllFivesIds_ReturnsTheAuthorizableRequest()
        {
            var sut = new AuthorizableRequestBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(RequestId, 2, 3, 4, 5);

            result.RequestId.ShouldBe(RequestId);
            result.ItineraryId.ShouldBe(2);
            result.EventId.ShouldBe(3);
            result.CampaignId.ShouldBe(4);
            result.OrganizationId.ShouldBe(5);
        }

        [Fact]
        public async Task Build_WithJustRequestId_ReturnsFromTheCacheWhenAvailable()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set("AuthorizableRequest_" + RequestId, new FakeAuthorizableRequestIdContainer(), TimeSpan.FromMinutes(5));

            var sut = new AuthorizableRequestBuilder(Context, cache, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(RequestId);

            result.RequestId.ShouldBe(RequestId);
            result.ItineraryId.ShouldBe(100);
            result.EventId.ShouldBe(200);
            result.CampaignId.ShouldBe(300);
            result.OrganizationId.ShouldBe(400);
        }

        [Fact]
        public async Task Build_WithJustRequestId_ReturnsTheAuthorizableRequestFromTheDatabase()
        {
            var sut = new AuthorizableRequestBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(RequestId);

            result.RequestId.ShouldBe(RequestId);
            result.ItineraryId.ShouldBe(40);
            result.EventId.ShouldBe(10);
            result.CampaignId.ShouldBe(20);
            result.OrganizationId.ShouldBe(30);
        }

        [Fact]
        public async Task Build_WithJustRequestId_ReturnsTheAuthorizableRequestFromTheDatabase_WithExpectedIds_WhenNoLinkedElements()
        {
            var sut = new AuthorizableRequestBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(RequestIdForRequestWithoutLinks);

            result.RequestId.ShouldBe(RequestIdForRequestWithoutLinks);
            result.ItineraryId.ShouldBe(-1);
            result.EventId.ShouldBe(-1);
            result.CampaignId.ShouldBe(-1);
            result.OrganizationId.ShouldBe(-1);
        }

        [Fact]
        public async Task Build_WithJustRequestId_ReturnsTheAuthorizableRequestFromTheDatabase_WithExpectedIds_WhenPartialLinkedElements()
        {
            var sut = new AuthorizableRequestBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(RequestIdForRequestWithPartialLinks);

            result.RequestId.ShouldBe(RequestIdForRequestWithPartialLinks);
            result.ItineraryId.ShouldBe(-1);
            result.EventId.ShouldBe(-1);
            result.CampaignId.ShouldBe(-1);
            result.OrganizationId.ShouldBe(30);
        }

        [Fact]
        public async Task Build_SetsCaches_WithTheAuthorizableRequest()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());

            var sut = new AuthorizableRequestBuilder(Context, cache, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(RequestId);

            var foundInCache = cache.TryGetValue("AuthorizableRequest_" + RequestId, out IAuthorizableRequestIdContainer AuthorizableRequest);

            foundInCache.ShouldBeTrue();
            AuthorizableRequest.RequestId.ShouldBe(RequestId);
            AuthorizableRequest.ItineraryId.ShouldBe(40);
            AuthorizableRequest.EventId.ShouldBe(10);
            AuthorizableRequest.CampaignId.ShouldBe(20);
            AuthorizableRequest.OrganizationId.ShouldBe(30);
        }

        private class FakeAuthorizableRequestIdContainer : IAuthorizableRequestIdContainer
        {
            public Guid RequestId => AuthorizableRequestBuilderTests.RequestId;

            public int ItineraryId => 100;

            public int EventId => 200;

            public int CampaignId => 300;

            public int OrganizationId => 400;
        }

        protected override void LoadTestData()
        {
            var org = new Organization { Id = 30 };

            Context.Organizations.Add(org);

            var campaign = new Campaign { Id = 20, ManagingOrganization = org };

            Context.Campaigns.Add(campaign);

            var @event = new Event { Id = 10, Campaign = campaign };

            var itinerary = new Itinerary { Id = 40, EventId = 10 };

            Context.Itineraries.Add(itinerary);

            Context.Events.Add(@event);

            var request = new Request { RequestId = RequestId, ItineraryId = 40, EventId = 10, OrganizationId = 30 };
            var requestWithoutLinks = new Request { RequestId = RequestIdForRequestWithoutLinks };
            var RequestWithPartialLinks = new Request { RequestId = RequestIdForRequestWithPartialLinks, OrganizationId = 30 };

            Context.Requests.Add(request);
            Context.Requests.Add(requestWithoutLinks);
            Context.Requests.Add(RequestWithPartialLinks);

            Context.SaveChanges();
        }
    }
}
