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
    public class AuthorizableItineraryBuilderTests : InMemoryContextTest
    {
        [Fact]
        public async Task Build_WithAllFourIds_ReturnsTheAuthorizableItinerary()
        {
            var sut = new AuthorizableItineraryBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(1, 2, 3, 4);

            result.ItineraryId.ShouldBe(1);
            result.EventId.ShouldBe(2);
            result.CampaignId.ShouldBe(3);
            result.OrganizationId.ShouldBe(4);
        }

        [Fact]
        public async Task Build_WithJustItineraryId_ReturnsFromTheCacheWhenAvailable()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set("AuthorizableItinerary_1", new FakeAuthorizableItineraryIdContainer(), TimeSpan.FromMinutes(5));

            var sut = new AuthorizableItineraryBuilder(Context, cache, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(1);

            result.ItineraryId.ShouldBe(1);
            result.EventId.ShouldBe(100);
            result.CampaignId.ShouldBe(200);
            result.OrganizationId.ShouldBe(300);
        }

        [Fact]
        public async Task Build_WithJustItineraryId_ReturnsTheAuthorizableItineraryFromTheDatabase()
        {
            var sut = new AuthorizableItineraryBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(5);

            result.ItineraryId.ShouldBe(5);
            result.EventId.ShouldBe(10);
            result.CampaignId.ShouldBe(20);
            result.OrganizationId.ShouldBe(30);
        }

        [Fact]
        public async Task Build_SetsCaches_WithTheAuthorizableItinerary()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());

            var sut = new AuthorizableItineraryBuilder(Context, cache, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(5);

            IAuthorizableItineraryIdContainer AuthorizableItinerary;
            var foundInCache = cache.TryGetValue("AuthorizableItinerary_5", out AuthorizableItinerary);

            foundInCache.ShouldBeTrue();
            AuthorizableItinerary.ItineraryId.ShouldBe(5);
            AuthorizableItinerary.EventId.ShouldBe(10);
            AuthorizableItinerary.CampaignId.ShouldBe(20);
            AuthorizableItinerary.OrganizationId.ShouldBe(30);
        }

        private class FakeAuthorizableItineraryIdContainer : IAuthorizableItineraryIdContainer
        {
            public int ItineraryId => 1;

            public int CampaignId => 200;

            public int EventId => 100;

            public int OrganizationId => 300;
        }

        protected override void LoadTestData()
        {
            var org = new Organization { Id = 30 };

            Context.Organizations.Add(org);

            var campaign = new Campaign { Id = 20, ManagingOrganization = org };

            Context.Campaigns.Add(campaign);

            var @event = new Event { Id = 10, Campaign = campaign };
                        
            Context.Events.Add(@event);

            var itinerary = new Itinerary { Id = 5, EventId = 10 };

            Context.Itineraries.Add(itinerary);

            Context.SaveChanges();
        }
    }
}
