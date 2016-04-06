using MediatR;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class DeleteActivityCommand : IAsyncRequest
    {
        public int ActivityId {get; set;}
    }
}
