using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class OrganizationIdByEventIdQueryAsync : IAsyncRequest<int>
    {
        public int EventId { get; set; }
    }
}
