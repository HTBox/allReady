using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Providers.ExternalUserInformationProviders.Providers;
using AllReady.Services.Twitter;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Providers.ExternalUserInformationProviders.Providers
{
    public class TwitterExternalUserInformationProviderShould
    {
        [Fact]
        public async Task InvokeGetTwitterAccountWithCorrectParameters()
        {
            const string userId = "UserId";
            const string screenName = "ScreenName";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("urn:twitter:userid", userId),
                new Claim("urn:twitter:screenname", screenName)
            }));

            var twitterRepository = new Mock<ITwitterService>();

            var sut = new TwitterExternalUserInformationProvider(twitterRepository.Object);
            await sut.GetExternalUserInformation(new ExternalLoginInfo(claimsPrincipal, null, null, null));

            twitterRepository.Verify(x => x.GetTwitterAccount(userId, screenName), Times.Once);
        }

        [Fact]
        public async Task ReturnCorrectExternalUserInformationWhenTwitterAccountIsNull()
        {
            var sut = new TwitterExternalUserInformationProvider(Mock.Of<ITwitterService>());
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task ReturnCorrectExternalUserInformationWhenTwitterAccountUserIsNull()
        {
            var twitterRepository = new Mock<ITwitterService>();
            twitterRepository.Setup(x => x.GetTwitterAccount(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TwitterUserInfo());

            var sut = new TwitterExternalUserInformationProvider(twitterRepository.Object);
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task ReturnCorrectExternalUserInformationWhenTwitterUserNameIsNull()
        {
            var twitterRepository = new Mock<ITwitterService>();
            twitterRepository.Setup(x => x.GetTwitterAccount(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TwitterUserInfo());

            var sut = new TwitterExternalUserInformationProvider(twitterRepository.Object);
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task ReturnCorrectExternalUserInformationWhenTwitterUserNameIsEmpty()
        {
            var twitterRepository = new Mock<ITwitterService>();
            twitterRepository.Setup(x => x.GetTwitterAccount(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TwitterUserInfo());

            var sut = new TwitterExternalUserInformationProvider(twitterRepository.Object);
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task ReturnCorrectExternalUserInformationWhenUserNameHasNoWhiteSpace()
        {
            var twitterRepository = new Mock<ITwitterService>();
            twitterRepository.Setup(x => x.GetTwitterAccount(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TwitterUserInfo());

            var sut = new TwitterExternalUserInformationProvider(twitterRepository.Object);
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task ReturnCorrectExternalUserInformationWhenUserNameHasWhiteSpace()
        {
            const string firstName = "FirstName";
            const string lastName = "LastName";

            var twitterRepository = new Mock<ITwitterService>();
            twitterRepository.Setup(x => x.GetTwitterAccount(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TwitterUserInfo { Name = $"{firstName} {lastName}" });

            var sut = new TwitterExternalUserInformationProvider(twitterRepository.Object);
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Equal(result.FirstName, firstName);
            Assert.Equal(result.LastName, lastName);
        }
    }
}