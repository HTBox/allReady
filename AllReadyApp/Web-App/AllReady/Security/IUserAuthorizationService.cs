using System.Security.Claims;
using System.Threading.Tasks;

namespace AllReady.Security
{
    public interface IUserAuthorizationService
    {
        Task AssociateUser(ClaimsPrincipal userClaimsPrinciple);

        bool HasAssociatedUser { get; }

        string AssociatedUserId { get; }

        Task<bool> CanManageEvent(IAuthorizableEvent authorizableEvent);
    }
}