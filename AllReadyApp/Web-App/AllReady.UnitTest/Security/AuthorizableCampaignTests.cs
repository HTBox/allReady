using AllReady.Security;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Security
{
    public class AuthorizableCampaignTests
    {
        #region NoAssociatedUser

        private static async Task<IAuthorizableCampaign> AuthorizableCampaignWhenNoAssociatedUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(false);

            var authEventBuilder = new AuthorizableCampaignBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2);
        }

        [Fact]
        public async Task UserCanView_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableCampaignWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableCampaignWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableCampaignWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanManageChildObjects_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableCampaignWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanManageChildObjects();

            isAuthorized.ShouldBeFalse();
        }

        #endregion

        #region SiteAdminUser
        
        private static async Task<IAuthorizableCampaign> AuthorizableCampaignWhenSiteAdminUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(true);

            var authEventBuilder = new AuthorizableCampaignBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2);
        }

        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableCampaignWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableCampaignWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableCampaignWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageChildObjects_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableCampaignWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanManageChildObjects();

            isAuthorized.ShouldBeTrue();
        }

        #endregion
        
        #region OrgAdminUser

        private static async Task<IAuthorizableCampaign> AuthorizableCampaignWhenOrgAdminUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(true);

            var authEventBuilder = new AuthorizableCampaignBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2);
        }

        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrganizationAdmin_ReturnsTrue()
        {
            var sut = await AuthorizableCampaignWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrgAdmin()
        {
            var sut = await AuthorizableCampaignWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrgAdmin()
        {
            var sut = await AuthorizableCampaignWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageChildObjects_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrgAdmin()
        {
            var sut = await AuthorizableCampaignWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanManageChildObjects();

            isAuthorized.ShouldBeTrue();
        }

        #endregion
           
        #region CampaignManagerUser

        private static async Task<IAuthorizableCampaign> AuthorizableCampaignWhenCampaignManagerUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int> { 2 });

            var authEventBuilder = new AuthorizableCampaignBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(2, 3);
        }
        
        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableCampaignWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableCampaignWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableCampaignWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageChildObjects_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableCampaignWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanManageChildObjects();

            isAuthorized.ShouldBeTrue();
        }

        #endregion

        #region NotAuthorizedUser

        private static async Task<IAuthorizableCampaign> AuthorizableCampaignWhenUserAssociatedIsNotAdminOrManager()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int>());

            var authEventBuilder = new AuthorizableCampaignBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2);
        }

        [Fact]
        public async Task UserCanView_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableCampaignWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableCampaignWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableCampaignWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanManageChildObjects_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableCampaignWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanManageChildObjects();

            isAuthorized.ShouldBeFalse();
        }

        #endregion
    }
}
