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
    public class AuthorizableCampaignBuilderTests : InMemoryContextTest
    {
        [Fact]
        public async Task Build_WithBothIds_ReturnsTheAuthorizableCampaign()
        {
            var sut = new AuthorizableCampaignBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(1, 2);

            result.CampaignId.ShouldBe(1);
            result.OrganizationId.ShouldBe(2);
        }

        [Fact]
        public async Task Build_WithJustCampaignId_ReturnsFromTheCacheWhenAvailable()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set("AuthorizableCampaign_1", new FakeAuthorizableCampaignIdContainer(), TimeSpan.FromMinutes(5));

            var sut = new AuthorizableCampaignBuilder(Context, cache, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(1);

            result.CampaignId.ShouldBe(1);
            result.OrganizationId.ShouldBe(100);
        }

        [Fact]
        public async Task Build_WithJustCampaignId_ReturnsTheAuthorizableCampaignFromTheDatabase()
        {
            var sut = new AuthorizableCampaignBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(20);

            result.CampaignId.ShouldBe(20);
            result.OrganizationId.ShouldBe(30);
        }

        [Fact]
        public async Task Build_SetsCaches_WithTheAuthorizableCampaign()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());

            var sut = new AuthorizableCampaignBuilder(Context, cache, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(20);

            var foundInCache = cache.TryGetValue("AuthorizableCampaign_20", out IAuthorizableCampaignIdContainer AuthorizableCampaign);

            foundInCache.ShouldBeTrue();

            AuthorizableCampaign.CampaignId.ShouldBe(20);
            AuthorizableCampaign.OrganizationId.ShouldBe(30);
        }

        private class FakeAuthorizableCampaignIdContainer : IAuthorizableCampaignIdContainer
        {
            public int CampaignId => 1;

            public int OrganizationId => 100;
        }

        protected override void LoadTestData()
        {
            var org = new Organization { Id = 30 };

            Context.Organizations.Add(org);

            var campaign = new Campaign { Id = 20, ManagingOrganization = org };

            Context.Campaigns.Add(campaign);            

            Context.SaveChanges();
        }
    }
}
