using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Hangfire.MediatR
{
    public class SetRequestsToUnassignedCommandHandler : RequestHandler<SetRequestsToUnassignedCommand>
    {
        private readonly AllReadyContext context;

        public SetRequestsToUnassignedCommandHandler(AllReadyContext context)
        {
            this.context = context;
        }

        protected override void HandleCore(SetRequestsToUnassignedCommand message)
        {
            var requests = context.Requests.Where(x => message.RequestIds.Contains(x.RequestId)).ToList();
            requests.ForEach(request => request.Status = RequestStatus.Unassigned);
            
            var itineraryRequests = context.ItineraryRequests.Where(x => message.RequestIds.Contains(x.RequestId));
            context.ItineraryRequests.RemoveRange(itineraryRequests);

            context.SaveChanges();
        }
    }
}