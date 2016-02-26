using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationEditCommand : IRequest<int>
    {
        public OrganizationEditModel Organization { get; set; }
    }
}
