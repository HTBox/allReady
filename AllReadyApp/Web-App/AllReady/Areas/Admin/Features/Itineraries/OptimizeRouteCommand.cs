using MediatR;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    /// <summary>
    /// Represents a command message to generate an optimized route for an itinerary's requests
    /// </summary>
    public class OptimizeRouteCommand : IAsyncRequest    
    {
        /// <summary>
        /// The id of itinerary for which the requests will be optimized
        /// </summary>
        public int ItineraryId { get; set; }
    }
}
