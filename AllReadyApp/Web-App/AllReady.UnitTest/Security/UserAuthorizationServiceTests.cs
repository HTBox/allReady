using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.Security;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shouldly;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;

namespace AllReady.UnitTest.Security
{
    public class UserAuthorizationServiceTests : InMemoryContextTest
    {
        [Fact]
        public async Task AssociateUser_ShouldCallUserManager_IfClaimsPrincipleIdentityIsAuthenticated()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            userManager.FindByEmailAsyncCallCount.ShouldBe(1);
        }

        [Fact]
        public async Task AssociateUser_ShouldThrowError_IfUserAlreadyAssociated()
        {
            var sut = new UserAuthorizationService(new FakeUserManager(), Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            Exception ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.AssociateUser(new ClaimsPrincipal()));

            ex.ShouldNotBeNull();
        }

        [Fact]
        public async Task HasAssociatedUserShouldReturnTrue_WhenUserAssociated()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.HasAssociatedUser.ShouldBeTrue();
        }

        [Fact]
        public void HasAssociatedUserShouldReturnFalse_WhenNoUserAssociated()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Mock.Of<AllReadyContext>());

            sut.HasAssociatedUser.ShouldBeFalse();
        }

        [Fact]
        public async Task ShouldReturnIdOfTheAssociatedUser()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.AssociatedUserId.ShouldBe("123");
        }
        
        [Fact]
        public void ShouldReturnNull_WhenNoUserAssociated()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Mock.Of<AllReadyContext>());

            sut.AssociatedUserId.ShouldBeNull();
        }

        [Fact]
        public async Task GetManagedEventIds_CallsContextOnFirstLoad()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Context);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            var managedEventIds = await sut.GetManagedEventIds();

            managedEventIds.Count.ShouldBe(1);
        }

        [Fact]
        public async Task GetManagedEventIds_DoesNotCallContextOnSecondLoad()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Context);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            var managedEventIds = await sut.GetManagedEventIds();

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
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Context);
         
            var managedEventIds = await sut.GetManagedEventIds();

            managedEventIds.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetManagedCampaignIds_CallsContextOnFirstLoad()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Context);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            var managedCampaignIds = await sut.GetManagedCampaignIds();

            managedCampaignIds.Count.ShouldBe(1);
        }

        [Fact]
        public async Task GetManagedCampaignIds_DoesNotCallContextOnSecondLoad()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Context);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            var managedCampaignIds = await sut.GetManagedCampaignIds();

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
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Context);

            var managedCampaignIds = await sut.GetManagedCampaignIds();

            managedCampaignIds.ShouldBeEmpty();
        }

        [Fact]
        public async Task IsSiteAdmin_ShouldReturnTrue_WhenClaimsPrincipleHasSiteAdminClaim()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim> { new Claim(AllReady.Security.ClaimTypes.UserType, "SiteAdmin") }, "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.IsSiteAdmin.ShouldBeTrue();
        }
        
        [Fact]
        public async Task IsSiteAdmin_ShouldReturnFalse_WhenClaimsPrincipleDoesNotHaveSiteAdminClaim()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.IsSiteAdmin.ShouldBeFalse();
        }

        [Fact]
        public async Task IsOrganizationAdmin_ShouldReturnTrue_WhenClaimsPrincipleHasOrgAdminClaimAndOrganization()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim> { new Claim(AllReady.Security.ClaimTypes.UserType, "OrgAdmin"), new Claim(AllReady.Security.ClaimTypes.Organization, "1") }, "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.IsOrganizationAdmin(1).ShouldBeTrue();
        }

        [Fact]
        public async Task IsOrganizationAdmin_ShouldReturnFalse_WhenClaimsPrincipleHasOrgAdminClaimAndButDifferentOrganizationId()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim> { new Claim(AllReady.Security.ClaimTypes.UserType, "OrgAdmin"), new Claim(AllReady.Security.ClaimTypes.Organization, "1") }, "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.IsOrganizationAdmin(100).ShouldBeFalse();
        }

        [Fact]
        public async Task IsOrganizationAdmin_ShouldReturnFalse_WhenClaimsPrincipleDoesNotHaveOrgAdminClaim()
        {
            var userManager = new FakeUserManager();

            var sut = new UserAuthorizationService(userManager, Mock.Of<AllReadyContext>());

            var claimsIdentity = new ClaimsIdentity(new List<Claim>(), "CustomApiKeyAuth");

            await sut.AssociateUser(new ClaimsPrincipal(claimsIdentity));

            sut.IsOrganizationAdmin(1).ShouldBeFalse();
        }

        protected override void LoadTestData()
        {
            var user = new ApplicationUser { Id = "123" };

            Context.Users.Add(user);

            var eventManager = new EventManager { User = user, EventId = 1 };

            Context.EventManagers.Add(eventManager);

            var campaignManager = new CampaignManager { User = user, CampaignId = 1 };

            Context.CampaignManagers.Add(campaignManager);

            Context.SaveChanges();
        }

        private class FakeUserManager : UserManager<ApplicationUser>
        {
            public FakeUserManager()
                : base(new Mock<IUserStore<ApplicationUser>>().Object,
                      new Mock<IOptions<IdentityOptions>>().Object,
                      new Mock<IPasswordHasher<ApplicationUser>>().Object,
                      new IUserValidator<ApplicationUser>[0],
                      new IPasswordValidator<ApplicationUser>[0],
                      new Mock<ILookupNormalizer>().Object,
                      new Mock<IdentityErrorDescriber>().Object,
                      new Mock<IServiceProvider>().Object,
                      new Mock<ILogger<UserManager<ApplicationUser>>>().Object)
            { }

            public int FindByEmailAsyncCallCount { get; private set; }

            public override Task<ApplicationUser> FindByEmailAsync(string email)
            {
                FindByEmailAsyncCallCount += 1;
                return Task.FromResult(new ApplicationUser { Id = "123" });
            }
        }
    }
}
