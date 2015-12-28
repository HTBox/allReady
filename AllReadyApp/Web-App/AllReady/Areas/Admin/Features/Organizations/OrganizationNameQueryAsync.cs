using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationNameQueryAsync : IAsyncRequest<string>
    {
        public int Id { get; set; }
    }
}