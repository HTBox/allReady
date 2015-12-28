using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationDetailQuery : IRequest<OrganizationDetailModel >
    {
        public int Id { get; set; }
    }
}
