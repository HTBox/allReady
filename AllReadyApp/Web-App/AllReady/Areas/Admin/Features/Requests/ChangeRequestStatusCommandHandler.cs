using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            var originalStatus = request.Status;

            request.Status = message.NewStatus;

            if (message.NewStatus == RequestStatus.Unassigned)
            {
                var itineraryRequests = await context.ItineraryRequests
                    .Where(x => x.RequestId == message.RequestId).ToListAsync();                    

                context.ItineraryRequests.RemoveRange(itineraryRequests);
            }

            await context.SaveChangesAsync();

            await mediator.PublishAsync(new RequestStatusChangedNotification { RequestId = message.RequestId, OldStatus = originalStatus, NewStatus = message.NewStatus });
        }
    }
}