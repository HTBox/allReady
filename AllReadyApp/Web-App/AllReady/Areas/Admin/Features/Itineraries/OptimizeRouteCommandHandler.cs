using AllReady.Models;
using AllReady.Services.Routing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class OptimizeRouteCommandHandler : AsyncRequestHandler<OptimizeRouteCommand>
    {
        private readonly AllReadyContext _context;
        private readonly IOptimizeRouteService _optimizeRouteService;

        public OptimizeRouteCommandHandler(AllReadyContext context, IOptimizeRouteService optimizeRouteService)
        {
            _context = context;
            _optimizeRouteService = optimizeRouteService;
        }

        protected override async Task HandleCore(OptimizeRouteCommand message)
        {
            var requests = await _context.ItineraryRequests
                .Include(rec => rec.Request)
                .Include(rec => rec.Itinerary).ThenInclude(i => i.StartLocation)
                .Include(rec => rec.Itinerary).ThenInclude(i => i.EndLocation)
                .Where(rec => rec.ItineraryId == message.ItineraryId)
                .ToListAsync();

            var itinerary = requests.FirstOrDefault()?.Itinerary;

            if (!string.IsNullOrWhiteSpace(itinerary?.StartLocation?.FullAddress))
            {
                var startAddress = itinerary.StartLocation.FullAddress;
                var endAddress = itinerary.EndLocation?.FullAddress;

                if (itinerary.UseStartAddressAsEndAddress)
                {
                    endAddress = startAddress;
                }

                if (!string.IsNullOrWhiteSpace(endAddress))
                {
                    var waypoints = requests.Select(req => new OptimizeRouteWaypoint(req.Request.Longitude, req.Request.Latitude, req.RequestId)).ToList();

                    if (!waypoints.Any()) return;

                    var optimizeResult = await _optimizeRouteService.OptimizeRoute(new OptimizeRouteCriteria(startAddress, endAddress, waypoints));

                    if (optimizeResult != null && optimizeResult.RequestIds.Count == waypoints.Count)
                    {
                        for (var i = 0; i < waypoints.Count; i++)
                        {
                            var itineraryRequest = requests.SingleOrDefault(r => r.RequestId == optimizeResult.RequestIds[i]);

                            if (itineraryRequest != null)
                            {
                                itineraryRequest.OrderIndex = i + 1;
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                }
            }            
        }
    }
}
