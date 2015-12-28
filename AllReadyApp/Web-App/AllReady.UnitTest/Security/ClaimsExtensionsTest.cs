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
        public void SiteAdminUserShouldNotMatchOrganizationAdmin()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, "SiteAdmin")
                        }
                    ));

            Assert.False(principal.IsUserType(UserType.OrgAdmin));
        }


        [Fact]
        public void OrganizationAdminUserShouldMatchOrganizationAdmin()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, "OrgAdmin")
                        }
                    ));

            Assert.True(principal.IsUserType(UserType.OrgAdmin));
        }

        [Fact]
        public void OrganizationAdminUserShouldNotMatchSiteAdmin()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, "OrgAdmin")
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
                            new Claim(AllReady.Security.ClaimTypes.UserType, "OrgAdmin"),
                        }
                    ));

            Assert.True(principal.IsUserType(UserType.SiteAdmin));
            Assert.True(principal.IsUserType(UserType.OrgAdmin));
        }

        [Fact]
        public void GetOrganizationIdShouldReturnOrganizationId()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.Organization, "12")
                        }
                    ));

            Assert.Equal(12, principal.GetOrganizationId());
        }

        [Fact]
        public void GetOrganizationIdShouldReturnNullWhenNotAnInteger()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.Organization, "ThisIsWRong")
                        }
                    ));

            Assert.Null(principal.GetOrganizationId());
        }


        [Fact]
        public void SiteAdminShouldBeAdminOfAnyOrganizationId()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, "SiteAdmin")
                        }
                    ));

            Assert.True(principal.IsOrganizationAdmin(12));
        }

        [Fact]
        public void WhenOrganizationIdIsNotSetOrganizationAdminShouldNotBeAdminOfOrganization()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, "OrgAdmin")
                        }
                    ));

            Assert.False(principal.IsOrganizationAdmin(1));
        }

        [Fact]
        public void WhenOrganizationIdIsSetOrganizationAdminShouldBeAdminOfOrganization()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, "OrgAdmin"),
                            new Claim(AllReady.Security.ClaimTypes.Organization, "2")
                        }
                    ));

            Assert.True(principal.IsOrganizationAdmin(2));
        }

        [Fact]
        public void WhenOrganizationIdIsSetOrganizationAdminShouldNotBeAdminOfAnotherOrganization()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.UserType, "OrgAdmin"),
                            new Claim(AllReady.Security.ClaimTypes.Organization, "2")
                        }
                    ));

            Assert.False(principal.IsOrganizationAdmin(1));
        }

        [Fact]
        public void WhenOrganizationIdIsSetNonOrganizationAdminShouldNotBeAdminOfOrganization()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new[]
                        {
                            new Claim(AllReady.Security.ClaimTypes.Organization, "2")
                        }
                    ));

            Assert.False(principal.IsOrganizationAdmin(2));
        }

    }
}
