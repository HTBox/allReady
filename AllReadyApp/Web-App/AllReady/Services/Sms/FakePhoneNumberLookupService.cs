using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AllReady.Services.Sms
{
    /// <summary>
    /// A fake phone number service that can be used during development. Use 00000000 to return a failed result. Use 99900000 a non mobile result.
    /// </summary>
    public class FakePhoneNumberLookupService : IPhoneNumberLookupService
    {
        public Task<PhoneNumberLookupResult> LookupNumber(string phoneNumber, string countryCode)
        {
            if (Regex.Matches(phoneNumber, @"[a-zA-Z]").Count > 0)
            {
                return Task.FromResult(PhoneNumberLookupResult.FailedLookup);
            }

            switch (phoneNumber)
            {
                case "00000000":
                    return Task.FromResult(PhoneNumberLookupResult.FailedLookup);

                case "99900000":
                    return Task.FromResult(new PhoneNumberLookupResult(phoneNumber, PhoneNumberType.Landline));
            }

            return Task.FromResult(new PhoneNumberLookupResult(phoneNumber, PhoneNumberType.Mobile));
        }
    }
}
