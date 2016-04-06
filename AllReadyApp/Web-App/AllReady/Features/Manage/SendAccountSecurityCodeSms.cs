using MediatR;

namespace AllReady.Features.Manage
{
    public class SendAccountSecurityCodeSms : IAsyncRequest
    {
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
    }
}
