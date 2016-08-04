using AllReady.Providers.ExternalUserInformationProviders.Providers;
using Autofac;

namespace AllReady.Providers.ExternalUserInformationProviders
{
    public class ExternalUserInformationProviderFactory : IExternalUserInformationProviderFactory
    {
        private readonly IComponentContext autofacContainer;

        public ExternalUserInformationProviderFactory(IComponentContext autofacContainer)
        {
            this.autofacContainer = autofacContainer;
        }

        public IProvideExternalUserInformation GetExternalUserInformationProvider(string loginProvider)
        {
            switch (loginProvider)
            {
                case "Facebook":
                    return autofacContainer.Resolve<MicrosoftAndFacebookExternalUserInformationProvider>();
                case "Microsoft":
                    return autofacContainer.Resolve<MicrosoftAndFacebookExternalUserInformationProvider>(); ;
                case "Google":
                    return autofacContainer.Resolve<GoogleExternalUserInformationProvider>();
                case "Twitter":
                    return autofacContainer.Resolve<TwitterExternalUserInformationProvider>();
            }
            return null;
        }
    }
}
