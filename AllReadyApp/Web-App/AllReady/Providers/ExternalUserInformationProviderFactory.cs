using AllReady.Providers.ExternalUserInformationProviders;

namespace AllReady.Providers
{
    public interface IExternalUserInformationProviderFactory
    {
        IProvideExternalUserInformation GetExternalUserInformationProviderFor(string loginProvider);
    }

    public class ExternalUserInformationProviderFactory : IExternalUserInformationProviderFactory
    {
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
                    return new TwitterExternalUserInformationProvider();
            }
            return null;
        }
    }
}
