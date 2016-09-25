using AllReady.ViewModels.Organization;
using MediatR;

namespace AllReady.Features.Organizations
{
    public class OrganizationPrivacyPolicyQuery : IAsyncRequest<OrganizationPrivacyPolicyViewModel>
    {
        public int OrganizationId { get; set; }
    }
}