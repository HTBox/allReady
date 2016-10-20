using System.Linq;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class SetRequstsToUnassignedCommandHandler : RequestHandler<SetRequstsToUnassignedCommand>
    {
        private readonly AllReadyContext context;

        public SetRequstsToUnassignedCommandHandler(AllReadyContext context)
        {
            this.context = context;
        }

        protected override void HandleCore(SetRequstsToUnassignedCommand message)
        {
            var requests = context.Requests.Where(x => message.RequestIds.Contains(x.RequestId));
            foreach (var request in requests)
            {
                request.Status = RequestStatus.Unassigned;
            }

            var intineraryRequests = context.ItineraryRequests.Where(x => message.RequestIds.Contains(x.RequestId));
            context.ItineraryRequests.RemoveRange(intineraryRequests);

            context.SaveChanges();
        }
    }
}