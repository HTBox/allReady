using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.UnlinkedRequests;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class UnlinkedRequestControllerTests
    {
        [Fact]
        public async void ListReturnsRequestUnathorized_WhenUserIsNotAnOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            var sut = new UnlinkedRequestController(mediator.Object);
            sut.MakeUserNotAnOrgAdmin();

            await sut.List();
            Assert.IsType<UnauthorizedResult>(await sut.List());
        }

        [Fact]
        public async void ListCallsRequestListItemsQueryWithUsersOrgId_WhenUserIsOrgAdmin()
        {
            const string orgId = "1001";

            var mediator = new Mock<IMediator>();
            var sut = new UnlinkedRequestController(mediator.Object);
            sut.MakeUserAnOrgAdmin(orgId);

            await sut.List();

            mediator.Verify(x => x.SendAsync(It.Is<UnlinkedRequestListQuery>(y => y.OrganizationId == 1001)), Times.Once);
        }
    }
}
