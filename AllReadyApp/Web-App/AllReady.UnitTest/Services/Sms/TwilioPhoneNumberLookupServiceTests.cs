using AllReady.Services.Sms;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Threading.Tasks;
using AllReady.Configuration;
using Twilio.Clients;
using Twilio.Rest.Lookups.V1;
using Xunit;

namespace AllReady.UnitTest.Services.Sms
{
    public class TwilioPhoneNumberLookupServiceTests
    {
        private string TwilioResponseJson = "{\"carrier\": {\"error_code\": null,\"mobile_country_code\": \"310\",\"mobile_network_code\": \"456\",\"name\": \"verizon\",\"type\": \"mobile\"},\"country_code\": \"US\",\"national_format\": \"(510) 867-5309\",\"phone_number\": \"+15108675309\",\"add_ons\": {\"status\": \"successful\",\"message\": null,\"code\": null,\"results\": {}},\"url\": \"https://lookups.twilio.com/v1/PhoneNumbers/phone_number\"}";
        private string TwilioResponseJsonNoCarrier = "{\"country_code\": \"US\",\"national_format\": \"(510) 867-5309\",\"phone_number\": \"+15108675309\",\"add_ons\": {\"status\": \"successful\",\"message\": null,\"code\": null,\"results\": {}},\"url\": \"https://lookups.twilio.com/v1/PhoneNumbers/phone_number\"}";
        private string TwilioResponseJsonLandline = "{\"carrier\": {\"error_code\": null,\"mobile_country_code\": \"310\",\"mobile_network_code\": \"456\",\"name\": \"verizon\",\"type\": \"landline\"},\"country_code\": \"US\",\"national_format\": \"(510) 867-5309\",\"phone_number\": \"+15108675309\",\"add_ons\": {\"status\": \"successful\",\"message\": null,\"code\": null,\"results\": {}},\"url\": \"https://lookups.twilio.com/v1/PhoneNumbers/phone_number\"}";
        private string TwilioResponseJsonVoip = "{\"carrier\": {\"error_code\": null,\"mobile_country_code\": \"310\",\"mobile_network_code\": \"456\",\"name\": \"verizon\",\"type\": \"voip\"},\"country_code\": \"US\",\"national_format\": \"(510) 867-5309\",\"phone_number\": \"+15108675309\",\"add_ons\": {\"status\": \"successful\",\"message\": null,\"code\": null,\"results\": {}},\"url\": \"https://lookups.twilio.com/v1/PhoneNumbers/phone_number\"}";

        [Fact]
        public async Task LookupNumber_CallsTwilioWrapper()
        {
            var options = new Mock<IOptions<TwilioSettings>>();
            options.Setup(x => x.Value).Returns(new TwilioSettings { Sid = "123",Token = "ABC", PhoneNo = "1234567890" });

            var twilioWrapper = new Mock<ITwilioWrapper>();

            var sut = new TwilioPhoneNumberLookupService(options.Object, twilioWrapper.Object);

            await sut.LookupNumber("123456789", "us");

            twilioWrapper.Verify(x => x.FetchPhoneNumberResource(It.IsAny<FetchPhoneNumberOptions>(), It.IsAny<ITwilioRestClient>()), Times.Once);
        }

        [Fact]
        public async Task LookupNumber_CallsTwilioWrapper_WithCorrectPhoneNumber()
        {
            FetchPhoneNumberOptions fetchOptions = null;

            var options = new Mock<IOptions<TwilioSettings>>();
            options.Setup(x => x.Value).Returns(new TwilioSettings { Sid = "123", Token = "ABC", PhoneNo = "1234567890" });

            var twilioWrapper = new Mock<ITwilioWrapper>();
            twilioWrapper.Setup(x => x.FetchPhoneNumberResource(It.IsAny<FetchPhoneNumberOptions>(), It.IsAny<ITwilioRestClient>()))
                .Callback<FetchPhoneNumberOptions, ITwilioRestClient>((x, y) => fetchOptions = x);

            var sut = new TwilioPhoneNumberLookupService(options.Object, twilioWrapper.Object);

            await sut.LookupNumber("123456789", "us");

            fetchOptions.PathPhoneNumber.ToString().ShouldBe(new Twilio.Types.PhoneNumber("123456789").ToString());
        }

