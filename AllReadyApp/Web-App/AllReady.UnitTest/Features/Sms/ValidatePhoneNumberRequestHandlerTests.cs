using AllReady.Features.Sms;
using AllReady.Services.Sms;
using Moq;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Sms
{
    public class ValidatePhoneNumberRequestHandlerTests
    {
        [Fact]
        public async Task ValidatePhoneNumberRequestHandler_ShouldCallPhoneNumberLookupService_Once()
        {
            var lookupService = new Mock<IPhoneNumberLookupService>();
            lookupService.Setup(x => x.LookupNumber(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(PhoneNumberLookupResult.FailedLookup).Verifiable();

            var sut = new ValidatePhoneNumberRequestHandler(lookupService.Object);

            await sut.Handle(new ValidatePhoneNumberRequest { PhoneNumber = "0123456789", CountryCode = "us", ValidateType = true });

            lookupService.Verify(x => x.LookupNumber(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ValidatePhoneNumberRequestHandler_ShouldCallPhoneNumberLookupService_WithCorrectValues()
        {
            string phoneNumber = null;
            string countryCode = null;

            var lookupService = new Mock<IPhoneNumberLookupService>();
            lookupService.Setup(x => x.LookupNumber(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(PhoneNumberLookupResult.FailedLookup)
                .Callback<string, string>((x, y) => { phoneNumber = x; countryCode = y; })
                .Verifiable();

            var sut = new ValidatePhoneNumberRequestHandler(lookupService.Object);

            await sut.Handle(new ValidatePhoneNumberRequest { PhoneNumber = "0123456789", CountryCode = "us", ValidateType = true });

            phoneNumber.ShouldBe("0123456789");
            countryCode.ShouldBe("us");
        }

        [Fact]
        public async Task ValidatePhoneNumberRequestHandler_ShouldReturnInvalidPhoneNumberResult_WhenPhoneNumberLookupServicesReturnsFailedLookup()
        {
            var lookupService = new Mock<IPhoneNumberLookupService>();
            lookupService.Setup(x => x.LookupNumber(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(PhoneNumberLookupResult.FailedLookup);

            var sut = new ValidatePhoneNumberRequestHandler(lookupService.Object);

            var result = await sut.Handle(new ValidatePhoneNumberRequest { PhoneNumber = "0123456789", CountryCode = "us", ValidateType = true });

            result.IsValid.ShouldBeFalse();
        }

        [Fact]
        public async Task ValidatePhoneNumberRequestHandler_ShouldReturnInvalidPhoneNumberResult_WhenPhoneNumberLookupServicesReturnsEmptyPhoneNumber()
        {
            var lookupService = new Mock<IPhoneNumberLookupService>();
            lookupService.Setup(x => x.LookupNumber(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new PhoneNumberLookupResult(""));

            var sut = new ValidatePhoneNumberRequestHandler(lookupService.Object);

            var result = await sut.Handle(new ValidatePhoneNumberRequest { PhoneNumber = "0123456789", CountryCode = "us", ValidateType = true });

            result.IsValid.ShouldBeFalse();
        }

        [Fact]
        public async Task ValidatePhoneNumberRequestHandler_ShouldReturnInvalidPhoneNumberResult_WhenPhoneNumberLookupServicesReturnsNonMobile_AndValidTypeIsSetToTrue()
        {
            var lookupService = new Mock<IPhoneNumberLookupService>();
            lookupService.Setup(x => x.LookupNumber(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new PhoneNumberLookupResult("00000", PhoneNumberType.Unknown));

            var sut = new ValidatePhoneNumberRequestHandler(lookupService.Object);

            var result = await sut.Handle(new ValidatePhoneNumberRequest { PhoneNumber = "0123456789", CountryCode = "us", ValidateType = true });

            result.IsValid.ShouldBeFalse();
        }

        [Fact]
        public async Task ValidatePhoneNumberRequestHandler_ShouldReturnValidPhoneNumberResult_WhenPhoneNumberLookupServicesReturnsNonMobile_AndValidTypeIsSetToFalse()
        {
            var lookupService = new Mock<IPhoneNumberLookupService>();
            lookupService.Setup(x => x.LookupNumber(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new PhoneNumberLookupResult("00000", PhoneNumberType.Unknown));

            var sut = new ValidatePhoneNumberRequestHandler(lookupService.Object);

            var result = await sut.Handle(new ValidatePhoneNumberRequest { PhoneNumber = "0123456789", CountryCode = "us", ValidateType = false });

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public async Task ValidatePhoneNumberRequestHandler_ShouldReturnValidPhoneNumberResult_WhenPhoneNumberLookupServicesReturnsMobile_AndValidTypeIsSetToTrue()
        {
            var lookupService = new Mock<IPhoneNumberLookupService>();
            lookupService.Setup(x => x.LookupNumber(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new PhoneNumberLookupResult("00000", PhoneNumberType.Mobile));

            var sut = new ValidatePhoneNumberRequestHandler(lookupService.Object);

            var result = await sut.Handle(new ValidatePhoneNumberRequest { PhoneNumber = "0123456789", CountryCode = "us", ValidateType = true });

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public async Task ValidatePhoneNumberRequestHandler_ShouldReturnCorrectPhoneNumberForAValidResult()
        {
            var lookupService = new Mock<IPhoneNumberLookupService>();
            lookupService.Setup(x => x.LookupNumber(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new PhoneNumberLookupResult("00000", PhoneNumberType.Mobile));

            var sut = new ValidatePhoneNumberRequestHandler(lookupService.Object);

            var result = await sut.Handle(new ValidatePhoneNumberRequest { PhoneNumber = "0123456789", CountryCode = "us", ValidateType = true });

            result.PhoneNumberE164.ShouldBe("00000");
        }
    }
}
