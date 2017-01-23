using System;
using AllReady.Providers.ExternalUserInformationProviders.Providers;
using Autofac.Features.Indexed;

namespace AllReady.Providers.ExternalUserInformationProviders
{
    public class ExternalUserInformationProviderFactory : IExternalUserInformationProviderFactory
    {
        private readonly IIndex<string, IProvideExternalUserInformation> providers;

        public ExternalUserInformationProviderFactory(IIndex<string, IProvideExternalUserInformation> providers)
        {
            this.providers = providers;
        }

        public IProvideExternalUserInformation GetExternalUserInformationProvider(string loginProvider)
        {
            IProvideExternalUserInformation provider;
            if (!providers.TryGetValue(loginProvider, out provider))
            {
                throw new Exception($"could not find external user information provider for login provider: {loginProvider}");
            }
            return provider;
        }
    }
}