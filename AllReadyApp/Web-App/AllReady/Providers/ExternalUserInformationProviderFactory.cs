using AllReady.Providers.ExternalUserInformationProviders;
using Microsoft.Extensions.OptionsModel;

namespace AllReady.Providers
{
    public interface IExternalUserInformationProviderFactory
    {
        IProvideExternalUserInformation GetExternalUserInformationProviderFor(string loginProvider);
    }

    public class ExternalUserInformationProviderFactory : IExternalUserInformationProviderFactory
    {
        private readonly IOptions<TwitterAuthenticationSettings> twitterAuthenticationSettings;

        public ExternalUserInformationProviderFactory(IOptions<TwitterAuthenticationSettings> twitterAuthenticationSettings)
        {
            this.twitterAuthenticationSettings = twitterAuthenticationSettings;
        }

        public IProvideExternalUserInformation GetExternalUserInformationProviderFor(string loginProvider)
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
