using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class RemoveRequestCommandHandlerAsync : IAsyncRequestHandler<RemoveRequestCommand, bool>
    {
        private readonly AllReadyContext _context;

        public RemoveRequestCommandHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RemoveRequestCommand message)
        {
            var itineraryRequests = await _context.ItineraryRequests
                .Include(r => r.Itinerary)
                .Include(r => r.Request)
                .Where(r => r.ItineraryId == message.ItineraryId)
                .ToListAsync().ConfigureAwait(false);

            var requestToRemove = itineraryRequests.FirstOrDefault(r => r.RequestId == message.RequestId);

            if (requestToRemove == null || requestToRemove.Request.Status == RequestStatus.Completed)
            {
                return false;
            }

            // Update the request status
            requestToRemove.Request.Status = RequestStatus.UnAssigned;        

            // remove the request to itinerary assignment
            _context.ItineraryRequests.Remove(requestToRemove);

            var requestsToMoveUp = itineraryRequests.Where(r => r.ItineraryId == message.ItineraryId && r.OrderIndex > requestToRemove.OrderIndex);

            foreach(var requestToMoveUp in requestsToMoveUp)
            {
                requestToMoveUp.OrderIndex--;
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }
    }
}