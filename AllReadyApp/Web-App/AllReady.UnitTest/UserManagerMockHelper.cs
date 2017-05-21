using AllReady.Models;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace AllReady.UnitTest
{
    public static class UserManagerMockHelper
    {
        public static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            return new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        }
    }
}