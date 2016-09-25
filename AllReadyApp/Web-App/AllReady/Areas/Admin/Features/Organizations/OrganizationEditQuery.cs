using AllReady.Areas.Admin.ViewModels.Organization;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationEditQuery : IAsyncRequest<OrganizationEditViewModel>
    {
        public int Id { get; set; }
    }
}
