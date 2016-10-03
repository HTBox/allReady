using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class OrganizationIdByEventIdQuery : IAsyncRequest<int>
    {
        public int EventId { get; set; }
    }
}
