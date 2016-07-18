using Microsoft.AspNetCore.Identity;

namespace AllReady.Providers
{
    //TODO: make contract for GetExternalUserInformationWith to async to not force locking on async invocations inside individual providers
    public interface IProvideExternalUserInformation
    {
        ExternalUserInformation GetExternalUserInformationWith(ExternalLoginInfo externalLoginInfo);
    }

    public class ExternalUserInformation
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
