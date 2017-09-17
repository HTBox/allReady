using System;
using System.Linq;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Models;
using MediatR;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Requests
{
    public class ChangeRequestStatusCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async void UpdateTheRequestWithTheCorrectStatus()
        {
            var request = new Request { RequestId = Guid.NewGuid(), Status = RequestStatus.Unassigned };
            Context.Requests.Add(request);
            Context.SaveChanges();

            var message = new ChangeRequestStatusCommand { RequestId = request.RequestId, NewStatus = RequestStatus.Assigned };

            var sut = new ChangeRequestStatusCommandHandler(Context, Mock.Of<IMediator>());
            await sut.Handle(message);

            var newStatus = Context.Requests.Single(x => x.RequestId == request.RequestId).Status;
            Assert.Equal(newStatus, request.Status);
        }

        [Fact]
        public async void PublishRequestStatusChangedNotificationWithTheCorrectDataOnTheMessage()
        {
            const RequestStatus originalRequestStatus = RequestStatus.Unassigned;
            var originalRequest = new Request { RequestId = Guid.NewGuid(), Status = originalRequestStatus };
            Context.Requests.Add(originalRequest);
            Context.SaveChanges();

            var message = new ChangeRequestStatusCommand { RequestId = originalRequest.RequestId, NewStatus = RequestStatus.Assigned };

            var mediator = new Mock<IMediator>();

            var sut = new ChangeRequestStatusCommandHandler(Context, mediator.Object);
            await sut.Handle(message);

            mediator.Verify(x => x.PublishAsync(It.Is<RequestStatusChangedNotification>(y => y.RequestId == originalRequest.RequestId &&
                y.OldStatus == originalRequestStatus &&
                y.NewStatus == message.NewStatus)));
        }
    }
}
