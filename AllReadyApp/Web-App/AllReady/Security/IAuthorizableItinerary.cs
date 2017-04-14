using System.Threading.Tasks;

namespace AllReady.Security
{
    public interface IAuthorizableItinerary : IAuthorizable
    {
        /// <summary>
        /// The ID of the itinerary
        /// </summary>
        int ItineraryId { get; }

        /// <summary>
        /// The ID of the event that the itinerary belongs to
        /// </summary>
        int EventId { get; }

        /// <summary>
        /// The ID of the campaign that the itinerary belongs to
        /// </summary>
        int CampaignId { get; }

        /// <summary>
        /// The ID of the organization that the itinerary belongs to
        /// </summary>
        int OrganizationId { get; }

        /// <summary>
        /// Indicates the user can manage requests for the itinerary
        /// </summary>
        /// <remarks>
        /// We could limit to action types such as delete/edit etc if our rules differ in each case. For now, this single method is enough based on our rules.
        /// </remarks>
        Task<bool> UserCanManageRequests();

        /// <summary>
        /// Indicates that the user can manage (add/remove) team members for the itinerary
        /// </summary>
        /// <returns></returns>
        Task<bool> UserCanManageTeamMembers();
    }
}
