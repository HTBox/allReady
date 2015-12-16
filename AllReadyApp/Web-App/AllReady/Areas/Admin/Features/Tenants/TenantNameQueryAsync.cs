using MediatR;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantNameQueryAsync : IAsyncRequest<string>
    {
        public int Id { get; set; }
    }
}