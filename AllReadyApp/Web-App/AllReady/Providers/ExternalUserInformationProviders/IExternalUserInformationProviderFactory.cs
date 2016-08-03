using AllReady.Providers.ExternalUserInformationProviders.Providers;

namespace AllReady.Providers.ExternalUserInformationProviders
{
    public interface IExternalUserInformationProviderFactory
    {
        IProvideExternalUserInformation GetExternalUserInformationProvider(string loginProvider);
    }
}