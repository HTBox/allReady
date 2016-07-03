using MediatR;

namespace AllReady.Features.Event
{
    public class UnregisterEvent : IAsyncRequest
    {
        public int EventSignupId { get; set; }
        public string UserId { get; set; }
    }
}
