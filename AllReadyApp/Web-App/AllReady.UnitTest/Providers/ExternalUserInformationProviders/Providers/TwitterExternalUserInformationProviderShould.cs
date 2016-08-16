﻿using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Providers.ExternalUserInformationProviders.Providers;
using LinqToTwitter;
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

            var twitterRepository = new Mock<ITwitterRepository>();

            var sut = new TwitterExternalUserInformationProvider(twitterRepository.Object);
            await sut.GetExternalUserInformation(new ExternalLoginInfo(claimsPrincipal, null, null, null));

            twitterRepository.Verify(x => x.GetTwitterAccount(userId, screenName), Times.Once);
        }

        [Fact]
        public async Task ReturnCorrectExternalUserInformationWhenTwitterAccountIsNull()
        {
            var sut = new TwitterExternalUserInformationProvider(Mock.Of<ITwitterRepository>());
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Null(result.Email);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task ReturnCorrectExternalUserInformationWhenTwitterAccountUserIsNull()
        {
            var twitterRepository = new Mock<ITwitterRepository>();
            twitterRepository.Setup(x => x.GetTwitterAccount(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Account());

            var sut = new TwitterExternalUserInformationProvider(twitterRepository.Object);
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Null(result.Email);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task ReturnCorrectExternalUserInformationWhenTwitterUserNameIsNull()
        {
            const string email = "email";

            var twitterRepository = new Mock<ITwitterRepository>();
            twitterRepository.Setup(x => x.GetTwitterAccount(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Account { User = new User { Email = email }});

            var sut = new TwitterExternalUserInformationProvider(twitterRepository.Object);
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Equal(result.Email, email);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task ReturnCorrectExternalUserInformationWhenTwitterUserNameIsEmpty()
        {
            const string email = "email";

            var twitterRepository = new Mock<ITwitterRepository>();
            twitterRepository.Setup(x => x.GetTwitterAccount(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Account { User = new User { Name = string.Empty, Email = email } });

            var sut = new TwitterExternalUserInformationProvider(twitterRepository.Object);
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Equal(result.Email, email);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task ReturnCorrectExternalUserInformationWhenUserNameHasNoWhiteSpace()
        {
            const string email = "email";

            var twitterRepository = new Mock<ITwitterRepository>();
            twitterRepository.Setup(x => x.GetTwitterAccount(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Account { User = new User { Name = "FirstNameLastName", Email = email } });

            var sut = new TwitterExternalUserInformationProvider(twitterRepository.Object);
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Equal(result.Email, email);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task ReturnCorrectExternalUserInformationWhenUserNameHasWhiteSpace()
        {
            const string email = "email";
            const string firstName = "FirstName";
            const string lastName = "LastName";

            var twitterRepository = new Mock<ITwitterRepository>();
            twitterRepository.Setup(x => x.GetTwitterAccount(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Account { User = new User { Name = $"{firstName} {lastName}", Email = email } });

            var sut = new TwitterExternalUserInformationProvider(twitterRepository.Object);
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Equal(result.Email, email);
            Assert.Equal(result.FirstName, firstName);
            Assert.Equal(result.LastName, lastName);
        }
    }
}