using AllReady.Security;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Security
{
    public class AuthorizableItineraryTests
    {
        #region NoAssociatedUser

        private static async Task<IAuthorizableItinerary> AuthorizableItineraryWhenNoAssociatedUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(false);

            var authEventBuilder = new AuthorizableItineraryBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2, 3, 4);
        }

        [Fact]
        public async Task UserCanView_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableItineraryWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableItineraryWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableItineraryWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanManageRequests_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableItineraryWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanManageRequests();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanManageTeamMembers_ReturnsFalse_WhenUserAuthorizationService_HasNoAssociatedUser()
        {
            var sut = await AuthorizableItineraryWhenNoAssociatedUser();

            var isAuthorized = await sut.UserCanManageTeamMembers();

            isAuthorized.ShouldBeFalse();
        }

        #endregion

        #region SiteAdminUser

        private static async Task<IAuthorizableItinerary> AuthorizableItineraryWhenSiteAdminUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(true);

            var authEventBuilder = new AuthorizableItineraryBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2, 3, 4);
        }

        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableItineraryWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableItineraryWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableItineraryWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageRequests_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableItineraryWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanManageRequests();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageTeamMembers_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsSiteAdmin()
        {
            var sut = await AuthorizableItineraryWhenSiteAdminUser();

            var isAuthorized = await sut.UserCanManageTeamMembers();

            isAuthorized.ShouldBeTrue();
        }

        #endregion

        #region OrgAdminUser

        private static async Task<IAuthorizableItinerary> AuthorizableItineraryWhenOrgAdminUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(true);

            var authEventBuilder = new AuthorizableItineraryBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2, 3, 4);
        }

        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrganizationAdmin_ReturnsTrue()
        {
            var sut = await AuthorizableItineraryWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrgAdmin()
        {
            var sut = await AuthorizableItineraryWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrgAdmin()
        {
            var sut = await AuthorizableItineraryWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageRequests_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrgAdmin()
        {
            var sut = await AuthorizableItineraryWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanManageRequests();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageTeamMembers_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsOrgAdmin()
        {
            var sut = await AuthorizableItineraryWhenOrgAdminUser();

            var isAuthorized = await sut.UserCanManageTeamMembers();

            isAuthorized.ShouldBeTrue();
        }

        #endregion

        #region EventManagerUser

        private static async Task<IAuthorizableItinerary> AuthorizableItineraryWhenEventManagerUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int> { 2 });

            var authEventBuilder = new AuthorizableItineraryBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2, 3, 4);
        }
        
        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsEventManager()
        {
            var sut = await AuthorizableItineraryWhenEventManagerUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsEventManager()
        {
            var sut = await AuthorizableItineraryWhenEventManagerUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsEventManager()
        {
            var sut = await AuthorizableItineraryWhenEventManagerUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageRequests_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsEventManager()
        {
            var sut = await AuthorizableItineraryWhenEventManagerUser();

            var isAuthorized = await sut.UserCanManageRequests();

            isAuthorized.ShouldBeTrue();
        }


        [Fact]
        public async Task UserCanManageTeamMembers_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsEventManager()
        {
            var sut = await AuthorizableItineraryWhenEventManagerUser();

            var isAuthorized = await sut.UserCanManageTeamMembers();

            isAuthorized.ShouldBeTrue();
        }

        #endregion

        #region CampaignManagerUser

        private static async Task<IAuthorizableItinerary> AuthorizableItineraryWhenCampaignManagerUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int> { 3 });

            var authEventBuilder = new AuthorizableItineraryBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2, 3, 4);
        }
        
        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableItineraryWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableItineraryWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableItineraryWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageRequests_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableItineraryWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanManageRequests();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageTeamMembers_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsCampaignManager()
        {
            var sut = await AuthorizableItineraryWhenCampaignManagerUser();

            var isAuthorized = await sut.UserCanManageTeamMembers();

            isAuthorized.ShouldBeTrue();
        }

        #endregion

        #region TeamLeadUser

        private static async Task<IAuthorizableItinerary> AuthorizableItineraryWhenTeamLeadUser()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetLedItineraryIds()).ReturnsAsync(new List<int> { 1 });

            var authEventBuilder = new AuthorizableItineraryBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2, 3, 4);
        }

        [Fact]
        public async Task UserCanView_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsTeamLead()
        {
            var sut = await AuthorizableItineraryWhenTeamLeadUser();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndIsTeamLead()
        {
            var sut = await AuthorizableItineraryWhenTeamLeadUser();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndIsTeamLead()
        {
            var sut = await AuthorizableItineraryWhenTeamLeadUser();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanManageRequests_ReturnsTrue_WhenUserAuthorizationService_HasAssociatedUser_AndIsTeamLead()
        {
            var sut = await AuthorizableItineraryWhenTeamLeadUser();

            var isAuthorized = await sut.UserCanManageRequests();

            isAuthorized.ShouldBeTrue();
        }

        [Fact]
        public async Task UserCanManageTeamMembers_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndIsTeamLead()
        {
            var sut = await AuthorizableItineraryWhenTeamLeadUser();

            var isAuthorized = await sut.UserCanManageTeamMembers();

            isAuthorized.ShouldBeFalse();
        }

        #endregion

        #region NotAuthorizedUser

        private static async Task<IAuthorizableItinerary> AuthorizableItineraryWhenUserAssociatedIsNotAdminOrManager()
        {
            var userAuthService = new Mock<IUserAuthorizationService>();
            userAuthService.Setup(x => x.HasAssociatedUser).Returns(true);
            userAuthService.Setup(x => x.IsSiteAdmin).Returns(false);
            userAuthService.Setup(x => x.IsOrganizationAdmin(It.IsAny<int>())).Returns(false);
            userAuthService.Setup(x => x.GetManagedEventIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetManagedCampaignIds()).ReturnsAsync(new List<int>());
            userAuthService.Setup(x => x.GetLedItineraryIds()).ReturnsAsync(new List<int>());

            var authEventBuilder = new AuthorizableItineraryBuilder(null, new MemoryCache(new MemoryCacheOptions()), userAuthService.Object);

            return await authEventBuilder.Build(1, 2, 3, 4);
        }

        [Fact]
        public async Task UserCanView_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableItineraryWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanView();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanDelete_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableItineraryWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanDelete();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanEdit_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableItineraryWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanEdit();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanManageRequests_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableItineraryWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanManageRequests();

            isAuthorized.ShouldBeFalse();
        }

        [Fact]
        public async Task UserCanManageTeamMembers_ReturnsFalse_WhenUserAuthorizationService_HasAssociatedUser_AndAllOtherChecksAreNotMatched()
        {
            var sut = await AuthorizableItineraryWhenUserAssociatedIsNotAdminOrManager();

            var isAuthorized = await sut.UserCanManageTeamMembers();

            isAuthorized.ShouldBeFalse();
        }

        #endregion
    }
}
