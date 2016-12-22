using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class UnlinkedRequestsControllerTests
    {
        [Fact]
        public async void ListReturnsRequestUnathorized_WhenUserIsNotAnOrgAdmin()
        {
            var mediator = new Mock<IMediator>();
            var sut = new UnlinkedRequestsController(mediator.Object);
            sut.MakeUserNotAnOrgAdmin();

            await sut.List();
            Assert.IsType<UnauthorizedResult>(await sut.List());
        }

        [Fact]
        public async void ListCallsRequestListItemsQueryWithUsersOrgId_WhenUserIsOrgAdmin()
        {
            const string orgId = "1001";

            var mediator = new Mock<IMediator>();
            var sut = new UnlinkedRequestsController(mediator.Object);
            sut.MakeUserAnOrgAdmin(orgId);

            await sut.List();

            mediator.Verify(x => x.SendAsync(It.Is<RequestListItemsQuery>(y => y.Criteria.OrganizationId == 1001)), Times.Once);
        }

        [Fact]
        public async void ListCallsRequestListItemsQuerWithRequestStatusUnassigned_WhenUserIsOrgAdmin()
        {
            const string orgId = "1001";

            var mediator = new Mock<IMediator>();
            var sut = new UnlinkedRequestsController(mediator.Object);
            sut.MakeUserAnOrgAdmin(orgId);

            await sut.List();

            mediator.Verify(x => x.SendAsync(It.Is<RequestListItemsQuery>(y => y.Criteria.Status == RequestStatus.Unassigned)), Times.Once);
        }
    }
}
