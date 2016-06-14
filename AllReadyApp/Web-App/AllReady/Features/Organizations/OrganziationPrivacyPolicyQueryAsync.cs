using AllReady.ViewModels;
using AllReady.ViewModels.Organization;
using MediatR;

namespace AllReady.Features.Organizations
{
    public class OrganziationPrivacyPolicyQueryAsync : IAsyncRequest<OrganizationPrivacyPolicyViewModel>
    {
        public int OrganizationId { get; set; }
    }
}