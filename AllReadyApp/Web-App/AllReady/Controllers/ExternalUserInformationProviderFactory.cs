namespace AllReady.Controllers
{
    public interface IExternalUserInformationProviderFactory
    {
        IProvideExternalUserInformation GetExternalUserInformationProviderFor(string loginProvider);
    }

    public class ExternalUserInformationProviderFactory : IExternalUserInformationProviderFactory
    {
        //make string loginProvider an enum LoginProvier
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
