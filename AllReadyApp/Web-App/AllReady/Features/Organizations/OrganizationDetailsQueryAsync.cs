using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Organizations
{
    public class OrganizationDetailsQueryAsync : IAsyncRequest<OrganizationViewModel>
    {
        public int Id { get; set; }
    }
}