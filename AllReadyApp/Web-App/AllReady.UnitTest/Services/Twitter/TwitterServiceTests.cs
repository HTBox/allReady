using AllReady.Services;
using AllReady.Services.Twitter;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Services.Twitter
{
    public class TwitterServiceTests
    {
        private const string AccessToken = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA%2FAAAAAAAAAAAAAAAAAAAA%3DAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
        private const string TwitterName = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA%2FAAAAAAAAAAAAAAAAAAAA%3DAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

        private const string NonBearerTokenTypeResponse = "{\"token_type\":\"non-bearer\",\"access_token\":\"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA%2FAAAAAAAAAAAAAAAAAAAA%3DAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA\"}";
        private const string BearerTokenTypeResponse = "{\"token_type\":\"bearer\",\"access_token\":\"" + AccessToken + "\"}";

        private const string ShowUserResponse = "{\"name\":\""+ TwitterName +"\"}";

        [Fact]
        public async Task GetTwitterAccount_ReturnsNull_IfUserIdAndScreenNameAreNull()
        {
            var options = new Mock<IOptions<TwitterAuthenticationSettings>>();
            options.Setup(x => x.Value).Returns(new TwitterAuthenticationSettings {  ConsumerKey = "KEY", ConsumerSecret = "SECRET" });

            var sut = new TwitterService(options.Object, Mock.Of<IHttpClient>());

            var result = await sut.GetTwitterAccount(null, null);

            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTwitterAccount_RequestBearerToken_ShouldCallHttpClient_Once()
        {
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)).Verifiable();

            var options = new Mock<IOptions<TwitterAuthenticationSettings>>();
            options.Setup(x => x.Value).Returns(new TwitterAuthenticationSettings { ConsumerKey = "KEY", ConsumerSecret = "SECRET" });

            var sut = new TwitterService(options.Object, httpClient.Object);

            var result = await sut.GetTwitterAccount(null, "username");

            httpClient.Verify(x => x.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Once);
        }
        
        [Fact]
        public async Task GetTwitterAccount_RequestBearerToken_ShouldCallHttpClient_WithCorrectAuthentication()
        {
            HttpRequestMessage request = null;

            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)).Callback<HttpRequestMessage>(x => request = x);

            var options = new Mock<IOptions<TwitterAuthenticationSettings>>();
            options.Setup(x => x.Value).Returns(new TwitterAuthenticationSettings { ConsumerKey = "KEY", ConsumerSecret = "SECRET" });

            var sut = new TwitterService(options.Object, httpClient.Object);

            var result = await sut.GetTwitterAccount(null, "username");

            request.Headers.Authorization.Scheme.ShouldBe("Basic");
            request.Headers.Authorization.Parameter.ShouldBe("S0VZOlNFQ1JFVA");
        }

        [Fact]
        public async Task GetTwitterAccount_ReturnsNull_IfHttpClientBearerCallThrowsAnError()
        {
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).Throws<Exception>();

            var options = new Mock<IOptions<TwitterAuthenticationSettings>>();
            options.Setup(x => x.Value).Returns(new TwitterAuthenticationSettings { ConsumerKey = "KEY", ConsumerSecret = "SECRET" });

            var sut = new TwitterService(options.Object, httpClient.Object);

            var result = await sut.GetTwitterAccount(null, "username");

            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTwitterAccount_ReturnsNull_IfHttpClientBearerCallReturnsNonSuccessCode()
        {
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest));

            var options = new Mock<IOptions<TwitterAuthenticationSettings>>();
            options.Setup(x => x.Value).Returns(new TwitterAuthenticationSettings { ConsumerKey = "KEY", ConsumerSecret = "SECRET" });

            var sut = new TwitterService(options.Object, httpClient.Object);

            var result = await sut.GetTwitterAccount(null, "username");

            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTwitterAccount_ReturnsNull_IfBearerRequestResponse_DoesNotContainBearerTokenType()
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(NonBearerTokenTypeResponse);

            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(response);

            var options = new Mock<IOptions<TwitterAuthenticationSettings>>();
            options.Setup(x => x.Value).Returns(new TwitterAuthenticationSettings { ConsumerKey = "KEY", ConsumerSecret = "SECRET" });

            var sut = new TwitterService(options.Object, httpClient.Object);

            var result = await sut.GetTwitterAccount(null, "username");

            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTwitterAccount_RequestBearerToken_ShouldCallHttpClient_TwiceWhenFirstRequestReturnsValidBearerToken()
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(BearerTokenTypeResponse);

            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupSequence(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(response)
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK));

            var options = new Mock<IOptions<TwitterAuthenticationSettings>>();
            options.Setup(x => x.Value).Returns(new TwitterAuthenticationSettings { ConsumerKey = "KEY", ConsumerSecret = "SECRET" });

            var sut = new TwitterService(options.Object, httpClient.Object);

            var result = await sut.GetTwitterAccount(null, "username");

            httpClient.Verify(x => x.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetTwitterAccount_RequestBearerToken_ShouldCallHttpClientTheSecondTime_WithCorrectBearerToken()
        {
            HttpRequestMessage request = null;

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(BearerTokenTypeResponse);

            var calls = 0;
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(response)
                .Callback<HttpRequestMessage>(x =>
                {
                    calls++;
                    if (calls == 2)
                    {
                        request = x;
                    }
                });

            var options = new Mock<IOptions<TwitterAuthenticationSettings>>();
            options.Setup(x => x.Value).Returns(new TwitterAuthenticationSettings { ConsumerKey = "KEY", ConsumerSecret = "SECRET" });

            var sut = new TwitterService(options.Object, httpClient.Object);

            await sut.GetTwitterAccount(null, "username");

            request.Headers.Authorization.Scheme.ShouldBe("Bearer");
            request.Headers.Authorization.Parameter.ShouldBe(AccessToken);
        }

        [Fact]
        public async Task GetTwitterAccount_RequestBearerToken_WhenOnlyScreennameSent_ShouldCallHttpClientTheSecondTime_WithCorrectUrl()
        {
            HttpRequestMessage request = null;

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(BearerTokenTypeResponse);

            var calls = 0;
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(response)
                .Callback<HttpRequestMessage>(x =>
                {
                    calls++;
                    if (calls == 2)
                    {
                        request = x;
                    }
                });

            var options = new Mock<IOptions<TwitterAuthenticationSettings>>();
            options.Setup(x => x.Value).Returns(new TwitterAuthenticationSettings { ConsumerKey = "KEY", ConsumerSecret = "SECRET" });

            var sut = new TwitterService(options.Object, httpClient.Object);

            await sut.GetTwitterAccount(null, "username");

            request.RequestUri.ShouldBe(new Uri("https://api.twitter.com/1.1/users/show.json?screen_name=username"));
        }

        [Fact]
        public async Task GetTwitterAccount_RequestBearerToken_WhenOnlyUserIdSent_ShouldCallHttpClientTheSecondTime_WithCorrectUrl()
        {
            HttpRequestMessage request = null;

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(BearerTokenTypeResponse);

            var calls = 0;
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(response)
                .Callback<HttpRequestMessage>(x =>
                {
                    calls++;
                    if (calls == 2)
                    {
                        request = x;
                    }
                });

            var options = new Mock<IOptions<TwitterAuthenticationSettings>>();
            options.Setup(x => x.Value).Returns(new TwitterAuthenticationSettings { ConsumerKey = "KEY", ConsumerSecret = "SECRET" });

            var sut = new TwitterService(options.Object, httpClient.Object);

            await sut.GetTwitterAccount("12345", null);

            request.RequestUri.ShouldBe(new Uri("https://api.twitter.com/1.1/users/show.json?user_id=12345"));
        }

        [Fact]
        public async Task GetTwitterAccount_RequestBearerToken_WhenUserIdAndScreennameSent_ShouldCallHttpClientTheSecondTime_WithCorrectUrl()
        {
            HttpRequestMessage request = null;

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(BearerTokenTypeResponse);

            var calls = 0;
            var httpClient = new Mock<IHttpClient>();
            httpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(response)
                .Callback<HttpRequestMessage>(x =>
                {
                    calls++;
                    if (calls == 2)
                    {
                        request = x;
                    }
                });

            var options = new Mock<IOptions<TwitterAuthenticationSettings>>();
            options.Setup(x => x.Value).Returns(new TwitterAuthenticationSettings { ConsumerKey = "KEY", ConsumerSecret = "SECRET" });

            var sut = new TwitterService(options.Object, httpClient.Object);

            await sut.GetTwitterAccount("12345", "username");

            request.RequestUri.ShouldBe(new Uri("https://api.twitter.com/1.1/users/show.json?user_id=12345&screen_name=username"));
        }

        [Fact]
        public async Task GetTwitterAccount_ShouldReturnNull_WhenShowUserCallReturnsNonSuccessCode()
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(BearerTokenTypeResponse);

            var userResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            userResponse.Content = new StringContent(ShowUserResponse);

            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupSequence(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(response)
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest));

            var options = new Mock<IOptions<TwitterAuthenticationSettings>>();
            options.Setup(x => x.Value).Returns(new TwitterAuthenticationSettings { ConsumerKey = "KEY", ConsumerSecret = "SECRET" });

            var sut = new TwitterService(options.Object, httpClient.Object);

            var result = await sut.GetTwitterAccount(null, "username");

            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTwitterAccount_ShouldReturnCorrectTwitterUserInfo_WhenShowUserCallReturnsOkResultWithData()
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(BearerTokenTypeResponse);

            var userResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            userResponse.Content = new StringContent(ShowUserResponse);

            var httpClient = new Mock<IHttpClient>();
            httpClient.SetupSequence(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(response)
                .ReturnsAsync(userResponse);

            var options = new Mock<IOptions<TwitterAuthenticationSettings>>();
            options.Setup(x => x.Value).Returns(new TwitterAuthenticationSettings { ConsumerKey = "KEY", ConsumerSecret = "SECRET" });

            var sut = new TwitterService(options.Object, httpClient.Object);

            var result = await sut.GetTwitterAccount(null, "username");

            result.Name.ShouldBe(TwitterName);
        }
    }
}
