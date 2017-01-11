using AllReady.Services.Sms;
using MediatR;
using System.Threading.Tasks;

namespace AllReady.Features.Sms
{
    public class ValidatePhoneNumberRequestHandler : IAsyncRequestHandler<ValidatePhoneNumberRequest, ValidatePhoneNumberResult>
    {
        private readonly IPhoneNumberLookupService _phoneNumberLookupService;

        public ValidatePhoneNumberRequestHandler(IPhoneNumberLookupService phoneNumberLookupService)
        {
            _phoneNumberLookupService = phoneNumberLookupService;
        }

        public async Task<ValidatePhoneNumberResult> Handle(ValidatePhoneNumberRequest request)
        {
            var lookupResult = await _phoneNumberLookupService.LookupNumber(request.PhoneNumber, request.CountryCode);

            if (lookupResult.LookupFailed || (request.ValidateType && lookupResult.Type != PhoneNumberType.Mobile) || string.IsNullOrEmpty(request.PhoneNumber))
            {
                return new ValidatePhoneNumberResult { IsValid = false };
            }

            return new ValidatePhoneNumberResult { IsValid = true, PhoneNumberE164 = lookupResult.PhoneNumberE164 };
        }
    }
}
