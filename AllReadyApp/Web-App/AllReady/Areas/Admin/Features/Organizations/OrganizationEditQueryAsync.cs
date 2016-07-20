using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationEditQueryAsync : IAsyncRequest<OrganizationEditModel>
    {
        public int Id { get; set; }
    }
}
