using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class ManagingOrganizationIdByEventIdQuery : IAsyncRequest<int>
    {
        public int EventId { get; set; }
    }
}
