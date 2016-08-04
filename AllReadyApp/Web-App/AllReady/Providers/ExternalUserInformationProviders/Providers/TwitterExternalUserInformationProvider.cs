using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AllReady.Providers.ExternalUserInformationProviders.Providers
{
    public class TwitterExternalUserInformationProvider : IProvideExternalUserInformation
    {
        private readonly ITwitterRepository twitterRepository;

        public TwitterExternalUserInformationProvider(ITwitterRepository twitterRepository)
        {
            this.twitterRepository = twitterRepository;
        }

        public async Task<ExternalUserInformation> GetExternalUserInformation(ExternalLoginInfo externalLoginInfo)
        {
            var externalUserInformation = new ExternalUserInformation();

            var userId = externalLoginInfo.Principal.FindFirstValue("urn:twitter:userid");
            var screenName = externalLoginInfo.Principal.FindFirstValue("urn:twitter:screenname");

            var account = await twitterRepository.GetTwitterAccount(userId, screenName);
            if (account != null && account.User != null)
            {
                var twitterUser = account.User;
                if (twitterUser != null)
                {
                    externalUserInformation.Email = twitterUser.Email;

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
            }

            return externalUserInformation;
        }
    }
}