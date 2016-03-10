using MediatR;

namespace AllReady.Features.Activity
{
    public class DeleteActivityAndTaskSignupsCommandAsync : IAsyncRequest
    {
        public int ActivitySignupId { get; set; }
    }
}
