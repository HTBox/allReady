using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Rest.Lookups.V1;

namespace AllReady.Services.Sms
{
    public class TwilioWrapper : ITwilioWrapper
    {
        public async Task<PhoneNumberResource> FetchPhoneNumberResource(FetchPhoneNumberOptions fetchOptions, ITwilioRestClient twilioClient)
        {
            return await PhoneNumberResource.FetchAsync(fetchOptions, twilioClient);
        }
    }
}
