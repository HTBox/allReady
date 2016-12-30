using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class ReorderRequestCommandHandler : IAsyncRequestHandler<ReorderRequestCommand, bool>
    {
        private readonly AllReadyContext _context;

        public ReorderRequestCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ReorderRequestCommand message)
        {
            var itineraryRequests = await _context.ItineraryRequests
                .Include(r => r.Itinerary)
                .Include(r => r.Request)
                .Where(r => r.ItineraryId == message.ItineraryId)
                .OrderBy(r => r.OrderIndex)
                .ToListAsync();

            var requestToMove = itineraryRequests.FirstOrDefault(r => r.RequestId == message.RequestId);
           
            if (message.ReOrderDirection == ReorderRequestCommand.Direction.Up)
            {
                if (requestToMove.OrderIndex == 1)
                    return false;

                var requestAbove = itineraryRequests.FirstOrDefault(r => r.OrderIndex == requestToMove.OrderIndex - 1);

                requestAbove.OrderIndex++;
                requestToMove.OrderIndex--;
            }
            else
            {
                if (itineraryRequests.Last().OrderIndex == requestToMove.OrderIndex)
                {
                    return false;
                }
                
                var requestBelow = itineraryRequests.FirstOrDefault(r => r.OrderIndex == requestToMove.OrderIndex + 1);

                requestBelow.OrderIndex--;
                requestToMove.OrderIndex++;
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}