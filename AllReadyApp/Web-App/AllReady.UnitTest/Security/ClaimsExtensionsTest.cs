using AllReady.Models;
using AllReady.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Security
{
    public class ClaimsExtensionsTest
    {
        [Fact]
        public void UserWithNoUserTypeClaimShouldNotMatch()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal();
            Assert.False(principal.IsUserType(UserType.SiteAdmin));
        }

        [Fact]
        public void SiteAdminUserShouldMatchSiteAdmin()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[] 
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, "SiteAdmin")
                        }
                    ));
           
            Assert.True(principal.IsUserType(UserType.SiteAdmin));
        }

        [Fact]
        public void SiteAdminUserShouldNotMatchTenantAdmin()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, "SiteAdmin")
                        }
                    ));

            Assert.False(principal.IsUserType(UserType.TenantAdmin));
        }


        [Fact]
        public void TenantAdminUserShouldMatchTenantAdmin()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, "TenantAdmin")
                        }
                    ));

            Assert.True(principal.IsUserType(UserType.TenantAdmin));
        }

        [Fact]
        public void TenantAdminUserShouldNotMatchSiteAdmin()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, "TenantAdmin")
                        }
                    ));

            Assert.False(principal.IsUserType(UserType.SiteAdmin));
        }


        [Fact]
        public void MultipleUserTypeClaimsShouldMatchAllTypes()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, "SiteAdmin"),
                            new Claim(AllReady.Security.ClaimTypes.UserType, "TenantAdmin"),
                        }
                    ));

            Assert.True(principal.IsUserType(UserType.SiteAdmin));
            Assert.True(principal.IsUserType(UserType.TenantAdmin));
        }

        [Fact]
        public void GetTenantIdShouldReturnTenantId()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.Tenant, "12")
                        }
                    ));

            Assert.Equal(12, principal.GetTenantId());
        }

        [Fact]
        public void GetTenantIdShouldReturnNullWhenNotAnInteger()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.Tenant, "ThisIsWRong")
                        }
                    ));

            Assert.Null(principal.GetTenantId());
        }


    }
}
