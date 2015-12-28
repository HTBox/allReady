using MediatR;

namespace AllReady.Areas.Admin.Features.Tenants
{
    public class OrganizationNameQueryAsync : IAsyncRequest<string>
    {
        public int Id { get; set; }
    }
}