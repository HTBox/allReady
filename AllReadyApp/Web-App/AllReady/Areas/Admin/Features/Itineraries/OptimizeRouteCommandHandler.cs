using System;
using System.Collections.Generic;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using AllReady.Services.Mapping.Routing;
using Microsoft.Extensions.Caching.Memory;
using AllReady.Features.Users;
using AllReady.Caching;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    /// <summary>
    /// Handler for <see cref="OptimizeRouteCommand"/>s
    /// </summary>
    public class OptimizeRouteCommandHandler : AsyncRequestHandler<OptimizeRouteCommand>
    {
        private readonly AllReadyContext _context;
        private readonly IOptimizeRouteService _optimizeRouteService;
        private readonly IMemoryCache _cache;

        public OptimizeRouteCommandHandler(AllReadyContext context, IOptimizeRouteService optimizeRouteService, IMemoryCache memoryCache)
        {
            _context = context;
            _optimizeRouteService = optimizeRouteService;
            _cache = memoryCache;
        }

        /// <summary>
        /// Will try to reorder requests based on an optimized route from a 3rd party service. The new order will be persisted to the database
        /// </summary>
        /// <param name="message"></param>
        protected override async Task HandleCore(OptimizeRouteCommand message)
        {
            var requests = await GetRequests(message.ItineraryId);

            if (!requests.Any())
            {
                return;
            }

            var itinerary = requests.First().Itinerary;

            var addresses = new AddressContainer(itinerary);

            if (addresses.HasAddresses)
            {
                var waypoints = requests.Select(req => new OptimizeRouteWaypoint(req.Request.Longitude, req.Request.Latitude, req.RequestId)).ToList();

                var optimizeResult = await _optimizeRouteService.OptimizeRoute(new OptimizeRouteCriteria(addresses.StartAddress, addresses.EndAddress, waypoints));

                if (optimizeResult != null && !optimizeResult.Status.IsSuccess)
                {
                    SetOptimizeCache(message.UserId, message.ItineraryId, optimizeResult.Status);
                }
                else if (optimizeResult?.RequestIds != null && ValidateOptimizedRequests(waypoints, optimizeResult.RequestIds))
                {
                    var originalRequestIds = waypoints.Select(x => x.RequestId);

                    if (originalRequestIds.SequenceEqual(optimizeResult.RequestIds))
                    {
                        SetOptimizeCache(message.UserId, message.ItineraryId, new OptimizeRouteResultStatus { StatusMessage = "Route already most optimal" });
                    }
                    else
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

                        SetOptimizeCache(message.UserId, message.ItineraryId, new OptimizeRouteResultStatus { StatusMessage = "Route optimized" });
                    }
                }
                else
                {
                    SetOptimizeCache(message.UserId, message.ItineraryId, OptimizeRouteResult.FailedOptimizeRouteResult("Route optimization failed").Status);
                }
            }
        }

        private async Task<List<ItineraryRequest>> GetRequests(int itineraryId)
        {
            // todo - we can be more specific about the fields we query for
            return await _context.ItineraryRequests
                .Include(rec => rec.Request)
                .Include(rec => rec.Itinerary).ThenInclude(i => i.StartLocation)
                .Include(rec => rec.Itinerary).ThenInclude(i => i.EndLocation)
                .Where(rec => rec.ItineraryId == itineraryId)
                .ToListAsync();
        }

        private class AddressContainer
        {
            public AddressContainer(Itinerary itinerary)
            {
                if (!string.IsNullOrWhiteSpace(itinerary?.StartLocation?.FullAddress))
                {
                    StartAddress = itinerary.StartLocation.FullAddress;
                    EndAddress = itinerary.EndLocation?.FullAddress;

                    if (itinerary.UseStartAddressAsEndAddress)
                    {
                        EndAddress = StartAddress;
                    }
                }
            }

            public string StartAddress { get; private set; }
            public string EndAddress { get; private set; }

            public bool HasAddresses
            {
                get
                {
                    return (!string.IsNullOrWhiteSpace(StartAddress) && !string.IsNullOrWhiteSpace(EndAddress));
                }
            }
        }

        private void SetOptimizeCache(string userId, int itineraryId, OptimizeRouteResultStatus message)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

                _cache.Set(string.Concat(CacheKeys.OptimizeRouteResultCache, userId, itineraryId), message, cacheEntryOptions);
            }
        }

        /// <summary>
        /// Validates that the request ids in both lists are the same (ignores order)
        /// </summary>
        /// <param name="waypoints"></param>
        /// <param name="optimizedRequestIds"></param>
        /// <returns></returns>
        private static bool ValidateOptimizedRequests(IEnumerable<OptimizeRouteWaypoint> waypoints, IEnumerable<Guid> optimizedRequestIds)
        {
            return optimizedRequestIds.Count() == waypoints.Count() &&
                waypoints.Select(x => x.RequestId).Except(optimizedRequestIds).ToList().Count == 0;
        }
    }
}
