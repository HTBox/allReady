using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AllReady.Providers.ExternalUserInformationProviders.Providers
{
    public class MicrosoftAndFacebookExternalUserInformationProvider : IProvideExternalUserInformation
    {
        public Task<ExternalUserInformation> GetExternalUserInformation(ExternalLoginInfo externalLoginInfo)
        {
            var externalUserInformation = new ExternalUserInformation { Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) };

            var name = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(name))
            {
                return Task.FromResult(externalUserInformation);
            }

            var array = name.Split(' ');
            if (array.Length < 2)
            {
                return Task.FromResult(externalUserInformation);
            }

            externalUserInformation.FirstName = array[0];
            externalUserInformation.LastName = array[1];

            return Task.FromResult(externalUserInformation);
        }
    }
}
