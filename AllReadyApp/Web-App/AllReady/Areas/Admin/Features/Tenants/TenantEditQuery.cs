using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantEditQuery : IRequest<TenantEditModel>
    {
        public int Id { get; set; }
    }
}
