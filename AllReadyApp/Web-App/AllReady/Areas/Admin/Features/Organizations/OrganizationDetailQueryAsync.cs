using AllReady.Areas.Admin.ViewModels.Organization;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationDetailQueryAsync : IAsyncRequest<OrganizationDetailModel>
    {
        public int Id { get; set; }
    }
}
