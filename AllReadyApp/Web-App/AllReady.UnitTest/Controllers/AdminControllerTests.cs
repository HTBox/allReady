using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Services;
using AllReady.UnitTest.Extensions;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Moq;
using Xunit;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.AspNet.Http;

namespace AllReady.UnitTest.Controllers
{
    public class AdminControllerTests
    {
        [Fact]
        public void RegisterReturnsViewResult()
        {
            var sut = CreateConstructableAdminController();
            var result = sut.Register();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void RegisterHasHttpGetAttribute()
        {
            var sut = CreateConstructableAdminController();
            var attribute = sut.GetAttributesOn(x => x.Register()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterHasAllowAnonymousAttribute()
        {
            var sut = CreateConstructableAdminController();
            var attribute = sut.GetAttributesOn(x => x.Register()).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task RegisterReturnsViewResultWhenModelStateIsNotValid()
        {
            var sut = CreateConstructableAdminController();
            sut.AddModelStateError();

            var result = await sut.Register(It.IsAny<RegisterViewModel>());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task RegisterReturnsCorrectModelWhenModelStateIsNotValid()
        {
            var model = new RegisterViewModel();

            var sut = CreateConstructableAdminController();
            sut.AddModelStateError();

            var result = await sut.Register(model) as ViewResult;
            var modelResult = result.ViewData.Model as RegisterViewModel;

            Assert.IsType<RegisterViewModel>(modelResult);
            Assert.Same(model, modelResult);
        }

        [Fact]
        public async Task RegisterInvokesCreateAsyncWithCorrectUserAndPassword()
        {
            const string defaultTimeZone = "DefaultTimeZone";

            var model = new RegisterViewModel { Email = "email", Password = "Password" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = defaultTimeZone });

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Failed()));

            var sut = new AdminController(userManager.Object, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);

            await sut.Register(model);

            userManager.Verify(x => x.CreateAsync(It.Is<ApplicationUser>(au =>
                au.UserName == model.Email &&
                au.Email == model.Email &&
                au.TimeZoneId == defaultTimeZone),
                model.Password));
        }

        [Fact]
        public async Task RegisterInvokesGenerateEmailConfirmationTokenAsyncWithCorrectUserWhenUserCreationIsSuccessful()
        {
            const string defaultTimeZone = "DefaultTimeZone";

            var model = new RegisterViewModel { Email = "email", Password = "Password" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = defaultTimeZone });

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));

            var sut = new AdminController(userManager.Object, null, Mock.Of<IEmailSender>(), null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = Mock.Of<IUrlHelper>();

            await sut.Register(model);

            userManager.Verify(x => x.GenerateEmailConfirmationTokenAsync(It.Is<ApplicationUser>(au =>
                au.UserName == model.Email &&
                au.Email == model.Email &&
                au.TimeZoneId == defaultTimeZone)));
        }

        [Fact]
        public async Task RegisterInvokesUrlActionWithCorrectParametersWhenUserCreationIsSuccessful()
        {
            const string requestScheme = "requestScheme";

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).Returns(() => Task.FromResult(It.IsAny<string>()));

            var sut = new AdminController(userManager.Object, null, Mock.Of<IEmailSender>(), null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(requestScheme);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            await sut.Register(new RegisterViewModel());

            //note: I can't test the Values part here b/c I do not have control over the Id generation on ApplicationUser b/c it's new'ed up in the controller
            urlHelper.Verify(mock => mock.Action(It.Is<UrlActionContext>(uac =>
                uac.Action == "ConfirmEmail" &&
                uac.Controller == "Admin" &&
                uac.Protocol == requestScheme)),
                Times.Once);
        }

        [Fact]
        public async Task RegisterInvokesSendEmailAsyncWithCorrectParametersWhenUserCreationIsSuccessful()
        {
            const string callbackUrl = "callbackUrl";

            var model = new RegisterViewModel { Email = "email" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).Returns(() => Task.FromResult(It.IsAny<string>()));

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(callbackUrl);

            var emailSender = new Mock<IEmailSender>();

            var sut = new AdminController(userManager.Object, null, emailSender.Object, null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            await sut.Register(model);

            emailSender.Verify(x => x.SendEmailAsync(model.Email, "Confirm your account", $"Please confirm your account by clicking this <a href=\"{callbackUrl}\">link</a>"));
        }

        [Fact]
        public async Task RegisterRedirectsToCorrectActionWhenUserCreationIsSuccessful()
        {
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).Returns(() => Task.FromResult(It.IsAny<string>()));

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new AdminController(userManager.Object, null, Mock.Of<IEmailSender>(), null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            var result = await sut.Register(new RegisterViewModel()) as RedirectToActionResult;

            Assert.Equal(result.ActionName, nameof(AdminController.DisplayEmail));
            Assert.Equal(result.ControllerName, "Admin");
        }

        [Fact]
        public async Task RegisterAddsIdentityResultErrorsToModelStateErrorsWhenUserCreationIsNotSuccessful()
        {
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "IdentityErrorDescription" });

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(identityResult));

