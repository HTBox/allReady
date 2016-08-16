using AllReady.Areas.Admin.ViewModels.Organization;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationEditQueryAsync : IAsyncRequest<OrganizationEditViewModel>
    {
        public int Id { get; set; }
    }
}
