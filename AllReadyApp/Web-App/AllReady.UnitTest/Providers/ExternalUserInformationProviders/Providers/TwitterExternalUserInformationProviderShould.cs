using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Providers.ExternalUserInformationProviders.Providers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Providers.ExternalUserInformationProviders.Providers
{
    public class TwitterExternalUserInformationProviderShould
    {
        [Fact]
        public async Task ReturnExternalUserInformationWithEmailPopulatedWhenEmailClaimIsPopulated()
        {
            const string screenName = "ScreenName";
            const string emailAddress = "someone@emailaddress.com";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("urn:twitter:screenname", screenName),
                new Claim(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",emailAddress), 
            }));

            var sut = CreateSut();
            var userInfo = await sut.GetExternalUserInformation(new ExternalLoginInfo(claimsPrincipal, null, null, null));

            Assert.Equal(emailAddress, userInfo.Email);
        }

        [Fact]
        public async Task ReturnCorrectExternalUserInformationWhenTwitterAccountUserIsNull()
        {
            var logger = new Mock<ILogger<TwitterExternalUserInformationProvider>>();
            var sut = CreateSut(logger);
            var result = await sut.GetExternalUserInformation(new ExternalLoginInfo(new ClaimsPrincipal(), null, null, null));

            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
            Assert.Null(result.Email);
            logger.Verify(
                m =>
                    m.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(),
                        It.IsAny<Func<object, Exception, string>>()));

        }

        private static TwitterExternalUserInformationProvider CreateSut(Mock<ILogger<TwitterExternalUserInformationProvider>> logger = null)
        {
            if (logger == null)
                logger = new Mock<ILogger<TwitterExternalUserInformationProvider>>();

            var sut = new TwitterExternalUserInformationProvider(logger.Object);
            return sut;
        }
    }
}