using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AllReady.Features.Login;
using Xunit;

namespace AllReady.UnitTest.Login
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

        [Fact]
        public async void QueryUserThatExists()
        {
            var queryHandler = new ApplicationUserQueryHandler(Context);
            var user = await queryHandler.Handle(new ApplicationUserQuery { UserName = "bObLoBlAw@RandomDomain.COM" });

            Assert.NotNull(user);
            Assert.Equal("bobloblaw@randomdomain.com", user.UserName);
        }

        [Fact]
        public async void QueryUserThatDoesNotExists()
        {
            var queryHandler = new ApplicationUserQueryHandler(Context);
            var user = await queryHandler.Handle(new ApplicationUserQuery { UserName = "nothere@oursite.com" });

            Assert.Null(user);            
        }
    }
}
