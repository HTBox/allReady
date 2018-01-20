using System;
using System.Linq;
using System.Security.Claims;
using AllReady.Models;
using TimeZoneConverter;
using Microsoft.AspNetCore.Identity;

namespace AllReady.Security
{
    public static class ClaimsExtensions
    {
        public static bool IsUserType(this ClaimsPrincipal user, UserType type)
        {
            var userTypeString = Enum.GetName(typeof(UserType), type);
            return user.HasClaim(ClaimTypes.UserType, userTypeString);
        }

        public static bool IsOrganizationAdmin(this ClaimsPrincipal user)
        {
            var userOrganizationId = user.GetOrganizationId();
            return userOrganizationId.HasValue && user.IsOrganizationAdmin(userOrganizationId.Value);
        }

        public static bool IsOrganizationAdmin(this ClaimsPrincipal user, int organizationId)
        {
            var userOrganizationId = user.GetOrganizationId();
            return user.IsUserType(UserType.SiteAdmin) ||
                  (user.IsUserType(UserType.OrgAdmin) && userOrganizationId.HasValue && userOrganizationId.Value == organizationId);
        }

        public static int? GetOrganizationId(this ClaimsPrincipal user)
        {
            int? result = null;
            var organizationIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Organization);
            if (organizationIdClaim != null)
            {
                int organizationId;
                if (int.TryParse(organizationIdClaim.Value, out organizationId))
                    result = organizationId;
                }

            return result;
        }

        public static string GetTimeZoneId(this ClaimsPrincipal user)
        {
            var timeZoneIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.TimeZoneId);
            return timeZoneIdClaim == null ? "UTC" : timeZoneIdClaim.Value;
        }

        public static bool IsUserProfileIncomplete(this ClaimsPrincipal user) =>
            user.HasClaim(c => c.Type == ClaimTypes.ProfileIncomplete);

        public static TimeZoneInfo GetTimeZoneInfo(this ClaimsPrincipal user)
        {
            var timeZoneId = user.GetTimeZoneId();
            return !string.IsNullOrEmpty(timeZoneId) ? TZConvert.GetTimeZoneInfo(timeZoneId) : null;
        }

        public static string GetDisplayName(this ClaimsPrincipal user, UserManager<ApplicationUser> userManager)
        {
            var username = userManager.GetUserName(user);
            var displayNameClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.DisplayName);
            if (displayNameClaim != null)
                username = user.Claims.Single(c => c.Type == ClaimTypes.DisplayName).Value;

            return username;
        }
    }
}
