using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AllReady.Providers.ExternalUserInformationProviders
{
    public class MicosoftExternalUserInformationProvider : IProvideExternalUserInformation
    {
        public ExternalUserInformation GetExternalUserInformationWith(ExternalLoginInfo externalLoginInfo)
        {
            var externalUserInformation = new ExternalUserInformation { Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) };

            var name = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(name))
            {
                return externalUserInformation;
            }

            var array = name.Split(' ');
            if (array.Length < 2)
            {
                return externalUserInformation;
            }

            externalUserInformation.FirstName = array[0];
            externalUserInformation.LastName = array[1];

            return externalUserInformation;
        }
    }
}