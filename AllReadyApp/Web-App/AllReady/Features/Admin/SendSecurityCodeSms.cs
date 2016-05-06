using MediatR;

namespace AllReady.Features.Admin
{
    public class SendSecurityCodeSms : IAsyncRequest
    {
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
    }
}
