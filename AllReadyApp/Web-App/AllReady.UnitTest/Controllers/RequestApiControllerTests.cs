using System.Linq;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Features.Requests;
using AllReady.Models;
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
        #region Post
        [Fact]
        public async Task PostReturnsBadRequestResultWhenAnErrorIsNotInternal()
        {
            var mediator = new Mock<IMediator>();
            var outReason = "anythingReally";
            var pid = "someIDValue";

            var error = new AddRequestError
            {
                ProviderId = pid,
                Reason = outReason
            };

            mediator.Setup(x => x.SendAsync(It.IsAny<AddRequestCommand>())).ReturnsAsync(error);

            var sut = new RequestApiController(mediator.Object, null);
            var result = await sut.Post(new RequestViewModel()) as BadRequestObjectResult;

            Assert.NotNull(result);

            var resultObject = result.Value as AddRequestError;

            Assert.NotNull(resultObject);
            Assert.Equal(outReason, resultObject.Reason);
            Assert.Equal(pid, resultObject.ProviderId);
        }

        [Fact]
        public async Task PostSendsAddRequestCommandAsyncWithCorrectData()
        {
            var mediator = new Mock<IMediator>();
            var sut = new RequestApiController(mediator.Object, null);
            var ourViewModel = new RequestViewModel
            {
                ProviderId = "Crazy-Eights",
                ProviderData = "Go-Fish",
                Address = "Address",
                City = "Citty",
                State = "Sttate",
                Zip = "ZZiipp",
                Phone = "Fone",
                Name = "ONAMEO",
                Email = "ANEMAILADDRESS",
                Status = RequestStatus.Assigned.ToString()
            };

            await sut.Post(ourViewModel);

            mediator.Verify(x => x.SendAsync(It.Is<AddRequestCommand>(y => y.Request.ProviderId == ourViewModel.ProviderId
            && y.Request.Status == RequestStatus.Assigned
            && y.Request.ProviderData == ourViewModel.ProviderData
            && y.Request.State == ourViewModel.State
            && y.Request.Address == ourViewModel.Address
            && y.Request.City == ourViewModel.City
            && y.Request.Phone == ourViewModel.Phone
            && y.Request.Zip == ourViewModel.Zip
            && y.Request.Name == ourViewModel.Name
            && y.Request.Email == ourViewModel.Email)), Times.Once);
        }


        [Fact]
        public async Task PostReturnsHttpStatusCodeResultOf201()
        {
            var mediator = new Mock<IMediator>();
            var sut = new RequestApiController(mediator.Object, null);

            var result = await sut.Post(new RequestViewModel()) as CreatedResult;

            Assert.NotNull(result);
            Assert.Equal(result.StatusCode, 201);
        }

        [Fact]
        public void PostHasHttpPostAttribute()
        {
            var sut = new RequestApiController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Post(It.IsAny<RequestViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }
        
        #endregion

        [Fact]
        public void ControllerHasRouteAtttributeWithTheCorrectRoute()
        {
            var sut = new RequestApiController(null, null);
            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Template, "api/request");
        }

        [Fact]
        public void ControllerHasProducesAtttributeWithTheCorrectContentType()
        {
            var sut = new RequestApiController(null, null);
            var attribute = sut.GetAttributes().OfType<ProducesAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.ContentTypes.Select(x => x).First(), "application/json");
        }
    }
}
