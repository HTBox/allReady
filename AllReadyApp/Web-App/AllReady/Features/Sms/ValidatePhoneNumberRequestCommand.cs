using MediatR;

namespace AllReady.Features.Sms
{
    public class ValidatePhoneNumberRequestCommand : IAsyncRequest<ValidatePhoneNumberResult>
    {
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }
        public bool ValidateType { get; set; }
    }
}
