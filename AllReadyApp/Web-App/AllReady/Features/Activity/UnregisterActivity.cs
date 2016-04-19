using MediatR;

namespace AllReady.Features.Activity
{
    public class UnregisterActivity : IAsyncRequest
    {
        public int ActivitySignupId { get; set; }
        public string UserId { get; set; }
    }
}
