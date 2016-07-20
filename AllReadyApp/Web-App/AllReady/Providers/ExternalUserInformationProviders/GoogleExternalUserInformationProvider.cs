using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AllReady.Providers.ExternalUserInformationProviders
{
    public class GoogleExternalUserInformationProvider : IProvideExternalUserInformation
    {
        public Task<ExternalUserInformation> GetExternalUserInformation(ExternalLoginInfo externalLoginInfo)
        {
            var externalUserInformation = new ExternalUserInformation
            {
                Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                FirstName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.GivenName),
                LastName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Surname)
            };

            return Task.FromResult(externalUserInformation);
        }
    }
}