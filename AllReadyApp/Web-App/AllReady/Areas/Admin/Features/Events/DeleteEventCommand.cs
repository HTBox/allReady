using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class DeleteEventCommand : IAsyncRequest
    {
        public int EventId {get; set;}
    }
}
