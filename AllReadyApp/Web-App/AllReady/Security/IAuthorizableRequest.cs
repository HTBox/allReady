using System;
using System.Threading.Tasks;

namespace AllReady.Security
{
    public interface IAuthorizableRequest : IAuthorizable
    {
        /// <summary>
        /// The ID of the request
        /// </summary>
        Guid RequestId { get; }

        /// <summary>
        /// The ID of the itinerary that the request belongs to
        /// </summary>
        int ItineraryId { get; }

        /// <summary>
        /// The ID of the event that the request belongs to
        /// </summary>
        int EventId { get; }

        /// <summary>
        /// The ID of the campaign that the request belongs to
        /// </summary>
        int CampaignId { get; }

        /// <summary>
        /// The ID of the organization that the request belongs to
        /// </summary>
        int OrganizationId { get; }
    }
}
