using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class EditOrganizationAsync : IAsyncRequest<int>
    {
        public OrganizationEditModel Organization { get; set; }
    }
}
