using System.Threading.Tasks;
using AllReady.Features.Login;
using AllReady.Models;
using AllReady.UnitTest.Features.Campaigns;
using Xunit;

namespace AllReady.UnitTest.Features.Login
{
    public class ApplicationUserQueryTests : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var username1 = $"bobloblaw@randomdomain.com";
            var username2 = $"someonelse@otherdomain.com";

            var user1 = new ApplicationUser { UserName = username1, Email = username1, EmailConfirmed = true, NormalizedUserName =  username1.ToUpperInvariant() };
            var user2 = new ApplicationUser { UserName = username2, Email = username2, EmailConfirmed = true, NormalizedUserName = username2.ToUpperInvariant() };
            Context.Users.Add(user1);
            Context.Users.Add(user2);
            Context.SaveChanges();
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task QueryUserThatExists()
        {
            var queryHandler = new ApplicationUserQueryHandler(Context);
            var user = await queryHandler.Handle(new ApplicationUserQuery { UserName = "bObLoBlAw@RandomDomain.COM" });

            Assert.NotNull(user);
            Assert.Equal("bobloblaw@randomdomain.com", user.UserName);
        }

        [Fact]
        public async Task QueryUserThatDoesNotExists()
        {
            var queryHandler = new ApplicationUserQueryHandler(Context);
            var user = await queryHandler.Handle(new ApplicationUserQuery { UserName = "nothere@oursite.com" });

            Assert.Null(user);            
        }
    }
}
