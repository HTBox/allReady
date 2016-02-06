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
        public void PutDeleteCommandOnBusWhenDeletingUsers()
        {
            var bus = new Mock<IMediator>();
            var controller = new SiteController(null, null, null, null, bus.Object);
            var userId = "foo_id";

            controller.ConfirmDeleteUser(userId);
            bus.Verify(b => b.Send(It.Is<DeleteUserCommand>(u => u.UserId == userId)));

        }
    }
}
