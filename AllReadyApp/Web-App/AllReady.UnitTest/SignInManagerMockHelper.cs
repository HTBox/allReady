using AllReady.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace AllReady.UnitTest
{
    public static class SignInManagerMockHelper
    {
        public static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(IMock<UserManager<ApplicationUser>> userManagerMock = null)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(mock => mock.HttpContext).Returns(Mock.Of<HttpContext>);

            return new Mock<SignInManager<ApplicationUser>>(
                userManagerMock == null ? UserManagerMockHelper.CreateUserManagerMock().Object : userManagerMock.Object,
                contextAccessor.Object, Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null);
        }
    }
}
