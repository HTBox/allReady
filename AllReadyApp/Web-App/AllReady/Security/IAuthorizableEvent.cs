using System.Threading.Tasks;

namespace AllReady.Security
{
    public interface IAuthorizableEvent : IAuthorizable
    {
        /// <summary>
        /// The ID of the event
        /// </summary>
        int EventId { get; }

        /// <summary>
        /// The ID of the campaign that the event belongs to
        /// </summary>
        int CampaignId { get; }

        /// <summary>
        /// The ID of the organization that the event belongs to
        /// </summary>
        int OrganizationId { get; }

        /// <summary>
        /// Returns the <see cref="EventAccessType"/> for the event of the current user
        /// </summary>
        /// <returns></returns>
        Task<EventAccessType> UserAccessType();
    }
}