using AllReady.Models;
using AllReady.Security;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Security
{
    public class AuthorizableOrganizationBuilderTests : InMemoryContextTest
    {
        [Fact]
        public async Task Build_WithBothIds_ReturnsTheAuthorizableOrganization()
        {
            var sut = new AuthorizableOrganizationBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(1);

            result.OrganizationId.ShouldBe(1);
        }

        [Fact]
        public async Task Build_WithJustOrganizationId_ReturnsFromTheCacheWhenAvailable()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set("AuthorizableOrganization_1", new FakeAuthorizableOrganizationIdContainer(), TimeSpan.FromMinutes(5));

            var sut = new AuthorizableOrganizationBuilder(Context, cache, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(1);

            result.OrganizationId.ShouldBe(1);
        }

        [Fact]
        public async Task Build_WithJustOrganizationId_ReturnsTheAuthorizableOrganizationFromTheDatabase()
        {
            var sut = new AuthorizableOrganizationBuilder(Context, new MemoryCache(new MemoryCacheOptions()), Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(20);

            result.OrganizationId.ShouldBe(20);
        }

        [Fact]
        public async Task Build_SetsCaches_WithTheAuthorizableOrganization()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());

            var sut = new AuthorizableOrganizationBuilder(Context, cache, Mock.Of<IUserAuthorizationService>());

            var result = await sut.Build(20);

            var foundInCache = cache.TryGetValue("AuthorizableOrganization_20", out IAuthorizableOrganizationIdContainer AuthorizableOrganization);

            foundInCache.ShouldBeTrue();

            AuthorizableOrganization.OrganizationId.ShouldBe(20);
        }

        private class FakeAuthorizableOrganizationIdContainer : IAuthorizableOrganizationIdContainer
        {
            public int OrganizationId => 1;
        }

        protected override void LoadTestData()
        {
            var org = new Organization { Id = 30 };

            Context.Organizations.Add(org);

            var organization = new Organization { Id = 20 };

            Context.Organizations.Add(organization);            

            Context.SaveChanges();
        }
    }
}
