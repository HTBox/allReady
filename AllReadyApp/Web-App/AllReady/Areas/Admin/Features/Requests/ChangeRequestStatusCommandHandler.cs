using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class ChangeRequestStatusCommandHandler : AsyncRequestHandler<ChangeRequestStatusCommand>
    {
        private readonly AllReadyContext context;
        private readonly IMediator mediator;

        public ChangeRequestStatusCommandHandler(AllReadyContext context, IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        protected override async Task HandleCore(ChangeRequestStatusCommand message)
        {
            var request = await context.Requests.SingleAsync(x => x.RequestId == message.RequestId);
            var requestStatus = request.Status;

            request.Status = message.NewStatus;
            await context.SaveChangesAsync();

            await mediator.PublishAsync(new RequestStatusChangedNotification { RequestId = message.RequestId, OldStatus = requestStatus, NewStatus = message.NewStatus });
        }
    }
}