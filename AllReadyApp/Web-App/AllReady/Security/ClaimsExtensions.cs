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

        public static bool IsOrganizationAdmin(this ClaimsPrincipal user)
        {
            int? userOrganizationId = user.GetOrganizationId();
            return userOrganizationId.HasValue && user.IsOrganizationAdmin(userOrganizationId.Value);
        }

        public static bool IsOrganizationAdmin(this ClaimsPrincipal user, int organizationId)
        {
            int? userOrganizationId = user.GetOrganizationId();
            return user.IsUserType(UserType.SiteAdmin) ||
                  (user.IsUserType(UserType.OrgAdmin) &&
                   userOrganizationId.HasValue && userOrganizationId.Value == organizationId);
        }

        public static int? GetOrganizationId(this ClaimsPrincipal user)
        {
            int? result = null;
            var organizationIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Organization);
            if (organizationIdClaim != null)
            {
                int organizationId;
                if (int.TryParse(organizationIdClaim.Value, out organizationId))
                {
                    result = organizationId;
                }
            }

            return result;
        }

        public static string GetTimeZoneId(this ClaimsPrincipal user)
        {
            string result = null;
            var timeZoneIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.TimeZoneId);
            if (timeZoneIdClaim != null)
            {
                result = timeZoneIdClaim.Value;
            }
            return result;
        }

        public static bool IsUserProfileIncomplete(this ClaimsPrincipal user)
        {
            return user.HasClaim(c => c.Type == ClaimTypes.ProfileIncompleted);                        
        }


        public static TimeZoneInfo GetTimeZoneInfo(this ClaimsPrincipal user)
        {
            var timeZoneId = user.GetTimeZoneId();
            if (!string.IsNullOrEmpty(timeZoneId))
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            return null;
        }

        public static string GetDisplayName(this ClaimsPrincipal user)
        {
            var username = user.GetUserName();
            if (user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.DisplayName) != null)
            {
                username = user.Claims.Single(c => c.Type == ClaimTypes.DisplayName).Value;
            }
            return username;

        }

        public static int? GetOrganizationId(this ApplicationUser user)
        {
            int? result = null;
            var organizationIdClaim = user.Claims.FirstOrDefault(c => c.ClaimType == ClaimTypes.Organization);
            if (organizationIdClaim != null)
            {
                int organizationId;
                if (int.TryParse(organizationIdClaim.ClaimValue, out organizationId))
                {
                    result = organizationId;
                }
            }

            return result;
        }


    }
}
