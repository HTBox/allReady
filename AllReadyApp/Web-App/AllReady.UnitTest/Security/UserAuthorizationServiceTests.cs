using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.Security;
using Moq;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Security
{
    public class UserAuthorizationServiceTests : InMemoryContextTest
    { 
        public static string UserId => "123";
        public static string UserId2 => "1234";

        [Fact]
        public async Task AssociateUser_ShouldInvokeFindByEmailAsyncWithTheCorrectEmail_WhenClaimsPrincipleIdentityIsAuthenticated()
        {
            const string email = "email";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var claimsIdentity = new ClaimsIdentity(new List<Claim> { new Claim(System.Security.Claims.ClaimTypes.Name, email) }, "CustomApiKeyAuth");

            var sut = new UserAuthorizationService(userManager.Object, Mock.Of<AllReadyContext>());

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            userManager.Verify(x => x.FindByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task AssociateUser_ShouldInvokeFindByEmailAsyncOnce_WhenAssociateUserIsInvokedMoreThanOnceWithSameClaimsPrincipal()
        {
            const string email = "Email";
            const string authentciationType = "CustomApiKeyAuth";
            
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser { Email = email });

            var sut = new UserAuthorizationService(userManager.Object, Mock.Of<AllReadyContext>());

            var claimsIdentity1 = new ClaimsIdentity(new List<Claim>(), authentciationType);
            claimsIdentity1.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, email));

            var claimsIdentity2 = new ClaimsIdentity(new List<Claim>(), authentciationType);
            claimsIdentity2.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, email));

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity1));
            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity2));

            userManager.Verify(x => x.FindByEmailAsync(email), Times.Exactly(1));
        }

        [Fact]
        public async Task AssociateUser_ShouldThrowError_IfUserAlreadyAssociatedWithDifferentEmail()
        {
            const string email = "Email";
            const string authentciationType = "CustomApiKeyAuth";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(new ApplicationUser());

            var sut = new UserAuthorizationService(userManager.Object, Mock.Of<AllReadyContext>());

            var claimsIdentity1 = new ClaimsIdentity(new List<Claim>(), authentciationType);
            claimsIdentity1.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, email));

            var claimsIdentity2 = new ClaimsIdentity(new List<Claim>(), authentciationType);
            claimsIdentity2.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, email));

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity1));

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.AssociateUser(new ClaimsPrincipal(claimsIdentity2)));
        }

        [Fact]
        public async Task HasAssociatedUserShouldReturnTrue_WhenUserAssociated()
        {
            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

            var sut = new UserAuthorizationService(userManager.Object, Mock.Of<AllReadyContext>());
            
            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.HasAssociatedUser.ShouldBeTrue();
        }

        [Fact]
        public void HasAssociatedUserShouldReturnFalse_WhenNoUserAssociated()
        {
            var sut = new UserAuthorizationService(UserManagerMockHelper.CreateUserManagerMock().Object, Mock.Of<AllReadyContext>());
            sut.HasAssociatedUser.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldReturnIdOfTheAssociatedUser()
        {

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId });

            var sut = new UserAuthorizationService(userManager.Object, Mock.Of<AllReadyContext>());
            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.AssociatedUserId.ShouldBe(UserId);
        }

        [Fact]
        public void ShouldReturnNull_WhenNoUserAssociated()
        {
            var sut = new UserAuthorizationService(UserManagerMockHelper.CreateUserManagerMock().Object, Mock.Of<AllReadyContext>());
            sut.AssociatedUserId.ShouldBeNull();
        }

        [Fact]
        public async Task IsEventManager_ReturnsTrue_WhenUserHasManagedEventRecord()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId });

            var sut = new UserAuthorizationService(userManager.Object, Context);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");
            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            var isEventManager = await sut.IsEventManager();
            isEventManager.ShouldBe(true);
        }

        [Fact]
        public async Task IsEventManager_ReturnsFalse_WhenUserHasNoManagedEventRecords()
        {
            var sut = new UserAuthorizationService(UserManagerMockHelper.CreateUserManagerMock().Object, Context);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            var isEventManager = await sut.IsEventManager();

            isEventManager.ShouldBe(false);
        }

        [Fact]
        public async Task IsEventManager_ReturnsFalse_WhenNoUserAssociated()
        {
            var sut = new UserAuthorizationService(UserManagerMockHelper.CreateUserManagerMock().Object, Context);
            
            var isEventManager = await sut.IsEventManager();

            isEventManager.ShouldBe(false);
        }

        [Fact]
        public async Task IsCampaignManager_ReturnsTrue_WhenUserHasManagedCampaignRecord()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId });

            var sut = new UserAuthorizationService(userManager.Object, Context);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            var isEventManager = await sut.IsCampaignManager();

            isEventManager.ShouldBe(true);
        }

        [Fact]
        public async Task IsCampaignManager_ReturnsFalse_WhenUserHasNoManagedCampaignRecords()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId2 });
            var sut = new UserAuthorizationService(userManager.Object, Context);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));
            var isEventManager = await sut.IsCampaignManager();

            isEventManager.ShouldBe(false);
        }

        [Fact]
        public async Task IsCampaignManager_ReturnsFalse_WhenNoUserAssociated()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

            var sut = new UserAuthorizationService(userManager.Object, Context);
            var isEventManager = await sut.IsCampaignManager();

            isEventManager.ShouldBe(false);
        }

        [Fact]
        public async Task GetManagedEventIds_ReturnsExpectedEventIdsForUser()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId});
            var sut = new UserAuthorizationService(userManager.Object, Context);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));
            var managedEventIds = await sut.GetManagedEventIds();

            managedEventIds.Count.ShouldBe(1);
        }

        [Fact]
        public async Task GetManagedEventIds_DoesNotCallContextOnSecondLoad()
        {
            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId });
            var sut = new UserAuthorizationService(userManager.Object, Context);

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));
            await sut.GetManagedEventIds();

            var manager = Context.EventManagers.First();

            Context.Remove(manager);
            Context.SaveChanges();

            Context.EventManagers.Count().ShouldBe(0);

            var managedEventIds2 = await sut.GetManagedEventIds();

            managedEventIds2.Count.ShouldBe(1);
        }

        [Fact]
        public async Task GetManagedEventIds_ShouldReturnEmptyListWhenNoAssociatedUser()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
            var sut = new UserAuthorizationService(userManager.Object, Context);

            var managedEventIds = await sut.GetManagedEventIds();

            managedEventIds.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetManagedCampaignIds_CallsContextOnFirstLoad()
        {
            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId});

            var sut = new UserAuthorizationService(userManager.Object, Context);
            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            var managedCampaignIds = await sut.GetManagedCampaignIds();

            managedCampaignIds.Count.ShouldBe(1);
        }

        [Fact]
        public async Task GetManagedCampaignIds_DoesNotCallContextOnSecondLoad()
        {
            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId });
            var sut = new UserAuthorizationService(userManager.Object, Context);

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));
            await sut.GetManagedCampaignIds();

            var manager = Context.CampaignManagers.First();

            Context.Remove(manager);
            Context.SaveChanges();

            Context.CampaignManagers.Count().ShouldBe(0);

            var managedCampaignIds2 = await sut.GetManagedCampaignIds();

            managedCampaignIds2.Count.ShouldBe(1);
        }

        [Fact]
        public async Task GetManagedCampaignIds_ShouldReturnEmptyListWhenNoAssociatedUser()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var sut = new UserAuthorizationService(userManager.Object, Context);

            var managedCampaignIds = await sut.GetManagedCampaignIds();

            managedCampaignIds.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetLedItineraryIds_CallsContextOnFirstLoad()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId });
            var sut = new UserAuthorizationService(userManager.Object, Context);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            var teamLeadIds = await sut.GetLedItineraryIds();

            teamLeadIds.Count.ShouldBe(1);
        }

        [Fact]
        public async Task GetLedItineraryIds_DoesNotCallContextOnSecondLoad()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId });
            var sut = new UserAuthorizationService(userManager.Object, Context);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));
            await sut.GetLedItineraryIds();

            var manager = Context.VolunteerTaskSignups.First();

            Context.Remove(manager);
            Context.SaveChanges();

            Context.VolunteerTaskSignups.Count().ShouldBe(0);

            var teamLeadIds2 = await sut.GetLedItineraryIds();

            teamLeadIds2.Count.ShouldBe(1);
        }

        [Fact]
        public async Task GetLedItineraryIds_ShouldReturnEmptyListWhenNoAssociatedUser()
        {
            var sut = new UserAuthorizationService(UserManagerMockHelper.CreateUserManagerMock().Object, Context);

            var teamLeadIds = await sut.GetLedItineraryIds();

            teamLeadIds.ShouldBeEmpty();
        }

        [Fact]
        public async Task IsTeamLead_ReturnsTrue_WhenUserHasTeamLeadTaskSignupRecord()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId });
            var sut = new UserAuthorizationService(userManager.Object, Context);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            var isEventManager = await sut.IsTeamLead();

            isEventManager.ShouldBe(true);
        }

        [Fact]
        public async Task IsTeamLead_ReturnsFalse_WhenUserHasNoTeamLeadTaskSignupRecords()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
            var sut = new UserAuthorizationService(userManager.Object, Context);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            var isEventManager = await sut.IsTeamLead();

            isEventManager.ShouldBe(false);
        }

        [Fact]
        public async Task IsTeamLead_ReturnsFalse_WhenNoUserAssociated()
        {
            var sut = new UserAuthorizationService(UserManagerMockHelper.CreateUserManagerMock().Object, Context);

            var isEventManager = await sut.IsTeamLead();

            isEventManager.ShouldBe(false);
        }

        [Fact]
        public async Task IsSiteAdmin_ShouldReturnTrue_WhenClaimsPrincipleHasSiteAdminClaim()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId });
            var sut = new UserAuthorizationService(userManager.Object, Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim> { new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.SiteAdmin)) }, "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.IsSiteAdmin.ShouldBeTrue();
        }

        [Fact]
        public async Task IsSiteAdmin_ShouldReturnFalse_WhenClaimsPrincipleDoesNotHaveSiteAdminClaim()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
            var sut = new UserAuthorizationService(userManager.Object, Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.IsSiteAdmin.ShouldBeFalse();
        }

        [Fact]
        public async Task IsOrganizationAdmin_ShouldReturnTrue_WhenClaimsPrincipleHasOrgAdminClaimAndOrganization()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId });
            var sut = new UserAuthorizationService(userManager.Object, Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim> { new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin)), new Claim(AllReady.Security.ClaimTypes.Organization, "1") }, "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.IsOrganizationAdmin(1).ShouldBeTrue();
        }

        [Fact]
        public async Task IsOrganizationAdmin_ShouldReturnFalse_WhenClaimsPrincipleHasOrgAdminClaimAndButDifferentOrganizationId()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = UserId });
            var sut = new UserAuthorizationService(userManager.Object, Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim> { new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin)), new Claim(AllReady.Security.ClaimTypes.Organization, "1") }, "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.IsOrganizationAdmin(100).ShouldBeFalse();
        }

        [Fact]
        public async Task IsOrganizationAdmin_ShouldReturnFalse_WhenClaimsPrincipleDoesNotHaveOrgAdminClaim()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
            var sut = new UserAuthorizationService(userManager.Object, Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.IsOrganizationAdmin(1).ShouldBeFalse();
        }

        protected override void LoadTestData()
        {
            var user = new ApplicationUser { Id = UserId };
            Context.Users.Add(user);

            var user2 = new ApplicationUser { Id = UserId2 };
            Context.Users.Add(user2);

            var eventManager = new EventManager { User = user, EventId = 1 };
            Context.EventManagers.Add(eventManager);

            var campaignManager = new CampaignManager { User = user, CampaignId = 1 };
            Context.CampaignManagers.Add(campaignManager);

            var volunteerTaskSignup = new VolunteerTaskSignup { User = user, IsTeamLead = true, ItineraryId = 1 };
            Context.VolunteerTaskSignups.Add(volunteerTaskSignup);

            Context.SaveChanges();
        }
    }
}