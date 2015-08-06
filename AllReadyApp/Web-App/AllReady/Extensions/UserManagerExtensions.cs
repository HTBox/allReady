using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using System.Security.Claims;

namespace AllReady.Extensions
{
    public static class UserManagerExtensions
    {
        public async static Task<ApplicationUser> GetCurrentUser(this UserManager<ApplicationUser> _userManager, HttpContext httpContext)
        {
            return await _userManager.FindByIdAsync(httpContext.User.GetUserId());
        }
        public async static Task<IList<Claim>> GetClaimsForCurrentUser(this UserManager<ApplicationUser> userManager, HttpContext httpContext)
        {
            ApplicationUser currentUser = await GetCurrentUser(userManager, httpContext);
            return await userManager.GetClaimsAsync(currentUser);
        }

        public async static Task<bool> IsSiteAdmin(this UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            return await CheckUserType(userManager, user, "SiteAdmin");
        }

        public async static Task<bool> IsTenantAdmin(this UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            // SiteAdmin has uber permissions, so checks for whether it is TenantAdmin should also succeed.
            return await CheckUserType(userManager, user, new [] { "TenantAdmin", "SiteAdmin" } );
        }

        private async static Task<bool> CheckUserType(UserManager<ApplicationUser> userManager, ApplicationUser user, params string [] types)
        {
            var claims = await userManager.GetClaimsAsync(user);
            if (claims.Count > 0)
            {
                var claimValue = claims.FirstOrDefault(c => c.Type.Equals("UserType")).Value;
                foreach (string type in types)
                {
                    if (claimValue.Equals(type))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
