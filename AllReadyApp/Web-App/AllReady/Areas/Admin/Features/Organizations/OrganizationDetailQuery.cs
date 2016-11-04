using AllReady.Areas.Admin.ViewModels.Organization;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationDetailQuery : IAsyncRequest<OrganizationDetailViewModel>
    {
        public int Id { get; set; }
    }
}
