using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Providers.ExternalUserInformationProviders.Providers;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace AllReady.UnitTest.Providers.ExternalUserInformationProviders.Providers
{
    public class GoogleExternalUserInformationProviderShould
    {
        [Fact]
        public async Task ReturnCorrectExternalLoginInfoWhenClaimsArePopulated()
        {
            const string email = "email";
            const string givenName = "GivenName";
            const string surName = "Surname";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.GivenName, givenName),
                new Claim(ClaimTypes.Surname, surName)
            }));

            var sut = new GoogleExternalUserInformationProvider();
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(claimsPrincipal, null, null, null));

            Assert.Equal(result.Email, email);
            Assert.Equal(result.FirstName, givenName);
            Assert.Equal(result.LastName, surName);
        }

        [Fact]
        public async Task ReturnCorrectExternalLoginInfoWhenClaimsAreNotPopulated()
        {
            var sut = new GoogleExternalUserInformationProvider();
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Null(result.Email);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }
    }
}