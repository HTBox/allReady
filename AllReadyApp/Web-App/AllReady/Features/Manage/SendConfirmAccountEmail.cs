using MediatR;

namespace AllReady.Features.Manage
{
    public class SendConfirmAccountEmail : IAsyncRequest
    {
        public string Email { get; set; }
        public string CallbackUrl { get; set; }
    }
}
