using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AllReady.Security
{
    public interface IUserAuthorizationService
    {
        Task AssociateUser(ClaimsPrincipal userClaimsPrinciple);

        bool HasAssociatedUser { get; }

        string AssociatedUserId { get; }

        
        bool IsOrgAdmin(int OrgId);

        bool IsSiteAdmin { get; }

        Task<List<int>> GetManagedCampaignIds();

        Task<List<int>> GetManagedEventIds();
    }
}