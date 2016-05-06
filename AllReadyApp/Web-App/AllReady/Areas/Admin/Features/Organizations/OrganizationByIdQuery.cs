using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationByIdQuery : IRequest<Organization>
    {
        public int OrganizationId { get; set; }
    }
}
