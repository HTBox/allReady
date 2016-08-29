﻿using System;
using AllReady.Models;

namespace AllReady.UnitTest.Extensions
{
    public static class ApplicationUserTestHelpers
    {
        public static void MakeOrgAdmin(this ApplicationUser user)
        {
            user.Claims.Add(new Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>
            {
                ClaimType = AllReady.Security.ClaimTypes.UserType,
                ClaimValue = Enum.GetName(typeof(UserType), UserType.OrgAdmin)
            });
        }

        public static void MakeSiteAdmin(this ApplicationUser user)
        {
            user.Claims.Add(new Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>
            {
                ClaimType = AllReady.Security.ClaimTypes.UserType,
                ClaimValue = Enum.GetName(typeof(UserType), UserType.SiteAdmin)
            });
        }
    }
}
