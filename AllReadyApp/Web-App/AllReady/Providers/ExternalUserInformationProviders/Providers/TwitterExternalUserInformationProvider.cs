using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AllReady.Providers.ExternalUserInformationProviders.Providers
{
    public class TwitterExternalUserInformationProvider : IProvideExternalUserInformation
    {
        private readonly IOptions<TwitterAuthenticationSettings> twitterAuthenticationSettings;

        public TwitterExternalUserInformationProvider(IOptions<TwitterAuthenticationSettings> twitterAuthenticationSettings)
        {
            this.twitterAuthenticationSettings = twitterAuthenticationSettings;
        }

        public async Task<ExternalUserInformation> GetExternalUserInformation(ExternalLoginInfo externalLoginInfo)
        {
            var externalUserInformation = new ExternalUserInformation();

            var userId = externalLoginInfo.Principal.FindFirstValue("urn:twitter:userid");
            var screenName = externalLoginInfo.Principal.FindFirstValue("urn:twitter:screenname");

            var authTwitter = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = twitterAuthenticationSettings.Value.ConsumerKey,
                    ConsumerSecret = twitterAuthenticationSettings.Value.ConsumerSecret,
                    OAuthToken = twitterAuthenticationSettings.Value.OAuthToken,
                    OAuthTokenSecret = twitterAuthenticationSettings.Value.OAuthSecret,

                    UserID = ulong.Parse(userId),
                    ScreenName = screenName
                }
            };

            await authTwitter.AuthorizeAsync();

            var twitterCtx = new TwitterContext(authTwitter);

            //VERY important you explicitly keep the "== true" part of comparison. ReSharper will prompt you to remove this, and if it does, the query will not work
            var verifyResponse = await (from acct in twitterCtx.Account where (acct.Type == AccountType.VerifyCredentials) && (acct.IncludeEmail == true)
                select acct).SingleOrDefaultAsync();

            if (verifyResponse != null && verifyResponse.User != null)
            {
                var twitterUser = verifyResponse.User;
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