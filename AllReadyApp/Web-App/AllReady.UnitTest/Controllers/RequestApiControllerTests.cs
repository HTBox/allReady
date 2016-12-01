using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Features.Requests;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class RequestApiControllerTests
    {
        [Fact]
        public async Task PostReturnsBadRequest_WhenModelStateIsInvalid()
        {
            var sut = new RequestApiController(Mock.Of<IMediator>());
            sut.AddModelStateError();
            var result = await sut.Post(new RequestApiViewModel());

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PostReturnsBadRequest_WhenStatusIsNotNew()
        {
            var sut = new RequestApiController(Mock.Of<IMediator>());
            var result = await sut.Post(new RequestApiViewModel { Status = "NotNew" });

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PostSendsRequestExistsByProviderIdQueryWithCorrectProviderRequestId()
        {
            const string providerRequestId = "ProviderRequestId";

            var mediator = new Mock<IMediator>();
            var sut = new RequestApiController(mediator.Object);
            await sut.Post(new RequestApiViewModel { Status = "new", ProviderRequestId = providerRequestId });

            mediator.Verify(x => x.SendAsync(It.Is<RequestExistsByProviderIdQuery>(y => y.ProviderRequestId == providerRequestId)), Times.Once);
        }

        [Fact]
        public async Task PostReturnsBadRequest_WhenRequestAlreadyExists()
        {
            const string providerRequestId = "ProviderRequestId";

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<RequestExistsByProviderIdQuery>())).ReturnsAsync(true);

            var sut = new RequestApiController(mediator.Object);
            var result = await sut.Post(new RequestApiViewModel { Status = "new", ProviderRequestId = providerRequestId });

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PostSendsAddApiRequestCommandWithCorrectViewModel()
        {
            var viewModel = new RequestApiViewModel { Status = "new" };
            var mediator = new Mock<IMediator>();

            var sut = new RequestApiController(mediator.Object);
            await sut.Post(viewModel);

            mediator.Verify(x => x.SendAsync(It.Is<ProcessApiRequestCommand>(y => y.ViewModel == viewModel)), Times.Once);
        }

        [Fact]
        public async Task PostReturns202StatusCode()
        {
            var sut = new RequestApiController(Mock.Of<IMediator>());
            var result = await sut.Post(new RequestApiViewModel { Status = "new" }) as StatusCodeResult;

            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(result.StatusCode, 202);
        }
    }
}