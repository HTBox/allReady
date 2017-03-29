using System.Threading.Tasks;

namespace AllReady.Security
{
    /// <summary>
    /// Provides access to authorization of the actions which the current user can perform on an event
    /// </summary>
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
        /// Indicates the user can child objects (tasks/events/requests) for the event
        /// </summary>
        /// <remarks>
        /// This can be broken out into sub methods if we require discrete control over managing different type of child object (tasks/requests etc).
        /// We could also limit to action types such as delete/edit etc if our rules differ in each case. For now, this single method is enough based on our rules.
        /// </remarks>
        Task<bool> UserCanManageChildObjects();
    }
}