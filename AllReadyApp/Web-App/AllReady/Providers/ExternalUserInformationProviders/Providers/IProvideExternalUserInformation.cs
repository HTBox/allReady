using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AllReady.Providers.ExternalUserInformationProviders.Providers
{
    public interface IProvideExternalUserInformation
    {
        Task<ExternalUserInformation> GetExternalUserInformation(ExternalLoginInfo externalLoginInfo);
    }

    public class ExternalUserInformation
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
