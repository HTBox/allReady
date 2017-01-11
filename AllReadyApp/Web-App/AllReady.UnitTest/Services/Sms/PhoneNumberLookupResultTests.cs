using AllReady.Services.Sms;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Services.Sms
{
    public class PhoneNumberLookupResultTests
    {
        [Fact]
        public void FailedLookup_ReturnsExpectedPhoneNumberLookupResult()
        {
            var sut = PhoneNumberLookupResult.FailedLookup;

            sut.LookupFailed.ShouldBeTrue();
            sut.PhoneNumberE164.ShouldBeNull();
            sut.Type.ShouldBe(PhoneNumberType.Unknown);
        }

        [Fact]
        public void Ctor_SetsCorrectProperties()
        {
            var phoneNumber = "+447777123456";

            var sut = new PhoneNumberLookupResult(phoneNumber, PhoneNumberType.Mobile);

            sut.LookupFailed.ShouldBeFalse();
            sut.PhoneNumberE164.ShouldBe(phoneNumber);
            sut.Type.ShouldBe(PhoneNumberType.Mobile);
        }
    }
}
