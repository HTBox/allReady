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
        /// Indicates the user can create requests for the event
        /// </summary>
        Task<bool> UserCanCreateRequests();

        /// <summary>
        /// Indicates the user can create itineraries for the event
        /// </summary>
        Task<bool> UserCanCreateItineraries();
        
        /// <summary>
        /// Indicates the user can create tasks for the event
        /// </summary>
        Task<bool> UserCanCreateTasks();
    }
}