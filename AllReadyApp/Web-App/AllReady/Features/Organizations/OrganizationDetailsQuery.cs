using AllReady.ViewModels.Organization;
using MediatR;

namespace AllReady.Features.Organizations
{
    public class OrganizationDetailsQuery : IAsyncRequest<OrganizationViewModel>
    {
        public int Id { get; set; }
    }
}