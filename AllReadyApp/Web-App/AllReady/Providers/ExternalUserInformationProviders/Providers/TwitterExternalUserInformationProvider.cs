using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Services.Twitter;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AllReady.Providers.ExternalUserInformationProviders.Providers
{
    public class TwitterExternalUserInformationProvider : IProvideExternalUserInformation
    {
        private readonly ITwitterService _twitterService;
        private readonly ILogger _logger;

        public TwitterExternalUserInformationProvider(ITwitterService twitterService, ILogger logger)
        {
            _twitterService = twitterService;
            _logger = logger;
        }

        public async Task<ExternalUserInformation> GetExternalUserInformation(ExternalLoginInfo externalLoginInfo)
        {
            var externalUserInformation = new ExternalUserInformation();

            var userId = externalLoginInfo.Principal.FindFirstValue("urn:twitter:userid");
            var screenName = externalLoginInfo.Principal.FindFirstValue("urn:twitter:screenname");
            var email = externalLoginInfo.Principal.FindFirstValue(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning($"Failed to retrieve user email address from Twitter, this could be either the setup of the app hasn't enabled email address or this user has a blank/unverified email in Twitter");
                //either the setup of the app hasn't enabled email address or this user has an unverified twitter email
                return externalUserInformation;
            }

            externalUserInformation.Email = email;

            //var twitterUser = await _twitterService.GetTwitterAccount(userId, screenName);

            //if (twitterUser != null)
            //{
            //    if (!string.IsNullOrEmpty(twitterUser.Name))
            //    {
            //        var array = twitterUser.Name.Split(' ');
            //        if (array.Length > 1)
            //        {
            //            externalUserInformation.FirstName = array[0];
            //            externalUserInformation.LastName = array[1];
            //        }
            //    }
            //}

            return externalUserInformation;
        }
    }
}