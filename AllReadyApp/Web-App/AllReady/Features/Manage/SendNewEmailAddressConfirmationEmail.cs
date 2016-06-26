using MediatR;

namespace AllReady.Features.Manage
{
    public class SendNewEmailAddressConfirmationEmail : IAsyncRequest
    {
        public string Email { get; set; }
        public string  CallbackUrl { get; set; }
    }
}
