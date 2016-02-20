using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Organizations
{
    public class OrganziationPrivacyPolicyQueryAsync : IAsyncRequest<OrganizationPrivacyPolicyViewModel>
    {
        public int Id { get; set; }
    }
}