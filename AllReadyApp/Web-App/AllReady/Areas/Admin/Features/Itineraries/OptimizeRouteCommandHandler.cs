using System;
using System.Collections.Generic;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using AllReady.Services.Mapping.Routing;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    /// <summary>
    /// Handler for <see cref="OptimizeRouteCommand"/>s
    /// </summary>
    public class OptimizeRouteCommandHandler : AsyncRequestHandler<OptimizeRouteCommand>
    {
        private readonly AllReadyContext _context;
        private readonly IOptimizeRouteService _optimizeRouteService;

        public OptimizeRouteCommandHandler(AllReadyContext context, IOptimizeRouteService optimizeRouteService)
        {
            _context = context;
            _optimizeRouteService = optimizeRouteService;
        }

        /// <summary>
        /// Will try to reorder requests based on an optimized route from a 3rd party service. The new order will be persisted to the database
        /// </summary>
        /// <param name="message"></param>
        protected override async Task HandleCore(OptimizeRouteCommand message)
        {
            var requests = await _context.ItineraryRequests
                .Include(rec => rec.Request)
                .Include(rec => rec.Itinerary).ThenInclude(i => i.StartLocation)
                .Include(rec => rec.Itinerary).ThenInclude(i => i.EndLocation)
                .Where(rec => rec.ItineraryId == message.ItineraryId)
                .ToListAsync();

            if (!requests.Any()) return;

            var itinerary = requests.First().Itinerary;

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
                    var waypoints = requests.Select(req => new OptimizeRouteWaypoint(req.Request.Latitude, req.Request.Longitude, req.RequestId)).ToList();

                    var optimizeResult = await _optimizeRouteService.OptimizeRoute(new OptimizeRouteCriteria(startAddress, endAddress, waypoints));

                    if (optimizeResult?.RequestIds != null && optimizeResult.RequestIds.Count == waypoints.Count && ValidateOptimizedRequests(waypoints, optimizeResult.RequestIds))
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

        /// <summary>
        /// Validated that the request ids in both lists are the same (ignores order)
        /// </summary>
        /// <param name="waypoints"></param>
        /// <param name="optimizedRequestIds"></param>
        /// <returns></returns>
        private static bool ValidateOptimizedRequests(IEnumerable<OptimizeRouteWaypoint> waypoints, IEnumerable<Guid> optimizedRequestIds)
        {
            return waypoints.Select(x => x.RequestId).Except(optimizedRequestIds).ToList().Count == 0;
        }
    }
}
