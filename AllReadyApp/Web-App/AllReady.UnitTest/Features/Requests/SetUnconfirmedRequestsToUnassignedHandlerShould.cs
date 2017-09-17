using System;
using System.Linq;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Features.Requests;
using AllReady.Models;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Requests
{
    public class SetUnconfirmedRequestsToUnassignedHandlerShould
    {
        [Fact]
        public void SendChangeRequestStatusCommandWithCorrectDataForEachRequestIdOnMessage()
        {
            var message = new DayOfRequestConfirmationsSent { RequestIds = Enumerable.Repeat(1, 2).Select(x => Guid.NewGuid()).ToList() };

            var mediator = new Mock<IMediator>();
            var sut = new SetUnconfirmedRequestsToUnassignedHandler(mediator.Object);

            sut.Handle(message);

            mediator.Verify(x => x.SendAsync(It.Is<ChangeRequestStatusCommand>(y => y.RequestId == message.RequestIds[0] && y.NewStatus == RequestStatus.Unassigned)), Times.Once);
            mediator.Verify(x => x.SendAsync(It.Is<ChangeRequestStatusCommand>(y => y.RequestId == message.RequestIds[1] && y.NewStatus == RequestStatus.Unassigned)), Times.Once);
        }
    }
}
