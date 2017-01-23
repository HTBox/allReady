using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Services.Twitter;
using Microsoft.AspNetCore.Identity;

namespace AllReady.Providers.ExternalUserInformationProviders.Providers
{
    public class TwitterExternalUserInformationProvider : IProvideExternalUserInformation
    {
        private readonly ITwitterService _twitterService;

        public TwitterExternalUserInformationProvider(ITwitterService twitterService)
        {
            _twitterService = twitterService;
        }

        public async Task<ExternalUserInformation> GetExternalUserInformation(ExternalLoginInfo externalLoginInfo)
        {
            var externalUserInformation = new ExternalUserInformation();

            var userId = externalLoginInfo.Principal.FindFirstValue("urn:twitter:userid");
            var screenName = externalLoginInfo.Principal.FindFirstValue("urn:twitter:screenname");

            var twitterUser = await _twitterService.GetTwitterAccount(userId, screenName);

            if (twitterUser != null)
            {
                if (!string.IsNullOrEmpty(twitterUser.Name))
                {
                    var array = twitterUser.Name.Split(' ');
                    if (array.Length > 1)
                    {
                        externalUserInformation.FirstName = array[0];
                        externalUserInformation.LastName = array[1];
                    }
                }
            }

            return externalUserInformation;
        }
    }
}