            var sut = new AdminController(userManager.Object, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());

            await sut.Register(new RegisterViewModel());

            var errorMessages = sut.ModelState.GetErrorMessages();

            Assert.Equal(errorMessages.Single(), identityResult.Errors.Select(x => x.Description).Single());
        }

        [Fact]
        public async Task RegisterReturnsViewResultAndCorrectModelWhenUserCreationIsNotSuccessful()
        {
            var model = new RegisterViewModel();

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Failed()));

            var sut = new AdminController(userManager.Object, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());

            var result = await sut.Register(model) as ViewResult;
            var modelResult = result.ViewData.Model as RegisterViewModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<RegisterViewModel>(modelResult);
            Assert.Same(model, modelResult);
        }

        [Fact]
        public void DisplayEmailReturnsViewResult()
        {
            var sut = CreateConstructableAdminController();
            var result = sut.DisplayEmail();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void DisplayEmailHasHttpGetAttribute()
        {
            var sut = CreateConstructableAdminController();
            var attribute = sut.GetAttributesOn(x => x.DisplayEmail()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DisplayEmailHasAllowAnonymousAttribute()
        {
            var sut = CreateConstructableAdminController();
            var attribute = sut.GetAttributesOn(x => x.DisplayEmail()).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task ConfirmEmailReturnsErrorWhenCodeIsNull()
        {
            var sut = CreateConstructableAdminController();
            var result = await sut.ConfirmEmail(null, null) as ViewResult;
            Assert.Equal(result.ViewName, "Error");
        }

        [Fact]
        public async Task ConfirmEmailReturnsErrorWhenCannotFindUserByUserId()
        {
            var userManager = CreateUserManagerMock();
            var sut = new AdminController(userManager.Object, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), Mock.Of<IOptions<GeneralSettings>>());
            var result = await sut.ConfirmEmail(null, "code") as ViewResult;
            Assert.Equal(result.ViewName, "Error");
        }

        [Fact]
        public async Task ConfirmEmailInvokesFindByIdAsyncWithCorrectUserId()
        {
            const string userId = "userId";
            var userManager = CreateUserManagerMock();
            var sut = new AdminController(userManager.Object, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), Mock.Of<IOptions<GeneralSettings>>());
            await sut.ConfirmEmail(userId, "code");

            userManager.Verify(x => x.FindByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailInvokesConfirmEmailAsyncWithCorrectUserAndCode()
        {
            const string code = "code";
            var user = new ApplicationUser();
            
            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Returns(() => Task.FromResult(user));
            userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Failed()));

            var sut = new AdminController(userManager.Object, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), Mock.Of<IOptions<GeneralSettings>>());
            await sut.ConfirmEmail(null, code);

            userManager.Verify(x => x.ConfirmEmailAsync(user, code), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailInvokesUrlActionWithCorrectParametersWhenUsersEmailIsConfirmedSuccessfully()
        {
            const string requestScheme = "requestScheme";

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Returns(() => Task.FromResult(new ApplicationUser()));
            userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));

            var settings = new Mock<IOptions<SampleDataSettings>>();
            settings.Setup(x => x.Value).Returns(new SampleDataSettings());

            var urlHelper = new Mock<IUrlHelper>();

            var sut = new AdminController(userManager.Object, null, Mock.Of<IEmailSender>(), null, settings.Object, Mock.Of<IOptions<GeneralSettings>>());
            sut.SetFakeHttpRequestSchemeTo(requestScheme);
            sut.Url = urlHelper.Object;

            await sut.ConfirmEmail(It.IsAny<string>(), "code");

            //note: I can't test the Values part here b/c I do not have control over the Id generation on ApplicationUser b/c it's new'ed up in the controller
            urlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(uac =>
                uac.Action == "EditUser" &&
                uac.Controller == "Site" &&
                uac.Protocol == requestScheme)),
                Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailInvokesSendEmailAsyncWithCorrectParametersWhenUsersEmailIsConfirmedSuccessfully()
        {
            const string defaultAdminUserName = "requestScheme";
            const string callbackUrl = "callbackUrl";

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Returns(() => Task.FromResult(new ApplicationUser()));
            userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));

            var settings = new Mock<IOptions<SampleDataSettings>>();
            settings.Setup(x => x.Value).Returns(new SampleDataSettings { DefaultAdminUsername = defaultAdminUserName });

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(callbackUrl);

            var emailSender = new Mock<IEmailSender>();

            var sut = new AdminController(userManager.Object, null, emailSender.Object, null, settings.Object, Mock.Of<IOptions<GeneralSettings>>());
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            await sut.ConfirmEmail(It.IsAny<string>(), "code");

            emailSender.Verify(x => x.SendEmailAsync(defaultAdminUserName, "Approve organization user account", 
                $"Please approve this account by clicking this <a href=\"{callbackUrl}\">link</a>"));
        }

        [Fact]
        public async Task ConfirmEmailReturnsCorrectViewWhenUsersConfirmationIsSuccessful()
        {
            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Returns(() => Task.FromResult(new ApplicationUser()));
            userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var settings = new Mock<IOptions<SampleDataSettings>>();
            settings.Setup(x => x.Value).Returns(new SampleDataSettings { DefaultAdminUsername = It.IsAny<string>() });

            var sut = new AdminController(userManager.Object, null, Mock.Of<IEmailSender>(), null, settings.Object, Mock.Of<IOptions<GeneralSettings>>());
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            var result = await sut.ConfirmEmail("userId", "code") as ViewResult;

            Assert.Equal(result.ViewName, "ConfirmEmail");
        }

        [Fact]
        public async Task ConfirmEmailReturnsCorrectViewWhenUsersConfirmationIsUnsuccessful()
        {
            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Returns(() => Task.FromResult(new ApplicationUser()));
            userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Failed()));

            var sut = new AdminController(userManager.Object, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), Mock.Of<IOptions<GeneralSettings>>());

            var result = await sut.ConfirmEmail("userId", "code") as ViewResult;

            Assert.Equal(result.ViewName, "Error");
        }

        [Fact]
        public void ConfirmEmailHasHttpGetAttribute()
        {
            var sut = CreateConstructableAdminController();
            var attribute = sut.GetAttributesOn(x => x.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ConfirmEmailHasAllowAnonymousAttribute()
        {
            var sut = CreateConstructableAdminController();
            var attribute = sut.GetAttributesOn(x => x.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ForgotPasswordReturnsView()
        {
            var sut = CreateConstructableAdminController();
            var result = sut.ForgotPassword();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ForgotPasswordHasHttpGetAttribute()
        {
            var sut = CreateConstructableAdminController();
            var attribute = sut.GetAttributesOn(x => x.ForgotPassword()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ForgotPasswordHasAllowAnonymousAttribute()
        {
            var sut = CreateConstructableAdminController();
            var attribute = sut.GetAttributesOn(x => x.ForgotPassword()).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task SendCodeInvokesGetTwoFactorAuthenticationUserAsync()
        {
            var signInManager = CreateSignInManagerMock();
            var sut = new AdminController(null, signInManager.Object, null, null, Mock.Of<IOptions<SampleDataSettings>>(), Mock.Of<IOptions<GeneralSettings>>());
            await sut.SendCode(It.IsAny<string>(), It.IsAny<bool>());

            signInManager.Verify(x => x.GetTwoFactorAuthenticationUserAsync(), Times.Once);
        }

        private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            return new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null, null);
        }

        private static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock()
        {
            //userManager, contextAccessor and claimsFactory are all required to construct
            //public SignInManager(Microsoft.AspNet.Identity.UserManager<TUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<TUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<TUser>> logger)
            var userManager = CreateUserManagerMock();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            //contextAccessor.Setup(mock => mock.HttpContext).Returns(() => mockHttpContext.Object);
            return new Mock<SignInManager<ApplicationUser>>(userManager.Object, contextAccessor.Object, Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null);
            //var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            //    userManagerMock.Object,
            //    contextAccessor.Object,
            //    claimsFactory.Object,
            //    null, null);
        }

        private AdminController CreateConstructableAdminController()
        {
            //this simpliest controller to create where calling new will not throw and exception.
            return new AdminController(null, null, null, null, Mock.Of<IOptions<SampleDataSettings>>(), Mock.Of<IOptions<GeneralSettings>>());
        }
    }
}