        [Fact]
        public async Task LookupNumber_CallsTwilioWrapper_WithCorrectType()
        {
            FetchPhoneNumberOptions fetchOptions = null;

            var options = new Mock<IOptions<TwilioSettings>>();
            options.Setup(x => x.Value).Returns(new TwilioSettings { Sid = "123", Token = "ABC", PhoneNo = "1234567890" });

            var twilioWrapper = new Mock<ITwilioWrapper>();
            twilioWrapper.Setup(x => x.FetchPhoneNumberResource(It.IsAny<FetchPhoneNumberOptions>(), It.IsAny<ITwilioRestClient>()))
                .Callback<FetchPhoneNumberOptions, ITwilioRestClient>((x, y) => fetchOptions = x);

            var sut = new TwilioPhoneNumberLookupService(options.Object, twilioWrapper.Object);

            await sut.LookupNumber("123456789", "us");

            fetchOptions.Type[0].ShouldBe("carrier");
        }

        [Fact]
        public async Task LookupNumber_CallsTwilioWrapper_WithCorrectCountryCode()
        {
            FetchPhoneNumberOptions fetchOptions = null;

            var options = new Mock<IOptions<TwilioSettings>>();
            options.Setup(x => x.Value).Returns(new TwilioSettings { Sid = "123", Token = "ABC", PhoneNo = "1234567890" });

            var twilioWrapper = new Mock<ITwilioWrapper>();
            twilioWrapper.Setup(x => x.FetchPhoneNumberResource(It.IsAny<FetchPhoneNumberOptions>(), It.IsAny<ITwilioRestClient>()))
                .Callback<FetchPhoneNumberOptions, ITwilioRestClient>((x, y) => fetchOptions = x);

            var sut = new TwilioPhoneNumberLookupService(options.Object, twilioWrapper.Object);

            await sut.LookupNumber("123456789", "us");

            fetchOptions.CountryCode.ShouldBe("us");
        }

