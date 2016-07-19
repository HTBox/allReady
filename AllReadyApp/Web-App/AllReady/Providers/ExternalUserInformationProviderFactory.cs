using AllReady.Providers.ExternalUserInformationProviders;
using Microsoft.Extensions.Options;

namespace AllReady.Providers
{
    public interface IExternalUserInformationProviderFactory
    {
        IProvideExternalUserInformation GetExternalUserInformationProvider(string loginProvider);
    }

    public class ExternalUserInformationProviderFactory : IExternalUserInformationProviderFactory
    {
        private readonly IOptions<TwitterAuthenticationSettings> twitterAuthenticationSettings;

        public ExternalUserInformationProviderFactory(IOptions<TwitterAuthenticationSettings> twitterAuthenticationSettings)
        {
            this.twitterAuthenticationSettings = twitterAuthenticationSettings;
        }

        public IProvideExternalUserInformation GetExternalUserInformationProvider(string loginProvider)
        {
            switch (loginProvider)
            {
                case "Facebook":
                    return new FacebookExternalUserInformationProvider();
                case "Google":
                    return new GoogleExternalUserInformationProvider();
                case "Microsoft":
                    return new MicosoftExternalUserInformationProvider();
                case "Twitter":
                    return new TwitterExternalUserInformationProvider(twitterAuthenticationSettings);
            }
            return null;
        }
    }
}
