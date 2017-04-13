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
    public class AuthorizableEventBuilderTests : InMemoryContextTest
    {
        [Fact]
        public async Task Build_WithAllThreeIds_ReturnsTheAuthorizableEvent()
        {
            var sut = new AuthorizableEventBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(1, 2, 3);

            result.EventId.ShouldBe(1);
            result.CampaignId.ShouldBe(2);
            result.OrganizationId.ShouldBe(3);
        }

        [Fact]
        public async Task Build_WithJustEventId_ReturnsFromTheCacheWhenAvailable()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set("AuthorizableEvent_1", new FakeAuthorizableEventIdContainer(), TimeSpan.FromMinutes(5));

            var sut = new AuthorizableEventBuilder(Context, cache, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(1);

            result.EventId.ShouldBe(1);
            result.CampaignId.ShouldBe(200);
            result.OrganizationId.ShouldBe(300);
        }

        [Fact]
        public async Task Build_WithJustEventId_ReturnsTheAuthorizableEventFromTheDatabase()
        {
            var sut = new AuthorizableEventBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(10);

            result.EventId.ShouldBe(10);
            result.CampaignId.ShouldBe(20);
            result.OrganizationId.ShouldBe(30);
        }

        [Fact]
        public async Task Build_SetsCaches_WithTheAuthorizableEvent()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());

            var sut = new AuthorizableEventBuilder(Context, cache, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(10);

            IAuthorizableEventIdContainer authorizableEvent;
            var foundInCache = cache.TryGetValue("AuthorizableEvent_10", out authorizableEvent);

            foundInCache.ShouldBeTrue();
            authorizableEvent.EventId.ShouldBe(10);
            authorizableEvent.CampaignId.ShouldBe(20);
            authorizableEvent.OrganizationId.ShouldBe(30);
        }

        private class FakeAuthorizableEventIdContainer : IAuthorizableEventIdContainer
        {
            public int CampaignId => 200;

            public int EventId => 1;

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

            Context.SaveChanges();
        }
    }
}
