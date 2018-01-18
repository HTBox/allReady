using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Login;
using AllReady.Models;
using AllReady.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Xunit;

namespace AllReady.UnitTest.Features.Login
{
    public class RemoveUserProfileIncompleteClaimCommandHandlerShould : InMemoryContextTest
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
            var claimForUser1 = new IdentityUserClaim<string>
            {
                UserId = _user1.Id,
                ClaimType = ClaimTypes.ProfileIncomplete,
                ClaimValue = "NewUser"
            };
            Context.UserClaims.Add(claimForUser1);

            var claimForUser2 = new IdentityUserClaim<string>
            {
                UserId = _user2.Id,
                ClaimType = "SomeOtherClaim",
                ClaimValue = "Blah"
            };
            Context.UserClaims.Add(claimForUser2);

            Context.SaveChanges();
        }

        [Fact]
        public async Task RemoveClaimFromUserWithClaim()
        {   
            var commandHandler = new RemoveUserProfileIncompleteClaimCommandHandler(Context);
            await commandHandler.Handle(new RemoveUserProfileIncompleteClaimCommand { UserId = _user1.Id});

            var matchingClaims = Context.UserClaims.Where(u => u.UserId == _user1.Id && u.ClaimType == ClaimTypes.ProfileIncomplete);
            Assert.Empty(matchingClaims);            
        }

        [Fact]
        public async Task RemoveClaimFromUserWithoutClaim()
        {
            var commandHandler = new RemoveUserProfileIncompleteClaimCommandHandler(Context);
            await commandHandler.Handle(new RemoveUserProfileIncompleteClaimCommand { UserId = _user2.Id });

            var matchingClaimForUser1 = Context.UserClaims.Where(u => u.UserId == _user1.Id && u.ClaimType == ClaimTypes.ProfileIncomplete);
            Assert.Single(matchingClaimForUser1);

            var matchingClaimForUser2 = Context.UserClaims.Where(u => u.UserId == _user2.Id && u.ClaimType == "SomeOtherClaim");
            Assert.Single(matchingClaimForUser2);
        }
    }
}
