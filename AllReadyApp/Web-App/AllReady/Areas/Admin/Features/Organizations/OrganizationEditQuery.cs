using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationEditQuery : IRequest<OrganizationEditModel>
    {
        public int Id { get; set; }
    }
}
