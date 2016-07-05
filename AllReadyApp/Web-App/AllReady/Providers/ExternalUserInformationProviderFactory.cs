using AllReady.Providers.ExternalUserInformationProviders;
using Microsoft.Extensions.Configuration;

namespace AllReady.Providers
{
    public interface IExternalUserInformationProviderFactory
    {
        IProvideExternalUserInformation GetExternalUserInformationProviderFor(string loginProvider);
    }

    public class ExternalUserInformationProviderFactory : IExternalUserInformationProviderFactory
    {
        private readonly IConfiguration configuration;

        public ExternalUserInformationProviderFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
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
                    return new TwitterExternalUserInformationProvider(configuration);
            }
            return null;
        }
    }
}
