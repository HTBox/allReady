using AllReady.Security;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Security
{
    public class AuthorizableEventTests
    {
        [Fact]
        public async Task UserAccessType_ReturnsUnauthorized_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(false);

            var authEventBuilder = new AuthorizableEventEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            var sut = await authEventBuilder.Build(1, 2, 3);

            var userType = await sut.UserAccessType();

            userType.ShouldBe(EventAccessType.Unauthorized);
        }

        [Fact]
        public async Task UserAccessType_ReturnsSiteAdmin_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin_ReturnsTrue()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(true);            

            var authEventBuilder = new AuthorizableEventEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            var sut = await authEventBuilder.Build(1, 2, 3);

            var userType = await sut.UserAccessType();

            userType.ShouldBe(EventAccessType.SiteAdmin);
        }

        [Fact]
        public async Task UserAccessType_ReturnsOrganizationAdmin_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrganizationAdmin_ReturnsTrue()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(true);

            var authEventBuilder = new AuthorizableEventEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            var sut = await authEventBuilder.Build(1, 2, 3);

            var userType = await sut.UserAccessType();

            userType.ShouldBe(EventAccessType.OrganizationAdmin);
        }

        [Fact]
        public async Task UserAccessType_ReturnsEventAdmin_WhenUserAuthorizationService_HasAssociatedUser_AndGetManagedEventIds_ContainsEventId()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int> { 1 });

            var authEventBuilder = new AuthorizableEventEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            var sut = await authEventBuilder.Build(1, 2, 3);

            var userType = await sut.UserAccessType();

            userType.ShouldBe(EventAccessType.EventAdmin);
        }

        [Fact]
        public async Task UserAccessType_ReturnsCampaignAdmin_WhenUserAuthorizationService_HasAssociatedUser_AndGetManagedCampaignIds_ContainsCampaignId()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int> { 2 });

            var authEventBuilder = new AuthorizableEventEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            var sut = await authEventBuilder.Build(1, 2, 3);

            var userType = await sut.UserAccessType();

            userType.ShouldBe(EventAccessType.CampaignAdmin);
        }

        [Fact]
        public async Task UserAccessType_ReturnsUnknown_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int>());

            var authEventBuilder = new AuthorizableEventEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            var sut = await authEventBuilder.Build(1, 2, 3);

            var userType = await sut.UserAccessType();

            userType.ShouldBe(EventAccessType.Unknown);
        }

        [Fact]
        public async Task IsUserAuthorized_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(false);

            var authEventBuilder = new AuthorizableEventEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            var sut = await authEventBuilder.Build(1, 2, 3);

            var isAuthorized = await sut.IsUserAuthorized();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task IsUserAuthorized_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin_ReturnsTrue()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(true);

            var authEventBuilder = new AuthorizableEventEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            var sut = await authEventBuilder.Build(1, 2, 3);

            var isAuthorized = await sut.IsUserAuthorized();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task IsUserAuthorized_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrganizationAdmin_ReturnsTrue()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(true);

            var authEventBuilder = new AuthorizableEventEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            var sut = await authEventBuilder.Build(1, 2, 3);

            var isAuthorized = await sut.IsUserAuthorized();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task IsUserAuthorized_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndGetManagedEventIds_ContainsEventId()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int> { 1 });

            var authEventBuilder = new AuthorizableEventEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            var sut = await authEventBuilder.Build(1, 2, 3);

            var isAuthorized = await sut.IsUserAuthorized();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task IsUserAuthorized_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndGetManagedCampaignIds_ContainsCampaignId()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int> { 2 });

            var authEventBuilder = new AuthorizableEventEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            var sut = await authEventBuilder.Build(1, 2, 3);

            var isAuthorized = await sut.IsUserAuthorized();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task IsUserAuthorized_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int>());

            var authEventBuilder = new AuthorizableEventEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            var sut = await authEventBuilder.Build(1, 2, 3);

            var isAuthorized = await sut.IsUserAuthorized();

            isAuthorized.ShouldBeFalse();
        }
    }
}
