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
    public class DeleteUserCommandHandlerShould : InMemoryContextTestAsync
    {

        protected override async Task LoadTestData()
        {
            var userId = "foo_id";
            var userName = "foo_user";

            var testuser = new ApplicationUser() { UserName = userName, Email = userName, Id = userId };
            var createResult = await UserManager.CreateAsync(testuser).ConfigureAwait(false);
        }

        [Fact]
        public async Task RemoveUserFromUserManager()
        {
            // arrange
            var handler = new DeleteUserCommandHandler(Context, UserManager);
            var userId = "foo_id";

            // act
            await handler.Handle(new DeleteUserCommand { UserId = userId });

            // assert
            var user = await UserManager.FindByIdAsync(userId);
            Assert.Null(user);


        }

    }
}
