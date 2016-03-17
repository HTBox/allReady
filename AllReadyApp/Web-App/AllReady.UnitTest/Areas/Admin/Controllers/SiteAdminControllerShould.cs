using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Users;
using MediatR;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class SiteAdminControllerShould
    {
        [Fact]
        public async Task ConfirmDeletUserSendsDeleteUserCommandAsync()
        {
            var mediator = new Mock<IMediator>();
            var controller = new SiteController(null, null, null, null, mediator.Object);
            const string userId = "foo_id";

            await controller.ConfirmDeleteUser(userId);
            mediator.Verify(b => b.SendAsync(It.Is<DeleteUserCommand>(u => u.UserId == userId)));

        }
    }
}
