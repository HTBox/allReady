using System.Linq;
using LinqToTwitter;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Configuration;

namespace AllReady.Providers.ExternalUserInformationProviders
{
    public class TwitterExternalUserInformationProvider : IProvideExternalUserInformation
    {
        public ExternalUserInformation GetExternalUserInformationWith(ExternalLoginInfo externalLoginInfo, IConfiguration configuration)
        {
            var externalUserInformation = new ExternalUserInformation();

            var userId = externalLoginInfo.ExternalPrincipal.FindFirstValue("urn:twitter:userid");
            var screenName = externalLoginInfo.ExternalPrincipal.FindFirstValue("urn:twitter:screenname");

            var authTwitter = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = configuration["Authentication:Twitter:ConsumerKey"],
                    ConsumerSecret = configuration["Authentication:Twitter:ConsumerSecret"],
                    UserID = ulong.Parse(userId),
                    ScreenName = screenName,
                    OAuthToken = configuration["Authentication:Twitter:OAuthToken"],
                    OAuthTokenSecret = configuration["Authentication:Twitter:OAuthSecret"]
                }
            };

            //TODO: make contract async to not force locking on async invocations
            authTwitter.AuthorizeAsync().Wait();

            var twitterCtx = new TwitterContext(authTwitter);

            var verifyResponse = (from acct in twitterCtx.Account
                // ReSharper disable once RedundantBoolCompare
                where (acct.Type == AccountType.VerifyCredentials) && (acct.IncludeEmail == true) //VERY important you explicitly keep the "== true" part of comparison here. ReSharper will prompt you to remove this, and if it does, the query will not wor
                select acct).SingleOrDefaultAsync().Result; //TODO: make contract async to not force locking on async invocations

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