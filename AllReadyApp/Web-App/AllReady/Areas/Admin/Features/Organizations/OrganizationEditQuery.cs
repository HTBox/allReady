using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationEditQuery : IAsyncRequest<OrganizationEditModel>
    {
        public int Id { get; set; }
    }
}
