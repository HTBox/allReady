using AllReady.Services;
using AllReady.Services.Mapping.GeoCoding;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AllReady.Configuration;
using Xunit;

namespace AllReady.UnitTest.Services.Mapping.GeoCoding
{
    public class GoogleGeocodeServiceTests
    {
        private string ValidGoogleResponse = "{\"results\" : [{\"geometry\" : {\"location\" : {\"lat\" : 37.4224764,\"lng\" : -122.0842499}}}], \"status\" : \"OK\"}";
        private string NoResultsGoogleResponse = "{\"results\" : [], \"status\" : \"ZERO_RESULTS\"}";

        [Fact]
        public async Task GetCoordinatesFromAddress_CallsHttpClientGetAsync_Once()
        {
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest));

            var options = new Mock<IOptions<MappingSettings>>();
            options.Setup(x => x.Value).Returns(new MappingSettings { GoogleMapsApiKey = "123" });

            var sut = new GoogleGeocodeService(httpClient.Object, options.Object);

            await sut.GetCoordinatesFromAddress("address");

            httpClient.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetCoordinatesFromAddress_CallsHttpClientGetAsync_WithCorrectUrl()
        {
            var inputAddress = "1 some street, sometown, usa";

            var url = string.Empty;

            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)).Callback<string>(x => url = x);

            var options = new Mock<IOptions<MappingSettings>>();
            options.Setup(x => x.Value).Returns(new MappingSettings { GoogleMapsApiKey = "123" });

            var sut = new GoogleGeocodeService(httpClient.Object, options.Object);

            await sut.GetCoordinatesFromAddress(inputAddress);

            url.ShouldBe(string.Concat("https://maps.googleapis.com/maps/api/geocode/json?address=", Uri.EscapeUriString(inputAddress), "&key=", "123"));
        }

        [Fact]
        public async Task GetCoordinatesFromAddress_WithIndividualAddressParams_CallsHttpClientGetAsync_WithCorrectUrl()
        {
            var url = string.Empty;

            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)).Callback<string>(x => url = x);

            var options = new Mock<IOptions<MappingSettings>>();
            options.Setup(x => x.Value).Returns(new MappingSettings { GoogleMapsApiKey = "123" });

            var sut = new GoogleGeocodeService(httpClient.Object, options.Object);

            await sut.GetCoordinatesFromAddress("1 some street", "town", "state", "postcode", null);

            url.ShouldBe(string.Concat("https://maps.googleapis.com/maps/api/geocode/json?address=", "1%20some%20street,town,state,postcode", "&key=", "123"));
        }

        [Fact]
        public async Task GetCoordinatesFromAddress_ReturnsNull_WhenNotSuccessStatusCode()
        {
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest));

            var options = new Mock<IOptions<MappingSettings>>();
            options.Setup(x => x.Value).Returns(new MappingSettings { GoogleMapsApiKey = "123" });

            var sut = new GoogleGeocodeService(httpClient.Object, options.Object);

            var result = await sut.GetCoordinatesFromAddress("address");

            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetCoordinatesFromAddress_ReturnsNull_WhenExceptionThrown()
        {
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.GetAsync(It.IsAny<string>())).Throws<Exception>();

            var options = new Mock<IOptions<MappingSettings>>();
            options.Setup(x => x.Value).Returns(new MappingSettings { GoogleMapsApiKey = "123" });

            var sut = new GoogleGeocodeService(httpClient.Object, options.Object);

            var result = await sut.GetCoordinatesFromAddress("address");

            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetCoordinatesFromAddress_ReturnsNull_WhenNoResultsResponse()
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(NoResultsGoogleResponse);

            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(response);

            var options = new Mock<IOptions<MappingSettings>>();
            options.Setup(x => x.Value).Returns(new MappingSettings { GoogleMapsApiKey = "123" });

            var sut = new GoogleGeocodeService(httpClient.Object, options.Object);

            var result = await sut.GetCoordinatesFromAddress("address");

            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetCoordinatesFromAddress_ReturnsCorrectCoordinates_WhenValidGoogleResponse()
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(ValidGoogleResponse);

            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(response);

            var options = new Mock<IOptions<MappingSettings>>();
            options.Setup(x => x.Value).Returns(new MappingSettings { GoogleMapsApiKey = "123" });

            var sut = new GoogleGeocodeService(httpClient.Object, options.Object);

            var result = await sut.GetCoordinatesFromAddress("address");

            result.Latitude.ShouldBe(37.4224764);
            result.Longitude.ShouldBe(-122.0842499);
        }
    }
}
