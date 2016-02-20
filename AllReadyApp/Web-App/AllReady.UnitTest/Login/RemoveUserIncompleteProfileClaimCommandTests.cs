using AllReady.Models;
using System.Linq;
using AllReady.Features.Login;
using Xunit;
using Microsoft.AspNet.Identity.EntityFramework;
using AllReady.Security;

namespace AllReady.UnitTest.Login
{
    public class RemoveUserIncompleteProfileClaimCommandTests : InMemoryContextTest
    {
        private ApplicationUser _user1;
        private ApplicationUser _user2;

        protected override void LoadTestData()
        {
            var username1 = $"bobloblaw@randomdomain.com";
            var username2 = $"someonelse@otherdomain.com";

            _user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true, NormalizedUserName =  username1.ToUpperInvariant() };
            _user2 = new ApplicationUser { UserName = username2, Email = username2, EmailConfirmed = true, NormalizedUserName = username2.ToUpperInvariant() };
            Context.Users.Add(_user1);
            Context.Users.Add(_user2);
            var claimForUser1 = new IdentityUserClaim<string>();
            claimForUser1.UserId = _user1.Id;
            claimForUser1.ClaimType = ClaimTypes.ProfileIncomplete;
            claimForUser1.ClaimValue = "NewUser";
            Context.UserClaims.Add(claimForUser1);

            var claimForUser2 = new IdentityUserClaim<string>();
            claimForUser2.UserId = _user2.Id;
            claimForUser2.ClaimType = "SomeOtherClaim";
            claimForUser2.ClaimValue = "Blah";
            Context.UserClaims.Add(claimForUser2);

            Context.SaveChanges();
        }

        [Fact]
        public async void RemoveClaimFromUserWithClaim()
        {   
            var commandHandler = new RemoveUserProfileIncompleteClaimCommandHandler(Context);
            await commandHandler.Handle(new RemoveUserProfileIncompleteClaimCommand { UserId = _user1.Id});

            var matchingClaims = Context.UserClaims.Where(u => u.UserId == _user1.Id && u.ClaimType == ClaimTypes.ProfileIncomplete).Count();
            Assert.Equal(0, matchingClaims);            
        }

        [Fact]
        public async void RemoveClaimFromUserWithoutClaim()
        {
            var commandHandler = new RemoveUserProfileIncompleteClaimCommandHandler(Context);
            await commandHandler.Handle(new RemoveUserProfileIncompleteClaimCommand { UserId = _user2.Id });

            var matchingClaimForUser1 = Context.UserClaims.Where(u => u.UserId == _user1.Id && u.ClaimType == ClaimTypes.ProfileIncomplete).Count();
            Assert.Equal(1, matchingClaimForUser1);

            var matchingClaimForUser2 = Context.UserClaims.Where(u => u.UserId == _user2.Id && u.ClaimType == "SomeOtherClaim").Count();
            Assert.Equal(1, matchingClaimForUser2);

        }
    }
}
