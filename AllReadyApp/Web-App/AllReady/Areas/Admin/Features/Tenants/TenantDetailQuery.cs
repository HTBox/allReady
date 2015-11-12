using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantDetailQuery : IRequest<TenantDetailModel>
    {
        public int Id { get; set; }
    }
}
