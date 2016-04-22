using MediatR;

namespace AllReady.Features.Manage
{
    public class SendAccountSecurityTokenSms : IAsyncRequest
    {
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
    }
}
