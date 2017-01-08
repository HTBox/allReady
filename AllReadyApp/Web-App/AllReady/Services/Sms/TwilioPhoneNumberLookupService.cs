using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Rest.Lookups.V1;

namespace AllReady.Services.Sms
{
    /// <summary>
    /// A service which wraps the Twilio REST API library to lookup phones numbers
    /// </summary>
    public class TwilioPhoneNumberLookupService : IPhoneNumberLookupService
    {
        private readonly TwilioSettings _twilioSettings;
        private readonly TwilioRestClient _twilioClient;

        public TwilioPhoneNumberLookupService(IOptions<TwilioSettings> twilioSettings)
        {
            _twilioSettings = twilioSettings.Value;
            _twilioClient = new TwilioRestClient(_twilioSettings.Sid, _twilioSettings.Token);
        }

        /// <inheritdoc />
        public async Task<PhoneNumberLookupResult> LookupNumber(string phoneNumber, string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode))
            {
                countryCode = "US";
            }

            var twilioPhoneNumber = new Twilio.Types.PhoneNumber(phoneNumber);

            var twilioOptions = BuildTwilioRequestOptions(countryCode, twilioPhoneNumber);

            try
            {
                var lookupResult = await PhoneNumberResource.FetchAsync(twilioOptions, _twilioClient);

                return BuildPhoneNumberLookupResult(lookupResult);
            }
            catch
            {
                // If we reach here then it's likely that the phone number or country code were not valid
                // TODO - Logging
            }

            return PhoneNumberLookupResult.FailedLookup();
        }

        private static FetchPhoneNumberOptions BuildTwilioRequestOptions(string countryCode, Twilio.Types.PhoneNumber twilioPhoneNumber)
        {
            var options = new FetchPhoneNumberOptions(twilioPhoneNumber)
            {
                CountryCode = countryCode
            };

            options.Type.Add("carrier");

            return options;
        }

        private static PhoneNumberLookupResult BuildPhoneNumberLookupResult(PhoneNumberResource phoneNumberResource)
        {
            if (phoneNumberResource == null)
                return PhoneNumberLookupResult.FailedLookup();

            var phoneNumberType = PhoneNumberType.Unknown;

            if (phoneNumberResource.Carrier != null)
            {
                var type = phoneNumberResource.Carrier["type"];

                if (!string.IsNullOrEmpty(type))
                {
                    switch (type)
                    {
                        case "landline":
                            phoneNumberType = PhoneNumberType.Landline;
                            break;

                        case "mobile":
                            phoneNumberType = PhoneNumberType.Mobile;
                            break;

                        case "voip":
                            phoneNumberType = PhoneNumberType.Voip;
                            break;
                    }
                }
            }

            return new PhoneNumberLookupResult(phoneNumberResource.PhoneNumber.ToString(), phoneNumberType);
        }
    }
}
