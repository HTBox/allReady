using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Configuration;

namespace AllReady.Providers
{
    //TODO: get IConfiguration out of the method call and find a way to inject it as a dependency into the Twitter provider in the ExternalUserInformationProviderFactory
    //TODO: make contract for GetExternalUserInformationWith to async to not force locking on async invocations inside individual providers
    public interface IProvideExternalUserInformation
    {
        ExternalUserInformation GetExternalUserInformationWith(ExternalLoginInfo externalLoginInfo, IConfiguration configuration);
    }

    public class ExternalUserInformation
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
