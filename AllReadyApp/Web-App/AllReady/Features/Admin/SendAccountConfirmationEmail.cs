using MediatR;

namespace AllReady.Features.Admin
{
    public class SendAccountConfirmationEmail : IAsyncRequest
    {
        public string Email { get; set; }
        public string CallbackUrl { get; set; }
    }
}
