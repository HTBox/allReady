namespace AllReady.Services.Sms
{
    /// <summary>
    /// The result of a phone number lookup
    /// </summary>
    public class PhoneNumberLookupResult
    {
        private PhoneNumberLookupResult()
        {
            LookupFailed = true;
        }

        /// <summary>
        /// Initialise a new <see cref="PhoneNumberLookupResult"/> 
        /// </summary>
        public PhoneNumberLookupResult(string e164PhoneNumber, PhoneNumberType type = PhoneNumberType.Unknown)
        {
            PhoneNumberE164 = e164PhoneNumber;
            Type = type;
        }

        public static PhoneNumberLookupResult FailedLookup
        {
            get
            {
                return new PhoneNumberLookupResult();
            }
        }

        /// <summary>
        /// The phone number in E.164 format. E.164 defines a general format for international telephone numbers.
        /// </summary>
        public string PhoneNumberE164 { get; private set; }

        /// <summary>
        /// The type for the phone number - Landline, mobile or VOIP
        /// </summary>
        public PhoneNumberType Type { get; private set; }

        /// <summary>
        /// Indicates that the lookup failed to complete
        /// </summary>
        public bool LookupFailed { get; private set; }
    }
}