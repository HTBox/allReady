using MediatR;

namespace AllReady.Features.Activity
{
    public class ActivityByActivityIdQuery : IRequest<Models.Activity>
    {
        public int ActivityId { get; set; }
    }
}
