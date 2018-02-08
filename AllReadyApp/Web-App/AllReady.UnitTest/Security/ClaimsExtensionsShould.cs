using System;
using AllReady.Models;
using AllReady.Security;
using System.Security.Claims;
using Xunit;
using ClaimTypes = System.Security.Claims.ClaimTypes;
using Microsoft.AspNetCore.Identity;

namespace AllReady.UnitTest.Security
{
  public class ClaimsExtensionsShould : TestBase
  {
    UserManager<ApplicationUser> _userManager;

    public ClaimsExtensionsShould()
    {
      _userManager = (UserManager<ApplicationUser>)ServiceProvider.GetService(typeof(UserManager<ApplicationUser>));
    }

    [Fact]
    public void UserWithNoUserTypeClaimShouldNotMatchSiteAdmin()
    {
      var principal = new ClaimsPrincipal();
      Assert.False(principal.IsUserType(UserType.SiteAdmin));
    }

    [Fact]
    public void SiteAdminUserShouldMatchSiteAdmin()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.SiteAdmin)) }));
      Assert.True(principal.IsUserType(UserType.SiteAdmin));
    }

    [Fact]
    public void SiteAdminUserShouldNotMatchOrganizationAdmin()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.SiteAdmin)) }));

      Assert.False(principal.IsUserType(UserType.OrgAdmin));
    }

    [Fact]
    public void OrganizationAdminUserShouldMatchOrganizationAdmin()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin)) }));

      Assert.True(principal.IsUserType(UserType.OrgAdmin));
    }

    [Fact]
    public void OrganizationAdminUserShouldNotMatchSiteAdmin()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin)) }));

      Assert.False(principal.IsUserType(UserType.SiteAdmin));
    }

    [Fact]
    public void MultipleUserTypeClaimsShouldMatchAllTypes()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
      {
                new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.SiteAdmin)),
                new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.BasicUser))
            }));

      Assert.True(principal.IsUserType(UserType.SiteAdmin));
      Assert.True(principal.IsUserType(UserType.OrgAdmin));
      Assert.True(principal.IsUserType(UserType.BasicUser));
    }

    [Fact]
    public void GetOrganizationIdShouldReturnOrganizationId()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.Organization, "12") }));

      Assert.Equal(12, principal.GetOrganizationId());
    }

    [Fact]
    public void GetOrganizationIdShouldReturnNullWhenNotAnInteger()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.Organization, "ThisIsWRong") }));

      Assert.Null(principal.GetOrganizationId());
    }

    [Fact]
    public void IsOrganizationAdminReturnsFalseWhenUserIsNotOrganizationAdmin()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.Organization, "1") }));
      Assert.False(principal.IsOrganizationAdmin());
    }

    [Fact]
    public void IsOrganizationAdminReturnsFalseWhenUserDoesNotHaveOrganizationId()
    {
      var principal = new ClaimsPrincipal();
      Assert.False(principal.IsOrganizationAdmin());
    }

    [Fact]
    public void IsOrganizationAdminReturnsTrueWhenUserIsOrganizationAdmin()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin)) }));
      Assert.False(principal.IsOrganizationAdmin());
    }

    [Fact]
    public void SiteAdminShouldBeAdminOfAnyOrganizationId()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.SiteAdmin)) }));

      Assert.True(principal.IsOrganizationAdmin(12));
    }

    [Fact]
    public void WhenOrganizationIdIsNotSetOrganizationAdminShouldNotBeAdminOfOrganization()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin)) }));

      Assert.False(principal.IsOrganizationAdmin(1));
    }

    [Fact]
    public void WhenOrganizationIdIsSetOrganizationAdminShouldBeAdminOfOrganization()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
      {
                new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, "2")
            }));

      Assert.True(principal.IsOrganizationAdmin(2));
    }

    [Fact]
    public void WhenOrganizationIdIsSetOrganizationAdminShouldNotBeAdminOfAnotherOrganization()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
      {
                new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.OrgAdmin)),
                new Claim(AllReady.Security.ClaimTypes.Organization, "2")
            }));

      Assert.False(principal.IsOrganizationAdmin(1));
    }

    [Fact]
    public void WhenOrganizationIdIsSetNonOrganizationAdminShouldNotBeAdminOfOrganization()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.Organization, "2") }));

      Assert.False(principal.IsOrganizationAdmin(2));
    }

    [Fact]
    public void IsUserProfileIncompleteReturnsTrueWhenUsersProfileIsIncomplete()
    {
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.ProfileIncomplete, "true") }));
      Assert.True(principal.IsUserProfileIncomplete());
    }

    [Fact]
    public void IsUserProfileIncompleteReturnsFalseWhenUsersProfileIsComplete()
    {
      var principal = new ClaimsPrincipal();
      Assert.False(principal.IsUserProfileIncomplete());
    }

    [Fact]
    public void GetTimeZoneIdReturnsCorrectTimeZoneIdWhenUserHasTimeZoneIdClaim()
    {
      const string timeZoneId = "Eastern Standard Time";
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.TimeZoneId, timeZoneId) }));
      var result = principal.GetTimeZoneId();
      Assert.Equal(timeZoneId, result);
    }

    [Fact]
    public void GetTimeZoneIdReturnsDefaultUtcWhenUserDoesNotHaveTimeZoneIdClaim()
    {
        var principal = new ClaimsPrincipal();
        var result = principal.GetTimeZoneId();
        Assert.Equal("UTC", result);
    }

    [Fact]
    public void GetTimeZoneInfoReturnsCorrectTimeZoneInfoForUsersTimeZoneId()
    {
      const string timeZoneId = "Eastern Standard Time";
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(AllReady.Security.ClaimTypes.TimeZoneId, timeZoneId) }));
      var result = principal.GetTimeZoneInfo();
      //b/c TimeZoneInfo.FindSystemTimeZoneById gets it's information from the registry, we'll check for non-null here
      //NB: on Linux it actually gets it from /usr/share/zoneinfo
      Assert.NotNull(result);
    }

    [Fact]
    public void GetTimeZoneInfoReturnsDefaultUtcUsersTimeZoneIdIsNotSet()
    {
        var principal = new ClaimsPrincipal();
        var result = principal.GetTimeZoneInfo();
        Assert.Equal("UTC", result.Id);
    }

    [Fact]
    public void GetDisplayNameReturnsCorrectDisplayNameForUsersWithDisplayNameClaim()
    {
      const string displayName = "FirstName LastName";
      var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
      {
                new Claim(ClaimTypes.Name, "FirstNameLastName"),
                new Claim(AllReady.Security.ClaimTypes.DisplayName, displayName)
            }));

      var result = principal.GetDisplayName(_userManager);
      Assert.Equal(displayName, result);
    }

    [Fact]
    public void GetDisplayNameReturnsNullForUsersWithoutDisplayNameClaim()
    {
      var principal = new ClaimsPrincipal();
      var result = principal.GetDisplayName(_userManager);
      Assert.Null(result);
    }
  }
}
