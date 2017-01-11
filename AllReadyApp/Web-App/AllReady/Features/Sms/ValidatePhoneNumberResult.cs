namespace AllReady.Features.Sms
{
    /// <summary>
    /// Represents the result of validating a phone number with an external service
    /// </summary>
    public class ValidatePhoneNumberResult
    {
        /// <summary>
        /// Indicates that the number was considered valid
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// The phone number in E.164 format. E.164 defines a general format for international telephone numbers.
        /// </summary>
        public string PhoneNumberE164 { get; set; }
    }
}
