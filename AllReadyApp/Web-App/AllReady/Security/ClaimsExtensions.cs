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

        public static int GetTenantId(this ClaimsPrincipal user)
        {            
            return Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Tenant).Value);
        }
    }
}
