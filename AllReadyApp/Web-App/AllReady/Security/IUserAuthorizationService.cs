using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AllReady.Security
{
    public interface IUserAuthorizationService
    {
        /// <summary>
        /// Used by middleware to associate the current user to the <see cref="IUserAuthorizationService"/>
        /// </summary>
        Task AssociateUser(ClaimsPrincipal userClaimsPrinciple);

        /// <summary>
        /// Flags whether there is currently a user associate to the <see cref="IUserAuthorizationService"/>
        /// </summary>
        bool HasAssociatedUser { get; }

        /// <summary>
        /// The user ID of the associated <see cref="ApplicationUser"/>
        /// </summary>
        string AssociatedUserId { get; }

        /// <summary>
        /// Indicates whether the user is an organization admin for the supplied organization id
        /// </summary>
        bool IsOrganizationAdmin(int organizationId);

        /// <summary>
        /// Indicates whether the user has the SiteAdmin claim
        /// </summary>
        bool IsSiteAdmin { get; }

        /// <summary>
        /// Indicates whether the user is an admin for any campaigns
        /// </summary>
        Task<bool> IsCampaignManager();

        /// <summary>
        /// Indicates whether the user is an admin for any events
        /// </summary>
        Task<bool> IsEventManager();

        /// <summary>
        /// Indicates whether the user is a team lead for any itineraries
        /// </summary>
        Task<bool> IsTeamLead();

        /// <summary>
        /// Retrieves the IDs for any campaigns the user is allowed to manage
        /// </summary>
        Task<List<int>> GetManagedCampaignIds();

        /// <summary>
        /// Retrieves the IDs for any events the user is allowed to manage
        /// </summary>
        Task<List<int>> GetManagedEventIds();

        /// <summary>
        /// Retrieves the IDs for any itineraries the user is the team lead for
        /// </summary>
        Task<List<int>> GetLedItineraryIds();

        /// <summary>
        /// Returns the id for the organization the currently assigned user managers. Will be null if user is not an org admin.
        /// </summary>
        int? GetOrganizationId { get; }
    }
}