using AllReady.Areas.Admin.Features.Requests;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Requests
{
    public class SetUnconfirmedRequestsToUnassignedHandler : INotificationHandler<DayOfRequestConfirmationsSent>
    {
        private readonly IMediator mediator;

        public SetUnconfirmedRequestsToUnassignedHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public void Handle(DayOfRequestConfirmationsSent notification)
        {
            notification.RequestIds.ForEach(requestId => mediator.SendAsync(new ChangeRequestStatusCommand { RequestId = requestId, NewStatus = RequestStatus.Unassigned }));
        }
    }
}