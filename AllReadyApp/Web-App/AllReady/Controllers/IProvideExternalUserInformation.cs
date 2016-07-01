using Microsoft.AspNet.Identity;
using System.Security.Claims;
using LinqToTwitter;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace AllReady.Controllers
{
    public interface IProvideExternalUserInformation
    {
        //TODO: get IConfiguration out of the method call and find a way to inject it as a dependency into the factory
        ExternalUserInformation GetExternalUserInformationWith(ExternalLoginInfo externalLoginInfo, IConfiguration configuration);
    }

    public class ExternalUserInformation
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

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

            //authTwitter.AuthorizeAsync();
            authTwitter.AuthorizeAsync().Wait();

            var twitterCtx = new TwitterContext(authTwitter);

            //var verifyResponse = await (from acct in twitterCtx.Account
            //    where (acct.Type == AccountType.VerifyCredentials) && (acct.IncludeEmail == true) //VERY important you explicitly keep the "== true" part of comparison here. ReSharper will prompt you to remove this, and if it does, the query will not work
            //    select acct).SingleOrDefaultAsync();

            var verifyResponse = (from acct in twitterCtx.Account
                where (acct.Type == AccountType.VerifyCredentials) && (acct.IncludeEmail == true) //VERY important you explicitly keep the "== true" part of comparison here. ReSharper will prompt you to remove this, and if it does, the query will not work
                select acct).SingleOrDefaultAsync().Result;

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

    public class MicosoftExternalUserInformationProvider : IProvideExternalUserInformation
    {
        public ExternalUserInformation GetExternalUserInformationWith(ExternalLoginInfo externalLoginInfo, IConfiguration configuration)
        {
            var externalUserInformation = new ExternalUserInformation { Email = externalLoginInfo.ExternalPrincipal.FindFirstValue(ClaimTypes.Email) };

            var name = externalLoginInfo.ExternalPrincipal.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(name))
                return externalUserInformation;

            var array = name.Split(' ');
            if (array.Length < 2)
                return externalUserInformation;

            externalUserInformation.FirstName = array[0];
            externalUserInformation.LastName = array[1];

            return externalUserInformation;
        }
    }

    public class FacebookExternalUserInformationProvider : IProvideExternalUserInformation
    {
        public ExternalUserInformation GetExternalUserInformationWith(ExternalLoginInfo externalLoginInfo, IConfiguration configuration)
        {
            var externalUserInformation = new ExternalUserInformation { Email = externalLoginInfo.ExternalPrincipal.FindFirstValue(ClaimTypes.Email) };

            var name = externalLoginInfo.ExternalPrincipal.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(name))
                return externalUserInformation;

            var array = name.Split(' ');
            if (array.Length < 2)
                return externalUserInformation;

            externalUserInformation.FirstName= array[0];
            externalUserInformation.LastName = array[1];

            return externalUserInformation;
        }
    }

    public class GoogleExternalUserInformationProvider : IProvideExternalUserInformation
    {
        public ExternalUserInformation GetExternalUserInformationWith(ExternalLoginInfo externalLoginInfo, IConfiguration configuration)
        {
            var externalUserInformation = new ExternalUserInformation
            {
                Email = externalLoginInfo.ExternalPrincipal.FindFirstValue(ClaimTypes.Email),
                FirstName = externalLoginInfo.ExternalPrincipal.FindFirstValue(ClaimTypes.GivenName),
                LastName = externalLoginInfo.ExternalPrincipal.FindFirstValue(ClaimTypes.Surname)
            };
            return externalUserInformation;
        }
    }
}
