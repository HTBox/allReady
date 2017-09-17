using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Services.Twitter;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AllReady.Providers.ExternalUserInformationProviders.Providers
{
    public class TwitterExternalUserInformationProvider : IProvideExternalUserInformation
    {
        private readonly ILogger _logger;

        public TwitterExternalUserInformationProvider(ILogger<TwitterExternalUserInformationProvider> logger)
        {
            _logger = logger;
        }

        public Task<ExternalUserInformation> GetExternalUserInformation(ExternalLoginInfo externalLoginInfo)
        {
            var externalUserInformation = new ExternalUserInformation();

            var screenName = externalLoginInfo.Principal.FindFirstValue("urn:twitter:screenname");
            var email = externalLoginInfo.Principal.FindFirstValue(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogError($"Failed to retrieve user email address for user {screenName} from Twitter, this could be due to either the setup of the app (ensure the app requests email address permissions) or this user has a blank/unverified email in Twitter");
                return Task.FromResult(externalUserInformation);
            }

            externalUserInformation.Email = email;
            return Task.FromResult(externalUserInformation);
        }
    }
}
