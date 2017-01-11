using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Rest.Lookups.V1;

namespace AllReady.Services.Sms
{
    /// <summary>
    /// Wraps the Twilio library code to enable easier testing
    /// </summary>
    public interface ITwilioWrapper
    {
        Task<PhoneNumberResource> FetchPhoneNumberResource(FetchPhoneNumberOptions fetchOptions, ITwilioRestClient twilioClient);
    }
}
