using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Features.Requests;
using AllReady.Hangfire.Jobs;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels.Requests;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
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
            var sut = new RequestApiController(Mock.Of<IMediator>(), null);
            sut.AddModelStateError();
            var result = await sut.Post(new RequestApiViewModel());

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PostReturnsBadRequest_WhenStatusIsNotNew()
        {
            var sut = new RequestApiController(Mock.Of<IMediator>(), null);
            var result = await sut.Post(new RequestApiViewModel { Status = "NotNew" });

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PostSendsRequestExistsByProviderIdQueryWithCorrectProviderRequestId()
        {
            const string providerRequestId = "ProviderRequestId";

            var mediator = new Mock<IMediator>();
            var sut = new RequestApiController(mediator.Object, Mock.Of<IBackgroundJobClient>());
            await sut.Post(new RequestApiViewModel { Status = "new", ProviderRequestId = providerRequestId });

            mediator.Verify(x => x.SendAsync(It.Is<RequestExistsByProviderIdQuery>(y => y.ProviderRequestId == providerRequestId)), Times.Once);
        }

        [Fact]
        public async Task PostReturnsBadRequest_WhenRequestAlreadyExists()
        {
            const string providerRequestId = "ProviderRequestId";

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<RequestExistsByProviderIdQuery>())).ReturnsAsync(true);

            var sut = new RequestApiController(mediator.Object, null);
            var result = await sut.Post(new RequestApiViewModel { Status = "new", ProviderRequestId = providerRequestId });

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PostEnqueuesProcessApiRequestsJobWithCorrectViewModel()
        {
            var viewModel = new RequestApiViewModel { Status = "new" };
            var mediator = new Mock<IMediator>();
            var backgroundJobClient = new Mock<IBackgroundJobClient>();

            var sut = new RequestApiController(mediator.Object, backgroundJobClient.Object);
            await sut.Post(viewModel);

            backgroundJobClient.Verify(x => x.Create(It.Is<Job>(job => 
                job.Method.Name == nameof(ProcessApiRequests.Process) && 
                job.Args[0] == viewModel), It.IsAny<EnqueuedState>()), Times.Once);
        }

        [Fact]
        public async Task PostReturns202StatusCode()
        {
            var sut = new RequestApiController(Mock.Of<IMediator>(), Mock.Of<IBackgroundJobClient>());
            var result = await sut.Post(new RequestApiViewModel { Status = "new" }) as StatusCodeResult;

            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(result.StatusCode, 202);
        }
    }
}