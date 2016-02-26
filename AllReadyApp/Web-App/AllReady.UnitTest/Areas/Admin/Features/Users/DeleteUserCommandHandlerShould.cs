using AllReady.Areas.Admin.Features.Users;
using AllReady.Models;
using Microsoft.AspNet.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using MediatR;

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
        public async Task RemoveUserFromUserManager()
        {
            // arrange
            var handler = new DeleteUserCommandHandler(Context, UserManager);
            var userId = "foo_id";

            // act
            await handler.Handle(new DeleteUserCommand { UserId = userId }).ConfigureAwait(false);

            // assert
            var user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
            Assert.Null(user);


        }

    }
}
