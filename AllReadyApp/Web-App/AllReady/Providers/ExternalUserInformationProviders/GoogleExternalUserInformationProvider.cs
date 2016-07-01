using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Configuration;

namespace AllReady.Providers.ExternalUserInformationProviders
{
    public class GoogleExternalUserInformationProvider : IProvideExternalUserInformation
    {
        public ExternalUserInformation GetExternalUserInformationWith(ExternalLoginInfo externalLoginInfo, IConfiguration configuration)
        {
            var externalUserInformation = new ExternalUserInformation
            {
                Email = externalLoginInfo.ExternalPrincipal.FindFirstValue(ClaimTypes.Email),
                FirstName = externalLoginInfo.ExternalPrincipal.FindFirstValue(ClaimTypes.GivenName),
                LastName = externalLoginInfo.ExternalPrincipal.FindFirstValue(ClaimTypes.Surname)
            };
            return externalUserInformation;
        }
    }
}