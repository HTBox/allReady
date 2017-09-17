using System.Linq;
using AllReady.Controllers;
using AllReady.UnitTest.Extensions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using MediatR;
using System.Threading.Tasks;
using AllReady.ViewModels.Account;

namespace AllReady.UnitTest.Controllers
{
    public class MeApiControllerTests
    {
        [Fact]
        public async Task LoginReturnsCorrectCookieString()
        {
            var model = new LoginViewModel()
            {
                Email = "Administrator@example.com",
                Password = "YouShouldChangeThisPassword1!"
            };

            var mediator = new Mock<IMediator>();
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            signInManager.Setup(
                    x =>
                        x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                            It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var sut = new MeApiController(userManager.Object, signInManager.Object, mediator.Object);
            var mockedHttpRequest = sut.GetMockHttpRequest();

            await sut.Login(model);

            mockedHttpRequest.Verify(x => x.Cookies[".AspNet.ApplicationCookie"], Times.Once());
        }

        [Fact]
        public void LoginMethodHasHttpPostAtttribute()
        {
            var mediator = new Mock<IMediator>();
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var sut = new MeApiController(userManager.Object, signInManager.Object, mediator.Object);

            var attribute =
                sut.GetAttributesOn(x => x.Login(new LoginViewModel())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ControllerHasRouteAttributeWithRoute()
        {
            var mediator = new Mock<IMediator>();
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var sut = new MeApiController(userManager.Object, signInManager.Object, mediator.Object);

            var attribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("api/me", attribute.Template);
        }

        [Fact]
        public void LoginHasRouteAttributeWithRoute()
        {
            var mediator = new Mock<IMediator>();
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var sut = new MeApiController(userManager.Object, signInManager.Object, mediator.Object);

            var attribute =
                sut.GetAttributesOn(x => x.Login(new LoginViewModel())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("login", attribute.Template);
        }
    }
}