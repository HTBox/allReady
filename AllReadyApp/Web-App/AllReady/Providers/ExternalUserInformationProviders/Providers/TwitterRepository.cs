﻿using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.Extensions.Options;

namespace AllReady.Providers.ExternalUserInformationProviders.Providers
{
    public class TwitterRepository : ITwitterRepository
    {
        private readonly IOptions<TwitterAuthenticationSettings> twitterAuthenticationSettings;

        public TwitterRepository(IOptions<TwitterAuthenticationSettings> twitterAuthenticationSettings)
        {
            this.twitterAuthenticationSettings = twitterAuthenticationSettings;
        }

        public async Task<Account> GetTwitterAccount(string userId, string screenName)
        {
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
            var account = await (from acct in twitterCtx.Account
                                 where (acct.Type == AccountType.VerifyCredentials) && (acct.IncludeEmail == true)
                                 select acct).SingleOrDefaultAsync();

            return account;
        }
    }
}
