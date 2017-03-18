using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using AllReady.Models;

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
        /// Retrieves the IDs for any campaigns the user is allowed to manage
        /// </summary>
        Task<List<int>> GetManagedCampaignIds();

        /// <summary>
        /// Retrieves the IDs for any events the user is allowed to manage
        /// </summary>
        Task<List<int>> GetManagedEventIds();
    }
}