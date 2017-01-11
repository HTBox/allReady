using System.Threading.Tasks;

namespace AllReady.Services.Sms
{
    /// <summary>
    /// Defines a service that can handle looking up a phone number
    /// </summary>
    public interface IPhoneNumberLookupService
    {
        /// <summary>
        /// Looks up a phone number
        /// </summary>
        Task<PhoneNumberLookupResult> LookupNumber(string phoneNumber, string countryCode);
    }
}
