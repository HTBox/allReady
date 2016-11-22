using System;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Features.Requests;
using AllReady.ViewModels.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class RequestApiControllerTests
    {
        //[Fact]
        //public async Task PostReturnsBadRequest_WhenProviderIdIsNull()
        //{
        //    var sut = new RequestApiController(Mock.Of<IMediator>());
        //    var result = await sut.Post(new RequestViewModel {ProviderId = null});

        //    Assert.IsType<BadRequestObjectResult>(result);
        //}

        //[Fact]
        //public async Task PostReturnsBadRequest_WhenRequestIdIsNull()
        //{
        //    var sut = new RequestApiController(Mock.Of<IMediator>());
        //    var result = await sut.Post(new RequestViewModel {RequestId = null});

        //    Assert.IsType<BadRequestObjectResult>(result);
        //}

        //[Fact]
        //public async Task PostReturnsBadRequest_WhenRequestIdIsEmpty()
        //{
        //    var sut = new RequestApiController(Mock.Of<IMediator>());
        //    var result = await sut.Post(new RequestViewModel {RequestId = string.Empty});

        //    Assert.IsType<BadRequestObjectResult>(result);
        //}

        //[Fact]
        //public async Task PostReturnsBadRequest_WhenRequestIdIsNotAGuid()
        //{
        //    var sut = new RequestApiController(Mock.Of<IMediator>());
        //    var result = await sut.Post(new RequestViewModel {RequestId = "NotAGuid"});

        //    Assert.IsType<BadRequestObjectResult>(result);
        //}

        //[Fact]
        //public async Task PostReturnsBadRequest_WhenIncomingStatusCannotBeMappedToRequestStatus()
        //{
        //    var sut = new RequestApiController(Mock.Of<IMediator>());
        //    var result = await sut.Post(new RequestViewModel {Status = "NotMappable"});

        //    Assert.IsType<BadRequestObjectResult>(result);
        //}

        //[Fact]
        //public async Task PostSendsAddRequestCommandWithTheCorrectParameters()
        //{
        //    var mediator = new Mock<IMediator>();
        //    var viewModel = new RequestViewModel {ProviderId = "ProviderId", RequestId = Guid.NewGuid().ToString()};

        //    var sut = new RequestApiController(mediator.Object);
        //    await sut.Post(viewModel);

        //    mediator.Verify(x => x.SendAsync(It.Is<AddApiRequestCommand>((y => y.ViewModel == viewModel))));
        //}

        //[Fact]
        //public async Task PostReturnsCreatedResult()
        //{
        //    var mediator = new Mock<IMediator>();
        //    var viewModel = new RequestViewModel {ProviderId = "ProviderId", RequestId = Guid.NewGuid().ToString()};

        //    var sut = new RequestApiController(mediator.Object);
        //    var result = await sut.Post(viewModel);

        //    Assert.IsType<CreatedResult>(result);
        //}
    }
}
