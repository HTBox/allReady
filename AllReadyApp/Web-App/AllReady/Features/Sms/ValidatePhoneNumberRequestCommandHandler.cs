using AllReady.Services.Sms;
using MediatR;
using System.Threading.Tasks;

namespace AllReady.Features.Sms
{
    public class ValidatePhoneNumberRequestCommandHandler : IAsyncRequestHandler<ValidatePhoneNumberRequestCommand, ValidatePhoneNumberResult>
    {
        private readonly IPhoneNumberLookupService _phoneNumberLookupService;

        public ValidatePhoneNumberRequestCommandHandler(IPhoneNumberLookupService phoneNumberLookupService)
        {
            _phoneNumberLookupService = phoneNumberLookupService;
        }

        public async Task<ValidatePhoneNumberResult> Handle(ValidatePhoneNumberRequestCommand request)
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
