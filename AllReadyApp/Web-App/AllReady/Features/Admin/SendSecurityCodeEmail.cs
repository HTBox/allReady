using MediatR;

namespace AllReady.Features.Admin
{
    public class SendSecurityCodeEmail : IAsyncRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
