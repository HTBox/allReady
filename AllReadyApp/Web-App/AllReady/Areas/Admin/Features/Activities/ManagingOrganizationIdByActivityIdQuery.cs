using MediatR;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class ManagingOrganizationIdByActivityIdQuery : IRequest<int>
    {
        public int ActivityId { get; set; }
    }
}
