using AllReady.Services.Mapping.Routing;
using MediatR;

namespace AllReady.Caching
{
    /// <summary>
    /// Retrieves any available optimize route status for a given user and itinerary
    /// </summary>
    public class OptimizeRouteResultStatusQuery : IRequest<OptimizeRouteResultStatus>
    {
        public OptimizeRouteResultStatusQuery(string userId, int itineraryId)
        {
            UserId = userId;
            ItineraryId = itineraryId;
        }

        /// <summary>
        /// The id of the user to retrieve the optimize route status for
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// The id of the itinerary to retrieve the optimize route status for
        /// </summary>
        public int ItineraryId { get; }
    }
}
