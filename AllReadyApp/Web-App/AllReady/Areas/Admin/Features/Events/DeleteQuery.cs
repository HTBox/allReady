using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class DeleteQuery : IAsyncRequest<DeleteViewModel>
    {
        public int EventId { get; set; }
    }
}
