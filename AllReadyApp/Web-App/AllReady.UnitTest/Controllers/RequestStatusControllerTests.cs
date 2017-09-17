using AllReady.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class RequestStatusControllerTests
    {
        [Fact]
        public async Task NonMatchingRequestId_IndexReturnsNotFoundView() {

            var mockMediator = new Mock<IMediator>();

            var sut = new RequestStatusController(mockMediator.Object);

            var actionResult = await sut.Index(new Guid());

            //  Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }
    }
}
