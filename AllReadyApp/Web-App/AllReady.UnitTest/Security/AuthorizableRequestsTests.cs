using AllReady.Security;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Security
{
    public class AuthorizableRequestTests
    {
        #region NoAssociatedUser

        private static async Task<IAuthorizableRequest> AuthorizableRequestWhenNoAssociatedUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(false);

            var authEventBuilder = new AuthorizableRequestBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(Guid.NewGuid(), 2, 3, 4, 5);
        }

        [Fact]
        public async Task UserCanView_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableRequestWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableRequestWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableRequestWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeFalse();
        }      

        #endregion

        #region SiteAdminUser

        private static async Task<IAuthorizableRequest> AuthorizableRequestWhenSiteAdminUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(true);

            var authEventBuilder = new AuthorizableRequestBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(Guid.NewGuid(), 2, 3, 4, 5);
        }

        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableRequestWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableRequestWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableRequestWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        #endregion

        #region OrgAdminUser

        private static async Task<IAuthorizableRequest> AuthorizableRequestWhenOrgAdminUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(true);

            var authEventBuilder = new AuthorizableRequestBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(Guid.NewGuid(), 2, 3, 4, 5);
        }

        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrganizationAdmin_ReturnsTrue()
        {
            var sut = await AuthorizableRequestWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrgAdmin()
        {
            var sut = await AuthorizableRequestWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrgAdmin()
        {
            var sut = await AuthorizableRequestWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        #endregion

        #region EventManagerUser

        private static async Task<IAuthorizableRequest> AuthorizableRequestWhenEventManagerUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int> { 3 });

            var authEventBuilder = new AuthorizableRequestBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(Guid.NewGuid(), 2, 3, 4, 5);
        }
        
        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsEventManager()
        {
            var sut = await AuthorizableRequestWhenEventManagerUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsEventManager()
        {
            var sut = await AuthorizableRequestWhenEventManagerUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsEventManager()
        {
            var sut = await AuthorizableRequestWhenEventManagerUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        #endregion

        #region CampaignManagerUser

        private static async Task<IAuthorizableRequest> AuthorizableRequestWhenCampaignManagerUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int> { 4 });

            var authEventBuilder = new AuthorizableRequestBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(Guid.NewGuid(), 2, 3, 4, 5);
        }
        
        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableRequestWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableRequestWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableRequestWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        #endregion

        #region TeamLeadUser

        private static async Task<IAuthorizableRequest> AuthorizableRequestWhenTeamLeadUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetLedItineraryIds()).ReturnsAsync(new List<int> { 2 });

            var authEventBuilder = new AuthorizableRequestBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(Guid.NewGuid(), 2, 3, 4, 5);
        }

        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsTeamLead()
        {
            var sut = await AuthorizableRequestWhenTeamLeadUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsTeamLead()
        {
            var sut = await AuthorizableRequestWhenTeamLeadUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsTeamLead()
        {
            var sut = await AuthorizableRequestWhenTeamLeadUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        #endregion

        #region NotAuthorizedUser

        private static async Task<IAuthorizableRequest> AuthorizableRequestWhenUserAssociatedIsNotAdminOrManager()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetLedItineraryIds()).ReturnsAsync(new List<int>());

            var authEventBuilder = new AuthorizableRequestBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(Guid.NewGuid(), 2, 3, 4, 5);
        }

        [Fact]
        public async Task UserCanView_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableRequestWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableRequestWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableRequestWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeFalse();
        }

        #endregion
    }
}
