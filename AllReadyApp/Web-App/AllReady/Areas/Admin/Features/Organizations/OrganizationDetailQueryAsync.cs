using AllReady.Areas.Admin.ViewModels.Organization;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationDetailQueryAsync : IAsyncRequest<OrganizationDetailViewModel>
    {
        public int Id { get; set; }
    }
}
