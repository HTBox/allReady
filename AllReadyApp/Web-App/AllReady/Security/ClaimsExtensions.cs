using System;
using System.Linq;
using AllReady.Models;
using System.Security.Claims;

namespace AllReady.Security
{
    public static class ClaimsExtensions
    {
        public static bool IsUserType(this ClaimsPrincipal user, UserType type)
        {
            string userTypeString = Enum.GetName(typeof(UserType), type);
            return user.HasClaim(ClaimTypes.UserType, userTypeString);
        }

        public static bool IsTenantAdmin(this ClaimsPrincipal user, int tenantId)
        {
            int? userTenantId = user.GetTenantId();
            return user.IsUserType(UserType.SiteAdmin) ||                   
                  (user.IsUserType(UserType.TenantAdmin) &&
                   userTenantId.HasValue && userTenantId.Value == tenantId);
        }

        public static int? GetTenantId(this ClaimsPrincipal user)
        {
            int? result = null;
            var tenantIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Tenant);
            if (tenantIdClaim != null)
            {
                int tenantId;
                if (Int32.TryParse(tenantIdClaim.Value, out tenantId))
                {
                    result = tenantId;
                }
            }

            return result;
        }
    }
}
