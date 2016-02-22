using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Users;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class SiteAdminControllerShould
    {
        [Fact]
        public async Task PutDeleteCommandOnBusWhenDeletingUsers()
        {
            var bus = new Mock<IMediator>();
            var controller = new SiteController(null, null, null, null, bus.Object);
            var userId = "foo_id";

            await controller.ConfirmDeleteUser(userId);
            bus.Verify(b => b.SendAsync(It.Is<DeleteUserCommand>(u => u.UserId == userId)));

        }
    }
}
