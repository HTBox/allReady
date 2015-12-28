using MediatR;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantDeleteCommand : IRequest
    {
        public int Id { get; set; }
    }
}