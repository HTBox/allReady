using AllReady.Security;
using MediatR;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    /// <summary>
    /// Uses an <see cref="IAuthorizableItineraryBuilder"/> to build a <see cref="IAuthorizableItinerary"/>
    /// </summary>
    public class AuthorizableItineraryQuery : IAsyncRequest<IAuthorizableItinerary>
    {
        /// <summary>
        /// Initializes a new instance of an <see cref="AuthorizableItineraryQuery"/>.
        /// Uses an <see cref="IAuthorizableItineraryBuilder"/> to build a <see cref="IAuthorizableItinerary"/>
        /// </summary>
        public AuthorizableItineraryQuery(int itineraryId, int? eventId = null, int? campaignId = null, int? orgId = null)
        {
            ItineraryId = itineraryId;
            EventId = eventId;
            CampaignId = campaignId;
            OrganizationId = orgId;
        }

        /// <summary>
        /// The itinerary ID
        /// </summary>
        public int ItineraryId { get; }

        /// <summary>
        /// The event ID for the itinerary, if known
        /// </summary>
        public int? EventId { get; }

        /// <summary>
        /// The organization ID for the itinerary, if known
        /// </summary>
        public int? OrganizationId { get; }

        /// <summary>
        /// The campaign ID for the itinerary, if known
        /// </summary>
        public int? CampaignId { get; }
    }

    /// <summary>
    /// Handles an <see cref="AuthorizableItineraryQuery"/>
    /// </summary>
    public class AuthorizableItineraryQueryHandler : IAsyncRequestHandler<AuthorizableItineraryQuery, IAuthorizableItinerary>
    {
        private readonly IAuthorizableItineraryBuilder _authorizableItineraryBuilder;

        public AuthorizableItineraryQueryHandler(IAuthorizableItineraryBuilder authorizableItineraryBuilder)
        {
            _authorizableItineraryBuilder = authorizableItineraryBuilder;
        }

        /// <summary>
        /// Handles an <see cref="AuthorizableItineraryQuery"/>
        /// </summary>
        public async Task<IAuthorizableItinerary> Handle(AuthorizableItineraryQuery message)
        {
            return await _authorizableItineraryBuilder.Build(message.ItineraryId, message.EventId, message.CampaignId, message.OrganizationId);
        }
    }
}
