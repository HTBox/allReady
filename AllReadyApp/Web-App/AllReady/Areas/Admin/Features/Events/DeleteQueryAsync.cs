using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class DeleteQueryAsync : IAsyncRequest<DeleteViewModel>
    {
        public int EventId { get; set; }
    }
}
