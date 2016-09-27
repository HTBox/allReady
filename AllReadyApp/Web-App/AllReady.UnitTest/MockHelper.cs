using AllReady.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace AllReady.UnitTest
{
    public static class MockHelper
    {
        public static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(IMock<UserManager<ApplicationUser>> userManagerMock = null)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(mock => mock.HttpContext).Returns(Mock.Of<HttpContext>);

            return new Mock<SignInManager<ApplicationUser>>(
                userManagerMock == null ? 
                    CreateUserManagerMock().Object : 
                    userManagerMock.Object,
                contextAccessor.Object, Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null);
        }

        public static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            return new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        }
    }
}
