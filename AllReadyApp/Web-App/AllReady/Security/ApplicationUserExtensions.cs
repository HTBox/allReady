using System;
using System.Linq;
using AllReady.Models;
using System.Security.Claims;

namespace AllReady.Security
{
    public static class ApplicationUserExtensions
    {
        public static bool IsUserType(this ApplicationUser user, UserType userType)
        {
            return user.Claims.Any(c => c.ClaimType == ClaimTypes.UserType && c.ClaimValue == Enum.GetName(typeof(UserType), userType));
        }
    }
}