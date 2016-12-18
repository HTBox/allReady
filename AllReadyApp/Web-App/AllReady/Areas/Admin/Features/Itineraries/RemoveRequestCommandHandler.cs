using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using System.Linq;
using AllReady.Areas.Admin.Features.Requests;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class RemoveRequestCommandHandler : AsyncRequestHandler<RemoveRequestCommand>
    {
        private readonly AllReadyContext context;
        private readonly IMediator mediator;

        public RemoveRequestCommandHandler(AllReadyContext context, IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        protected override async Task HandleCore(RemoveRequestCommand message)
        {
            var itineraryRequests = await context.ItineraryRequests
                .Include(r => r.Itinerary)
                .Include(r => r.Request)
                .Where(r => r.ItineraryId == message.ItineraryId)
                .ToListAsync();

            var requestToRemove = itineraryRequests.FirstOrDefault(r => r.RequestId == message.RequestId);
            var requestStatus = requestToRemove.Request.Status;

            if (requestToRemove == null || requestToRemove.Request.Status == RequestStatus.Completed)
            {
                return;
            }

            // Update the request status
            requestToRemove.Request.Status = RequestStatus.Unassigned;

            // remove the request to itinerary assignment
            context.ItineraryRequests.Remove(requestToRemove);

            var requestsToMoveUp = itineraryRequests.Where(r => r.ItineraryId == message.ItineraryId && r.OrderIndex > requestToRemove.OrderIndex);

            foreach (var requestToMoveUp in requestsToMoveUp)
            {
                requestToMoveUp.OrderIndex--;
            }

            await context.SaveChangesAsync();

            //TODO mgmccarthy: my guess here is that the installer did not get to this Request, and now the Request is no longer assigned to the Itinerary?
            //also, not too sure how to report this status back to GASA. Waiting to hear back from them
            await mediator.PublishAsync(new RequestStatusChangedNotification { RequestId = message.RequestId, OldStatus = requestStatus, NewStatus = RequestStatus.Unassigned });
        }
    }
}