using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class RemoveRequestCommandHandler : AsyncRequestHandler<RemoveRequestCommand>
    {
        private readonly AllReadyContext _context;

        public RemoveRequestCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override async Task HandleCore(RemoveRequestCommand message)
        {
            var itineraryRequests = await _context.ItineraryRequests
                .Include(r => r.Itinerary)
                .Include(r => r.Request)
                .Where(r => r.ItineraryId == message.ItineraryId)
                .ToListAsync();

            var requestToRemove = itineraryRequests.FirstOrDefault(r => r.RequestId == message.RequestId);

            if (requestToRemove == null || requestToRemove.Request.Status == RequestStatus.Completed)
            {
                return;
            }

            // Update the request status
            requestToRemove.Request.Status = RequestStatus.Unassigned;

            // remove the request to itinerary assignment
            _context.ItineraryRequests.Remove(requestToRemove);

            var requestsToMoveUp = itineraryRequests.Where(r => r.ItineraryId == message.ItineraryId && r.OrderIndex > requestToRemove.OrderIndex);

            foreach (var requestToMoveUp in requestsToMoveUp)
            {
                requestToMoveUp.OrderIndex--;
            }

            await _context.SaveChangesAsync();
        }
    }
}