using AllReady.Areas.Admin.ViewModels.Organization;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class EditOrganization : IAsyncRequest<int>
    {
        public OrganizationEditViewModel Organization { get; set; }
    }
}
