using System.Threading.Tasks;

namespace AllReady.Security
{
    /// <summary>
    /// Builder for <see cref="IAuthorizableItinerary"/>s
    /// </summary>
    public interface IAuthorizableItineraryBuilder
    {
        /// <summary>
        /// Builds an instance of an <see cref="IAuthorizableItinerary"/>
        /// </summary>
        Task<IAuthorizableItinerary> Build(int itineraryId, int? eventId = null, int? campaignId = null, int? orgId = null);
    }
}
