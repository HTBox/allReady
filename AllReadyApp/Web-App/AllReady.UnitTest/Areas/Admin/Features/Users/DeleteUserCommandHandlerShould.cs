using AllReady.Areas.Admin.Features.Users;
using AllReady.Models;
using Microsoft.AspNet.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Users
{
    public class DeleteUserCommandHandlerShould : InMemoryContextTest
    {

        protected override void LoadTestData()
        {
            var userId = "foo_id";
            var userName = "foo_user";

            var testuser = new ApplicationUser() { UserName = userName, Email = userName, Id = userId };
            var createResult = UserManager.CreateAsync(testuser).Result;
        }

        [Fact]
        public void RemoveUserFromUserManager()
        {
            // arrange
            var handler = new DeleteUserCommandHandler(Context, UserManager);
            var userId = "foo_id";

            // act
            handler.Handle(new DeleteUserCommand { UserId = userId });

            // assert
            var user = UserManager.FindByIdAsync(userId).Result;
            Assert.Null(user);


        }

    }
}
