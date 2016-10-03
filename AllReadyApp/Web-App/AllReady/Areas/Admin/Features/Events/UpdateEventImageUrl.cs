using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class UpdateEventImageUrl : IAsyncRequest
    {
        public int EventId { get; set; }
        public string ImageUrl { get; set; }
    }
}