        [Fact]
        public async Task LookupNumber_ReturnsFailedLokupResult_IfTheTwilioCallFails()
        {
            var options = new Mock<IOptions<TwilioSettings>>();
            options.Setup(x => x.Value).Returns(new TwilioSettings { Sid = "123", Token = "ABC", PhoneNo = "1234567890" });

            var twilioWrapper = new Mock<ITwilioWrapper>();
            twilioWrapper.Setup(x => x.FetchPhoneNumberResource(It.IsAny<FetchPhoneNumberOptions>(), It.IsAny<ITwilioRestClient>()))
                .Throws<Exception>();

            var sut = new TwilioPhoneNumberLookupService(options.Object, twilioWrapper.Object);

            var result = await sut.LookupNumber("123456789", "us");

            result.LookupFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task LookupNumber_ReturnsFailedLokupResult_IfTheTwilioCallReturnsNullPhoneNumberResource()
        {
            var options = new Mock<IOptions<TwilioSettings>>();
            options.Setup(x => x.Value).Returns(new TwilioSettings { Sid = "123", Token = "ABC", PhoneNo = "1234567890" });

            var twilioWrapper = new Mock<ITwilioWrapper>();
            twilioWrapper.Setup(x => x.FetchPhoneNumberResource(It.IsAny<FetchPhoneNumberOptions>(), It.IsAny<ITwilioRestClient>()))
                .ReturnsAsync((PhoneNumberResource)null);

            var sut = new TwilioPhoneNumberLookupService(options.Object, twilioWrapper.Object);

            var result = await sut.LookupNumber("123456789", "us");

            result.LookupFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task LookupNumber_ReturnsCorrectPhoneNumberInResult()
        {
            var options = new Mock<IOptions<TwilioSettings>>();
            options.Setup(x => x.Value).Returns(new TwilioSettings { Sid = "123", Token = "ABC", PhoneNo = "1234567890" });

            var twilioWrapper = new Mock<ITwilioWrapper>();
            twilioWrapper.Setup(x => x.FetchPhoneNumberResource(It.IsAny<FetchPhoneNumberOptions>(), It.IsAny<ITwilioRestClient>()))
                .ReturnsAsync(PhoneNumberResource.FromJson(TwilioResponseJson));

            var sut = new TwilioPhoneNumberLookupService(options.Object, twilioWrapper.Object);

            var result = await sut.LookupNumber("123456789", "us");

            result.PhoneNumberE164.ToString().ShouldBe(new Twilio.Types.PhoneNumber("+15108675309").ToString());
        }

        [Fact]
        public async Task LookupNumber_ReturnsUnknownType_WhenNoCarrierInfo()
        {
            var options = new Mock<IOptions<TwilioSettings>>();
            options.Setup(x => x.Value).Returns(new TwilioSettings { Sid = "123", Token = "ABC", PhoneNo = "1234567890" });

            var twilioWrapper = new Mock<ITwilioWrapper>();
            twilioWrapper.Setup(x => x.FetchPhoneNumberResource(It.IsAny<FetchPhoneNumberOptions>(), It.IsAny<ITwilioRestClient>()))
                .ReturnsAsync(PhoneNumberResource.FromJson(TwilioResponseJsonNoCarrier));

            var sut = new TwilioPhoneNumberLookupService(options.Object, twilioWrapper.Object);

            var result = await sut.LookupNumber("123456789", "us");

            result.Type.ShouldBe(PhoneNumberType.Unknown);
        }

        [Fact]
        public async Task LookupNumber_ReturnsMobileType_WhenResponseIncludesMobileCarrier()
        {
            var options = new Mock<IOptions<TwilioSettings>>();
            options.Setup(x => x.Value).Returns(new TwilioSettings { Sid = "123", Token = "ABC", PhoneNo = "1234567890" });

            var twilioWrapper = new Mock<ITwilioWrapper>();
            twilioWrapper.Setup(x => x.FetchPhoneNumberResource(It.IsAny<FetchPhoneNumberOptions>(), It.IsAny<ITwilioRestClient>()))
                .ReturnsAsync(PhoneNumberResource.FromJson(TwilioResponseJson));

            var sut = new TwilioPhoneNumberLookupService(options.Object, twilioWrapper.Object);

            var result = await sut.LookupNumber("123456789", "us");

            result.Type.ShouldBe(PhoneNumberType.Mobile);
        }

        [Fact]
        public async Task LookupNumber_ReturnsLandlineType_WhenResponseIncludesLandlineCarrier()
        {
            var options = new Mock<IOptions<TwilioSettings>>();
            options.Setup(x => x.Value).Returns(new TwilioSettings { Sid = "123", Token = "ABC", PhoneNo = "1234567890" });

            var twilioWrapper = new Mock<ITwilioWrapper>();
            twilioWrapper.Setup(x => x.FetchPhoneNumberResource(It.IsAny<FetchPhoneNumberOptions>(), It.IsAny<ITwilioRestClient>()))
                .ReturnsAsync(PhoneNumberResource.FromJson(TwilioResponseJsonLandline));

            var sut = new TwilioPhoneNumberLookupService(options.Object, twilioWrapper.Object);

            var result = await sut.LookupNumber("123456789", "us");

            result.Type.ShouldBe(PhoneNumberType.Landline);
        }

        [Fact]
        public async Task LookupNumber_ReturnsVoipType_WhenResponseIncludesVoipCarrier()
        {
            var options = new Mock<IOptions<TwilioSettings>>();
            options.Setup(x => x.Value).Returns(new TwilioSettings { Sid = "123", Token = "ABC", PhoneNo = "1234567890" });

            var twilioWrapper = new Mock<ITwilioWrapper>();
            twilioWrapper.Setup(x => x.FetchPhoneNumberResource(It.IsAny<FetchPhoneNumberOptions>(), It.IsAny<ITwilioRestClient>()))
                .ReturnsAsync(PhoneNumberResource.FromJson(TwilioResponseJsonVoip));

            var sut = new TwilioPhoneNumberLookupService(options.Object, twilioWrapper.Object);

            var result = await sut.LookupNumber("123456789", "us");

            result.Type.ShouldBe(PhoneNumberType.Voip);
        }
    }
}
