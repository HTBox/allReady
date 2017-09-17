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
        #region NoAssociatedUser

        private static async Task<IAuthorizableEvent> AuthorizableEventWhenNoAssociatedUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(false);

            var authEventBuilder = new AuthorizableEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2, 3);
        }

        [Fact]
        public async Task UserCanView_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableEventWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableEventWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableEventWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanManageChildObjects_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableEventWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanManageChildObjects();

            isAuthorized.ShouldBeFalse();
        }

        #endregion

        #region SiteAdminUser
        
        private static async Task<IAuthorizableEvent> AuthorizableEventWhenSiteAdminUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(true);

            var authEventBuilder = new AuthorizableEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2, 3);
        }

        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableEventWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableEventWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableEventWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageChildObjects_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableEventWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanManageChildObjects();

            isAuthorized.ShouldBeTrue();
        }

        #endregion
        
        #region OrgAdminUser

        private static async Task<IAuthorizableEvent> AuthorizableEventWhenOrgAdminUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(true);

            var authEventBuilder = new AuthorizableEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2, 3);
        }

        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrganizationAdmin_ReturnsTrue()
        {
            var sut = await AuthorizableEventWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrgAdmin()
        {
            var sut = await AuthorizableEventWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrgAdmin()
        {
            var sut = await AuthorizableEventWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageChildObjects_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrgAdmin()
        {
            var sut = await AuthorizableEventWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanManageChildObjects();

            isAuthorized.ShouldBeTrue();
        }

        #endregion

        #region EventManagerUser

        private static async Task<IAuthorizableEvent> AuthorizableEventWhenEventManagerUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int> { 1 });

            var authEventBuilder = new AuthorizableEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2, 3);
        }
        
        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsEventManager()
        {
            var sut = await AuthorizableEventWhenEventManagerUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndIsEventManager()
        {
            var sut = await AuthorizableEventWhenEventManagerUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsEventManager()
        {
            var sut = await AuthorizableEventWhenEventManagerUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageChildObjects_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsEventManager()
        {
            var sut = await AuthorizableEventWhenEventManagerUser();

            var isAuthorized = await sut.UserCanManageChildObjects();

            isAuthorized.ShouldBeTrue();
        }

        #endregion

        #region CampaignManagerUser

        private static async Task<IAuthorizableEvent> AuthorizableEventWhenCampaignManagerUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int> { 2 });

            var authEventBuilder = new AuthorizableEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2, 3);
        }
        
        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableEventWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableEventWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableEventWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageChildObjects_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableEventWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanManageChildObjects();

            isAuthorized.ShouldBeTrue();
        }

        #endregion

        #region NotAuthorizedUser

        private static async Task<IAuthorizableEvent> AuthorizableEventWhenUserAssociatedIsNotAdminOrManager()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int>());

            var authEventBuilder = new AuthorizableEventBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2, 3);
        }

        [Fact]
        public async Task UserCanView_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableEventWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableEventWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableEventWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanManageChildObjects_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableEventWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanManageChildObjects();

            isAuthorized.ShouldBeFalse();
        }

        #endregion
    }
}
