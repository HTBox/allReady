using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Providers.ExternalUserInformationProviders.Providers;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace AllReady.UnitTest.Providers.ExternalUserInformationProviders.Providers
{
    public class MicrosoftAndFacebookExternalUserInformationProviderShould
    {
        [Fact]
        public async Task ReturnCorrectExternalLoginInfoWhenNameClaimIsNull()
        {
            const string email = "email";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email)
            }));

            var sut = new MicrosoftAndFacebookExternalUserInformationProvider();
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(claimsPrincipal, null, null, null));

            Assert.Equal(result.Email, email);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task ReturnCorrectExternalLoginInfoWhenNameClaimIsEmpty()
        {
            const string email = "email";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, string.Empty)
            }));

            var sut = new MicrosoftAndFacebookExternalUserInformationProvider();
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(claimsPrincipal, null, null, null));

            Assert.Equal(result.Email, email);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task ReturnCorrectExternalLoginInfoWhenNameClaimValueDoesNotContainASpace()
        {
            const string email = "email";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, "firstlast")
            }));

            var sut = new MicrosoftAndFacebookExternalUserInformationProvider();
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(claimsPrincipal, null, null, null));

            Assert.Equal(result.Email, email);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task ReturnCorrectExternalLoginInfoWhenNameClaimContainsASpace()
        {
            const string email = "email";
            const string name = "first last";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, name)
            }));

            var splitName = name.Split(' ');

            var sut = new MicrosoftAndFacebookExternalUserInformationProvider();
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(claimsPrincipal, null, null, null));

            Assert.Equal(result.Email, email);
            Assert.Equal(result.FirstName, splitName[0]);
            Assert.Equal(result.LastName, splitName[1]);
        }
    }
}
