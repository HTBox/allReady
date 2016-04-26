using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class ManagingOrganizationIdByEventIdQuery : IRequest<int>
    {
        public int EventId { get; set; }
    }
}
