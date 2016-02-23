using MediatR;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class DeleteActivityCommand : IRequest
    {
        public int ActivityId {get; set;}
    }
}
