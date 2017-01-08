namespace AllReady.Features.Sms
{
    public class ValidatePhoneNumberResult
    {
        public bool IsValid { get; set; }
        public string PhoneNumberE164 { get; set; }
    }
}
