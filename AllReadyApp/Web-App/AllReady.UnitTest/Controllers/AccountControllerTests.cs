using System.Linq;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using MediatR;
using AllReady.UnitTest.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Claims;
using AllReady.Configuration;
using AllReady.Extensions;
using AllReady.Features.Login;
using AllReady.Features.Manage;
using AllReady.Providers.ExternalUserInformationProviders;
using AllReady.Providers.ExternalUserInformationProviders.Providers;
using AllReady.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace AllReady.UnitTest.Controllers
{
    public class AccountControllerTests
    {
        [Fact (Skip="Skipped 2.0 - Hard to test as we can't easily mock the SignInAsync ext on HttpContext")]
        public async Task LoginGetPopulatesViewDataWithTheCorrectTestUrl()
        {
            var sut = AccountController();
            
            var result = await sut.Login();

            var viewResult = (ViewResult)result;
            Assert.Null(viewResult.ViewData["ReturnUrl"]);

            const string testUrl = "return url";
            result = await sut.Login(testUrl);
            viewResult = (ViewResult)result;
            Assert.Equal(testUrl, viewResult.ViewData["ReturnUrl"]);
        }

        [Fact]
        public void LoginGetHasHttpGetAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Login(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void LoginGetHasAllowAnonymousAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Login(It.IsAny<string>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task LoginPostAssignsCorrectValueToReturnUrlOnViewData()
        {
            const string returnUrl = "ReturnUrl";

            var sut = CreateAccountControllerWithNoInjectedDependencies();
            sut.AddModelStateError();

            var result = await sut.Login(new LoginViewModel(), returnUrl) as ViewResult;

            Assert.Equal(result.ViewData["ReturnUrl"], returnUrl);
        }

        [Fact]
        public async Task LoginPostReturnsSameViewAndViewModel_WhenModelStatIsInvalid()
        {
            const string testUrl = "return url";
            var loginViewModel = new LoginViewModel();

            var sut = CreateAccountControllerWithNoInjectedDependencies();
            sut.AddModelStateError();

            var result = await sut.Login(loginViewModel, testUrl);
            Assert.IsType<ViewResult>(result);

            var resultViewModel = ((ViewResult)result).ViewData.Model;
            Assert.IsType<LoginViewModel>(resultViewModel);
            Assert.Equal(resultViewModel, loginViewModel);
        }

        [Fact]
        public async Task LoginPostSendsApplicationUserQueryWithTheCorrectEmail()
        {
            var model = new LoginViewModel { Email = "email" };
            var mediator = new Mock<IMediator>();

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(UserManagerMockHelper.CreateUserManagerMock());
            signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var sut = new AccountController(null, signInManager.Object, null, mediator.Object, null, null);
            await sut.Login(model);

            mediator.Verify(x => x.SendAsync(It.Is<ApplicationUserQuery>(y => y.UserName == model.Email)), Times.Once);
        }

        [Fact]
        public async Task LoginPostReturnsErrorViewWithCorrectTextInViewData_WhenUserIsNotNull_AndUserIsAnOrgAdmin_AndUsersEmailIsNotConfirmed()
        {
            var applicationUser = new ApplicationUser();
            applicationUser.MakeOrgAdmin();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ApplicationUserQuery>())).ReturnsAsync(applicationUser);

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.IsEmailConfirmedAsync(applicationUser)).ReturnsAsync(false).Verifiable();

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var sut = new AccountController(userManager.Object, signInManager.Object, null, mediator.Object, null, null);
            var result = await sut.Login(new LoginViewModel()) as ViewResult;

            Assert.Equal(result.ViewData["Message"], "You must have a confirmed email to log on.");
            Assert.Equal(result.ViewName, "Error");
        }

        [Fact]
        public async Task LoginPostReturnsErrorViewWithCorrectTextInViewData_WhenUserIsNotNull_AndUserIsASiteAdmin_AndUsersEmailIsNotConfirmed()
        {
            var applicationUser = new ApplicationUser();
            applicationUser.MakeSiteAdmin();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ApplicationUserQuery>())).ReturnsAsync(applicationUser);

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.IsEmailConfirmedAsync(applicationUser)).ReturnsAsync(false).Verifiable();

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var sut = new AccountController(userManager.Object, signInManager.Object, null, mediator.Object, null, null);
            var result = await sut.Login(new LoginViewModel()) as ViewResult;

            Assert.Equal(result.ViewData["Message"], "You must have a confirmed email to log on.");
            Assert.Equal(result.ViewName, "Error");
        }

        [Fact]
        public async Task LoginPostInvokesPasswordSignInAsyncWithCorrectParameters_WhenUserIsNull()
        {
            var model = new LoginViewModel { Email = "email", Password = "password", RememberMe = true };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ApplicationUserQuery>())).ReturnsAsync(new ApplicationUser());

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(UserManagerMockHelper.CreateUserManagerMock());
            signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var sut = new AccountController(null, signInManager.Object, null, mediator.Object, null, null);
            await sut.Login(model);

            signInManager.Verify(x => x.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true), Times.Once);
        }

        [Fact]
        public async Task LoginPostInvokesRedirectToLocalWithCorrectParameters_WhenUserIsNull_AndResultSucceeded()
        {
            const string returnUrl = "returnUrl";
            var applicationUser = new ApplicationUser();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ApplicationUserQuery>())).ReturnsAsync(applicationUser);

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(UserManagerMockHelper.CreateUserManagerMock());
            signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var redirectAccountControllerRequests = new Mock<IRedirectAccountControllerRequests>();
            var urlHelper = new Mock<IUrlHelper>();

            var sut = new AccountController(null, signInManager.Object, null, mediator.Object, null, redirectAccountControllerRequests.Object)
            {
                Url = urlHelper.Object
            };
            await sut.Login(new LoginViewModel(), returnUrl);

            redirectAccountControllerRequests.Verify(x => x.RedirectToLocal(returnUrl, applicationUser));
        }

        [Fact]
        public async Task LoginPostRedirectsToCorrectActionWithCorrectRouteValues_WhenUserIsNull_AndResultRequiresTwoFactorIsTrue()
        {
            const string returnUrl = "returnUrl";

            var model = new LoginViewModel { RememberMe = true };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ApplicationUserQuery>())).ReturnsAsync(new ApplicationUser());

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(UserManagerMockHelper.CreateUserManagerMock());
            signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.TwoFactorRequired);

            var routeValueDictionary = new RouteValueDictionary
            {
                ["ReturnUrl"] = returnUrl,
                ["RememberMe"] = model.RememberMe
            };

            var sut = new AccountController(null, signInManager.Object, null, mediator.Object, null, null);

            var result = await sut.Login(model, returnUrl) as RedirectToActionResult;

            Assert.Equal(result.ActionName, nameof(AdminController.SendCode));
            Assert.Equal(result.ControllerName, "Admin");
            Assert.Equal(result.RouteValues, routeValueDictionary);
        }

        [Fact]
        public async Task LoginPostReturnsLockoutView_WhenUserIsNull_AndResultIsLockedOutIsTrue()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<ApplicationUserQuery>())).ReturnsAsync(new ApplicationUser());

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(UserManagerMockHelper.CreateUserManagerMock());
            signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

            var sut = new AccountController(null, signInManager.Object, null, mediator.Object, null, null);

            var result = await sut.Login(new LoginViewModel()) as ViewResult;

            Assert.Equal(result.ViewName, "Lockout");
        }

        [Fact]
        public void LoginPostHasHttpPostAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Login(It.IsAny<LoginViewModel>(), It.IsAny<string>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void LoginPostHasAllowAnonymousAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Login(It.IsAny<LoginViewModel>(), It.IsAny<string>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void LoginPostHasValidateAntiForgeryTokenAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Login(It.IsAny<LoginViewModel>(), It.IsAny<string>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterGetReturnsAView()
        {
            var sut = AccountController();
            var result = (ViewResult)sut.Register();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void RegisterGetHasHttpGetAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Register()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterGetHasAllowAnonymousAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Register()).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task RegisterPostReturnsSameViewAndViewModel_WhenModelStateIsInvalid()
        {
            var registerViewModel = new RegisterViewModel();

            var sut = AccountController();
            sut.ModelState.AddModelError("foo", "bar");

            var result = await sut.Register(registerViewModel);
            Assert.IsType<ViewResult>(result);

            var resultViewModel = ((ViewResult)result).ViewData.Model;
            Assert.IsType<RegisterViewModel>(resultViewModel);
            Assert.Equal(resultViewModel, registerViewModel);
        }

        [Fact]
        public async Task RegisterPostInvokesCreateAsyncWithTheCorrectParameters_WhenModelStateIsValid()
        {
            const string defaultTimeZone = "DefaultTimeZone";

            var model = new RegisterViewModel { Email = "email", Password = "Password" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = defaultTimeZone });

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var sut = new AccountController(userManager.Object, null, generalSettings.Object, null, null, null);
            await sut.Register(model);

            userManager.Verify(x => x.CreateAsync(It.Is<ApplicationUser>(au =>
                au.UserName == model.Email &&
                au.Email == model.Email &&
                au.TimeZoneId == defaultTimeZone),
                model.Password), Times.Once);
        }

        [Fact]
        public async Task RegisterPostInvokesGenerateEmailConfirmationTokenAsyncWithTheCorrectParameters_WhenModelStateIsValid_AndUserCreationIsSuccessful()
        {
            const string defaultTimeZone = "DefaultTimeZone";

            var model = new RegisterViewModel { Email = "email", Password = "Password" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = defaultTimeZone });

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(It.IsAny<string>());

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, null);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = Mock.Of<IUrlHelper>();
            sut.TempData = Mock.Of<ITempDataDictionary>();

            await sut.Register(model);

            userManager.Verify(x => x.GenerateEmailConfirmationTokenAsync(It.Is<ApplicationUser>(au =>
                au.UserName == model.Email &&
                au.Email == model.Email &&
                au.TimeZoneId == defaultTimeZone)), Times.Once);
        }

        [Fact]
        public async Task RegisterPostInvokesUrlActionWithTheCorrectParameters_WhenModelStateIsValid_AndUserCreationIsSuccessful()
        {
            const string requestScheme = "requestScheme";

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(It.IsAny<string>());
            userManager.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(It.IsAny<string>());

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, null);
            sut.SetFakeHttpRequestSchemeTo(requestScheme);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;
            sut.TempData = Mock.Of<ITempDataDictionary>();

            await sut.Register(new RegisterViewModel());

            urlHelper.Verify(mock => mock.Action(It.Is<UrlActionContext>(uac =>
                uac.Action == "ConfirmEmail" &&
                uac.Controller == "Account" &&
                uac.Protocol == requestScheme)),
                Times.Once);
        }

        [Fact]
        public async Task RegisterPostSendsSendConfirmAccountEmailWithTheCorrectParameters_WhenModelStateIsValid_AndUserCreationIsSuccessful()
        {
            const string callbackUrl = "callbackUrl";
            var model = new RegisterViewModel { Email = "email" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(It.IsAny<string>());
            userManager.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(It.IsAny<string>());

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(callbackUrl);

            var mediator = new Mock<IMediator>();

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, mediator.Object, null, null);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;
            sut.TempData = Mock.Of<ITempDataDictionary>();

            await sut.Register(model);

            mediator.Verify(x => x.SendAsync(It.Is<SendConfirmAccountEmail>(y => y.Email == model.Email && y.CallbackUrl == callbackUrl)), Times.Once);
        }

        [Fact]
        public async Task RegisterPostInvokesGenerateChangePhoneNumberTokenAsyncWithTheCorrectParameters_WhenModelStateIsValid_AndUserCreationIsSuccessful()
        {
            var viewModel = new RegisterViewModel
            {
                FirstName = "firstName",
                LastName = "lastName",
                Password = "password",
                PhoneNumber = "566777222"
            };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(
                u =>
                    u.CreateAsync(
                        It.Is<ApplicationUser>(
                            user => user.FirstName == viewModel.FirstName && user.LastName == viewModel.LastName),
                        It.Is<string>(pass => pass == viewModel.Password)))
                .ReturnsAsync(IdentityResult.Success);
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var urlHelper = new Mock<IUrlHelper>();

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, null);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;
            sut.TempData = Mock.Of<ITempDataDictionary>();

            await sut.Register(viewModel);

            userManager.Verify(
                u =>
                    u.GenerateChangePhoneNumberTokenAsync(
                        It.Is<ApplicationUser>(
                            user => user.FirstName == viewModel.FirstName && user.LastName == viewModel.LastName),
                        It.Is<string>(phone => phone == viewModel.PhoneNumber)), Times.Once);
        }

        [Fact]
        public async Task RegisterPostSendsSendAccountSecurityTokenSmsWithCorrectParameters_WhenModelStateIsValid_AndUserCreationIsSuccessful()
        {
            var viewModel = new RegisterViewModel
            {
                FirstName = "firstName",
                LastName = "lastName",
                Password = "password",
                PhoneNumber = "566777222"
            };

            const string token = "abcdef12345";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(
                u =>
                    u.CreateAsync(
                        It.Is<ApplicationUser>(
                            user => user.FirstName == viewModel.FirstName && user.LastName == viewModel.LastName),
                        It.Is<string>(pass => pass == viewModel.Password)))
                .ReturnsAsync(IdentityResult.Success);
            userManager.Setup(u => u.GenerateChangePhoneNumberTokenAsync(It.Is<ApplicationUser>(
                user => user.FirstName == viewModel.FirstName && user.LastName == viewModel.LastName),
                It.Is<string>(phoneNumber => phoneNumber == viewModel.PhoneNumber)))
                .ReturnsAsync(token);
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var urlHelper = new Mock<IUrlHelper>();
            var mediator = new Mock<IMediator>();

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, mediator.Object, null, null);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;
            sut.TempData = Mock.Of<ITempDataDictionary>();

            await sut.Register(viewModel);

            mediator.Verify(
                m =>
                    m.SendAsync(
                        It.Is<SendAccountSecurityTokenSms>(
                            tokenSms => tokenSms.PhoneNumber == viewModel.PhoneNumber && tokenSms.Token == token)),
                Times.Once);
        }

        [Fact]
        public async Task RegisterPostInvokesAddClaimAsyncWithTheCorrectParameters_WhenModelStateIsValid_AndUserCreationIsSuccessful()
        {
            const string defaultTimeZone = "DefaultTimeZone";
            var model = new RegisterViewModel { Email = "email" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = defaultTimeZone });

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(It.IsAny<string>());
            userManager.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(It.IsAny<string>());

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, null);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;
            sut.TempData = Mock.Of<ITempDataDictionary>();

            await sut.Register(model);

            userManager.Verify(x => x.AddClaimAsync(It.Is<ApplicationUser>(au =>
                au.UserName == model.Email &&
                au.Email == model.Email &&
                au.TimeZoneId == defaultTimeZone), It.IsAny<Claim>()), Times.Once);
        }

        [Fact]
        public async Task RegisterPostInvokesSignInAsyncWithTheCorrectParameters_WhenModelStateIsValid_AndUserCreationIsSuccessful()
        {
            const string defaultTimeZone = "DefaultTimeZone";

            var model = new RegisterViewModel { Email = "email" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = defaultTimeZone });

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(It.IsAny<string>());
            userManager.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(It.IsAny<string>());

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, null);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;
            sut.TempData = Mock.Of<ITempDataDictionary>();

            await sut.Register(model);

            signInManager.Verify(x => x.SignInAsync(It.Is<ApplicationUser>(au =>
                au.UserName == model.Email &&
                au.Email == model.Email &&
                au.TimeZoneId == defaultTimeZone),
            It.IsAny<bool>(), null), Times.Once);
        }

        [Fact]
        public async Task RegisterPostRedirectsToCorrectActionAndController_WhenModelStateIsValid_AndUserCreationIsSuccessful()
        {
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(It.IsAny<string>());
            userManager.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(It.IsAny<string>());

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, null);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;
            sut.TempData = Mock.Of<ITempDataDictionary>();

            var result = await sut.Register(new RegisterViewModel()) as RedirectToPageResult;

            Assert.Equal(result.PageName, "/Index");
        }


        [Fact]
        public async Task RegisterPostSetsNewAccountFlag_WhenModelStateIsValid_AndUserCreationIsSuccessful()
        {
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(It.IsAny<string>());
            userManager.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(It.IsAny<string>());

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var tempData = new Mock<ITempDataDictionary>();
            tempData.SetupSet(x => x["NewAccount"] = true);

            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, null);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;
            sut.TempData = tempData.Object;

            var result = await sut.Register(new RegisterViewModel()) as RedirectToActionResult;

            tempData.VerifySet(x => x["NewAccount"] = true, Times.AtLeastOnce());
        }


        [Fact]
        public async Task RegisterPostAddsIdentityResultErrorsToModelStateError_WhenUserCreationFails()
        {
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "IdentityErrorDescription" });

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(identityResult);

            var sut = new AccountController(userManager.Object, null, generalSettings.Object, null, null, null);

            await sut.Register(new RegisterViewModel());

            var errorMessages = sut.ModelState.GetErrorMessages();
            Assert.Equal(errorMessages.Single(), identityResult.Errors.Select(x => x.Description).Single());
        }

        [Fact]
        public async Task RegisterPostReturnsTheSameViewAndViewModel_WhenUserCreationFails()
        {
            var model = new RegisterViewModel();

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var sut = new AccountController(userManager.Object, null, generalSettings.Object, null, null, null);
            var result = await sut.Register(model) as ViewResult;
            var modelResult = result.ViewData.Model as RegisterViewModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<RegisterViewModel>(modelResult);
            Assert.Same(model, modelResult);
        }

        [Fact]
        public void RegisterPostHasHttpPostAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Register(It.IsAny<RegisterViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterPostHasAllowAnonymousAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Register(It.IsAny<RegisterViewModel>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterPostHasValidateAntiForgeryTokenAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Register(It.IsAny<RegisterViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task LogOffInvokesSignOutAsync()
        {
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(UserManagerMockHelper.CreateUserManagerMock());

            var sut = new AccountController(null, signInManager.Object, null, null, null, null);
            await sut.LogOff();

            signInManager.Verify(x => x.SignOutAsync(), Times.Once);
        }

        [Fact]
        public async Task LogOffRedirectsToCorrectActionAndController()
        {
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(UserManagerMockHelper.CreateUserManagerMock());

            var sut = new AccountController(null, signInManager.Object, null, null, null, null);
            var result = await sut.LogOff() as RedirectToPageResult;

            Assert.Equal(result.PageName, "/Index");
        }

        [Fact]
        public async Task ConfirmEmailReturnsErrorView_WhenUserIdIsNull()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var result = await sut.ConfirmEmail(null, "sometoken") as ViewResult;
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task ConfirmEmailReturnsErrorView_WhenTokenIsNull()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var result = await sut.ConfirmEmail("someuserid", null) as ViewResult;
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task ConfirmEmailInvokesFindByIdAsyncWithCorrectUserId_WhenUserIdAndTokenAreNotNull()
        {
            const string userId = "userId";
            const string token = "someToken";
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var sut = new AccountController(userManager.Object, null, null, null, null, null);

            await sut.ConfirmEmail(userId, token);

            userManager.Verify(x => x.FindByIdAsync(It.Is<string>(y => y == userId)), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailReturnsErrorView_WhenUserIsNull_AndUserIdAndTokenAreNotNull()
        {
            const string userId = "userId";
            const string token = "someToken";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

            var sut = new AccountController(userManager.Object, null, null, null, null, null);
            var result = await sut.ConfirmEmail(userId, token) as ViewResult;

            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task ConfirmEmailInvokesConfirmEmailAsyncWithTheCorrectParameters_WhenUserAndUserIdAndTokenAreNotNull()
        {
            const string userId = "userId";
            const string token = "someToken";
            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            userManager.Setup(x => x.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);

            var sut = new AccountController(userManager.Object, null, null, null, null, null);
            await sut.ConfirmEmail(userId, token);

            userManager.Verify(x => x.ConfirmEmailAsync(It.Is<ApplicationUser>(y => y == user), It.Is<string>(y => y == token)), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailInvokesSendAsyncWithTheCorrectParameters_WhenUsersProfileIsComplete_AndUsersEmailIsConfirmed_AndUserAndUserIdAndTokenAreNotNull()
        {
            const string userId = "userId";
            const string token = "someToken";
            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var user = new ApplicationUser
            {
                Id = userId,
                FirstName = "first name",
                LastName = "last name",
                PhoneNumber = "test",
                PhoneNumberConfirmed = true,
                Email = "test@email.com",
                EmailConfirmed = true
            };
            userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            userManager.Setup(x => x.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(new RemoveUserProfileIncompleteClaimCommand { UserId = user.Id })).ReturnsAsync(It.IsAny<Unit>());

            var sut = new AccountController(userManager.Object, signInManager.Object, null, mediator.Object, null, null);
            sut.SetFakeUser(userId);
            await sut.ConfirmEmail(userId, token);

            mediator.Verify(x => x.SendAsync(It.Is<RemoveUserProfileIncompleteClaimCommand>(y => y.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailInvokesRefreshSignInAsyncWithTheCorrectParameters_WhenUserIsSignedIn_AndUsersProfileIsComplete_AndUsersEmailIsConfirmed_AndUserAndUserIdAndTokenAreNotNull()
        {
            const string userId = "userId";
            const string token = "someToken";
            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var user = new ApplicationUser
            {
                Id = userId,
                FirstName = "first name",
                LastName = "last name",
                PhoneNumber = "111-111-1111",
                PhoneNumberConfirmed = true,
                Email = "test@email.com",
                EmailConfirmed = true
            };
            userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            userManager.Setup(x => x.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(new RemoveUserProfileIncompleteClaimCommand { UserId = user.Id })).ReturnsAsync(It.IsAny<Unit>());

            var sut = new AccountController(userManager.Object, signInManager.Object, null, mediator.Object, null, null);
            sut.SetFakeUserWithCookieAuthenticationType(userId);
            signInManager.Setup(x => x.IsSignedIn(sut.User)).Returns(true);
            await sut.ConfirmEmail(userId, token);

            signInManager.Verify(x => x.RefreshSignInAsync(It.Is<ApplicationUser>(y => y.Id == user.Id)));
        }

        [Fact]
        public async Task ConfirmEmailReturnsErrorView_WhenUsersEmailCannotBeConfirmed()
        {
            const string userId = "userId";
            const string token = "someToken";
            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            userManager.Setup(x => x.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Failed());

            var sut = new AccountController(userManager.Object, null, null, null, null, null);
            var result = await sut.ConfirmEmail(userId, token) as ViewResult;

            Assert.Equal(result.ViewName, "Error");
        }

        [Fact]
        public async Task ConfirmEmailReturnsConfirmEmailView_WhenUsersEmailCanBeConfirmed()
        {
            const string userId = "userId";
            const string token = "someToken";
            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            userManager.Setup(x => x.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);

            var sut = new AccountController(userManager.Object, null, null, null, null, null);
            var result = await sut.ConfirmEmail(userId, token) as ViewResult;

            Assert.Equal(result.ViewName, "ConfirmEmail");
        }

        [Fact]
        public void ConfirmEmailHasHttpGetAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ConfirmEmailHasAllowAnonymousAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ForgotPasswordGetReturnsAView()
        {
            var sut = AccountController();
            var result = (ViewResult)sut.ForgotPassword();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ForgotPasswordGetHasHttpGetAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ForgotPassword()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ForgotPasswordGetHasAllowAnonymousAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ForgotPassword()).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task ForgotPasswordPostInvokesFindByNameAsyncWithTheCorrectEmailwhenModelStateIsValid()
        {
            const string email = "user@domain.tld";
            var vm = new ForgotPasswordViewModel { Email = email };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByNameAsync(email)).ReturnsAsync(user);

            var sut = new AccountController(userManager.Object, null, null, null, null, null);
            await sut.ForgotPassword(vm);

            userManager.Verify(m => m.FindByNameAsync(email), Times.Once);
        }

        [Fact]
        public async Task ForgotPasswordPostInvokesIsEmailConfirmedAsyncWithThecorrectUser_WhenModelStateIsValid()
        {
            const string email = "user@domain.tld";
            var vm = new ForgotPasswordViewModel { Email = email };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByNameAsync(email)).ReturnsAsync(user).Verifiable();

            var sut = new AccountController(userManager.Object, null, null, null, null, null);
            await sut.ForgotPassword(vm);

            userManager.Verify(m => m.IsEmailConfirmedAsync(user), Times.Once);
        }

        [Fact]
        public async Task ForgotPasswordPostReturnsForgotPasswordConfirmationView_WhenModelStateIsValid_AndUserIsNull()
        {
            const string email = "user@domain.tld";
            var vm = new ForgotPasswordViewModel { Email = email };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var user = default(ApplicationUser);
            userManager.Setup(x => x.FindByNameAsync(email)).ReturnsAsync(user);

            var sut = new AccountController(userManager.Object, null, null, null, null, null);
            var result = await sut.ForgotPassword(vm) as ViewResult;

            Assert.Equal(result.ViewName, "ForgotPasswordConfirmation");
        }

        [Fact]
        public async Task ForgotPasswordPostReturnsForgotPasswordConfirmationView_WhenModelStateIsValid_AndUsersEmailIsUnverified()
        {
            const string email = "user@domain.tld";
            var vm = new ForgotPasswordViewModel { Email = email };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByNameAsync(email)).ReturnsAsync(user);
            userManager.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(false);

            var sut = new AccountController(userManager.Object, null, null, null, null, null);
            var result = await sut.ForgotPassword(vm) as ViewResult;

            Assert.Equal(result.ViewName, "ForgotPasswordConfirmation");
        }

        [Fact]
        public async Task ForgotPasswordPostInvokesGeneratePasswordResetTokenAsyncWithCorrectUser_WhenModelStateIsValid_AndUserIsNotNull_AndUsersEmailHasBeenVerified()
        {
            const string email = "user@domain.tld";
            var vm = new ForgotPasswordViewModel { Email = email };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByNameAsync(email)).ReturnsAsync(user);
            userManager.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            userManager.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(It.IsAny<string>());

            var sut = new AccountController(userManager.Object, null, null, Mock.Of<IMediator>(), null, null);

            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = Mock.Of<IUrlHelper>();

            await sut.ForgotPassword(vm);

            userManager.Verify(x => x.GeneratePasswordResetTokenAsync(user), Times.Once);
        }

        [Fact]
        public async Task ForgotPasswordPostInvokesUrlActionWithCorrectParameters_WhenModelStateIsValid_AndUserIsNotNull_AndUsersEmailHasBeenVerified()
        {
            const string requestScheme = "requestScheme";
            const string email = "user@domain.tld";
            var vm = new ForgotPasswordViewModel { Email = email };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByNameAsync(email)).ReturnsAsync(user);
            userManager.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            userManager.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(It.IsAny<string>());

            var sut = new AccountController(userManager.Object, null, null, Mock.Of<IMediator>(), null, null);
            var urlHelper = new Mock<IUrlHelper>();
            sut.SetFakeHttpRequestSchemeTo(requestScheme);
            sut.Url = urlHelper.Object;

            await sut.ForgotPassword(vm);

            urlHelper.Verify(mock => mock.Action(It.Is<UrlActionContext>(uac =>
                uac.Action == "ResetPassword"
                && uac.Controller == "Account"
                && uac.Protocol == requestScheme)), Times.Once);
        }

        [Fact]
        public async Task ForgotPasswordPostSendsSendResetPasswordEmailWithCorrectParameters_WhenModelStateIsValid_AndUserIsNotNull_AndUsersEmailHasBeenVerified()
        {
            const string email = "user@domain.tld";
            const string callbackUrl = "callbackUrl";

            var vm = new ForgotPasswordViewModel { Email = email };
            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByNameAsync(email)).ReturnsAsync(user);
            userManager.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            userManager.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(It.IsAny<string>());

            var mediator = new Mock<IMediator>();

            var sut = new AccountController(userManager.Object, null, null, mediator.Object, null, null);
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(callbackUrl);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            await sut.ForgotPassword(vm);

            mediator.Verify(x => x.SendAsync(It.Is<SendResetPasswordEmail>(y => y.Email == vm.Email && y.CallbackUrl == callbackUrl)), Times.Once);
        }

        [Fact]
        public async Task ForgotPasswordPostReturnsForgotPasswordConfirmationView_WhenModelStateIsValid_AndUserIsNotNull_AndUsersEmailHasBeenVerified()
        {
            const string email = "user@domain.tld";
            var vm = new ForgotPasswordViewModel { Email = email };
            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByNameAsync(email)).ReturnsAsync(user);
            userManager.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            userManager.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(It.IsAny<string>());

            var sut = new AccountController(userManager.Object, null, null, Mock.Of<IMediator>(), null, null);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = Mock.Of<IUrlHelper>();
            var result = await sut.ForgotPassword(vm) as ViewResult;

            Assert.Equal(result.ViewName, "ForgotPasswordConfirmation");
        }

        [Fact]
        public async Task ForgotPasswordPostReturnsTheSameViewAndViewModel_WhenModelStateIsInvalid()
        {
            var vm = new ForgotPasswordViewModel();

            var sut = CreateAccountControllerWithNoInjectedDependencies();
            sut.AddModelStateError();

            var result = await sut.ForgotPassword(vm) as ViewResult;
            var modelResult = result.ViewData.Model as ForgotPasswordViewModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<ForgotPasswordViewModel>(modelResult);
            Assert.Same(modelResult, vm);
        }

        [Fact]
        public void ForgotPasswordPostHasHttpPostAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ForgotPassword(It.IsAny<ForgotPasswordViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ForgotPasswordPostHasAllowAnonymousAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ForgotPassword(It.IsAny<ForgotPasswordViewModel>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ForgotPasswordPostHasValidateAntiForgeryTokenAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ForgotPassword(It.IsAny<ForgotPasswordViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ResetPasswordGetReturnsErrorViewIfCodeIsNull()
        {
            var sut = AccountController();
            var result = (ViewResult)sut.ResetPassword();

            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public void ResetPasswordGetReturnsAViewIfCodeIsNotNull()
        {
            var sut = AccountController();
            const string code = "1234";
            var result = (ViewResult)sut.ResetPassword(code);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ResetPasswordGetHasHttpGetAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ResetPassword(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ResetPasswordGetHasAllowAnonymousAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ResetPassword(It.IsAny<string>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task ResetPasswordPostReturnsTheSameViewAndViewModel_WhenModelStateIsInvalid()
        {
            var vm = new ResetPasswordViewModel();

            var sut = CreateAccountControllerWithNoInjectedDependencies();
            sut.AddModelStateError();
            var result = await sut.ResetPassword(vm) as ViewResult;
            var modelResult = result.ViewData.Model as ResetPasswordViewModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<ResetPasswordViewModel>(modelResult);
            Assert.Same(modelResult, vm);
        }

        [Fact]
        public async Task ResetPasswordPostInvokesFindByNameAsyncWithTheCorrecEmail_WhenModelStateIsValid()
        {
            const string email = "user@domain.tld";

            var vm = new ResetPasswordViewModel { Email = email };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByNameAsync(email)).ReturnsAsync(user);
            userManager.Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var sut = new AccountController(userManager.Object, null, null, null, null, null);
            await sut.ResetPassword(vm);

            userManager.Verify(m => m.FindByNameAsync(email), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordPostInvokesResetPasswordAsyncWithCorrectParameters_WhenUserIsNotNull_AndModelStateIsValid()
        {
            const string email = "user@domain.tld";
            var vm = new ResetPasswordViewModel
            {
                Email = email,
                Password = "pass",
                Code = "code"
            };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByNameAsync(email)).ReturnsAsync(user);
            userManager.Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var sut = new AccountController(userManager.Object, null, null, null, null, null);
            await sut.ResetPassword(vm);

            userManager.Verify(m => m.ResetPasswordAsync(user, It.Is<string>(y => y == vm.Code), It.Is<string>(y => y == vm.Password)), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordPostRedirectsToCorrectAction_WhenUsersPasswordResetSucceeded_AndUserIsNotNull_AndModelStateIsValid()
        {
            const string email = "user@domain.tld";
            var vm = new ResetPasswordViewModel { Email = email };
            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByNameAsync(email)).ReturnsAsync(user);
            userManager.Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            var sut = new AccountController(userManager.Object, null, null, null, null, null);

            var result = await sut.ResetPassword(vm) as RedirectToActionResult;

            Assert.Equal("ResetPasswordConfirmation", result.ActionName);
        }

        [Fact]
        public async Task ResetPasswordPostAddsIdentityResultErrorsToModelStateErrors_WhenUsersPasswordResetFailed_AndUserIsNotNull_AndModelStateIsValid()
        {
            const string email = "user@domain.tld";

            var vm = new ResetPasswordViewModel { Email = email };
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var identityResult = IdentityResult.Failed(new IdentityError { Description = "IdentityErrorDescription" });
            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByNameAsync(email)).ReturnsAsync(user);
            userManager.Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(identityResult);

            var sut = new AccountController(userManager.Object, null, null, null, null, null);
            await sut.ResetPassword(vm);

            var errorMessages = sut.ModelState.GetErrorMessages();
            Assert.Equal(identityResult.Errors.Select(x => x.Description).Single(), errorMessages.Single());
        }

        [Fact]
        public async Task ResetPasswordPostReturnsAView_WhenUsersPasswordResetFailed_AndUserIsNotNull_AndModelStateIsValid()
        {
            const string email = "user@domain.tld";
            var vm = new ResetPasswordViewModel { Email = email };
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByNameAsync(email)).ReturnsAsync(user);
            userManager.Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var sut = new AccountController(userManager.Object, null, null, null, null, null);
            var result = await sut.ResetPassword(vm) as ViewResult;

            Assert.IsType<ViewResult>(result);
            Assert.Null(result.ViewData.Model);
        }

        [Fact]
        public void ResetPasswordPostHasHttpPostAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ResetPassword(It.IsAny<ResetPasswordViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ResetPasswordPostHasAllowAnonymousAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ResetPassword(It.IsAny<ResetPasswordViewModel>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ResetPasswordPostHasValidateAntiForgeryTokenAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ResetPassword(It.IsAny<ResetPasswordViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ResetPasswordConfirmationReturnsAView()
        {
            var sut = AccountController();
            var result = (ViewResult)sut.ResetPasswordConfirmation();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ResetPasswordConfirmationHasHttpGetAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ResetPasswordConfirmation()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ResetPasswordConfirmationHasAllowAnonymousAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ResetPasswordConfirmation()).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ExternalLoginInvokesUrlActionWithCorrectParameters()
        {
            const string provider = "provider";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var properties = new AuthenticationProperties();

            signInManager.Setup(s => s.ConfigureExternalAuthenticationProperties(provider, null, null)).Returns(properties);

            var sut = new AccountController(userManager.Object, signInManager.Object, null, Mock.Of<IMediator>(), null, null);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            sut.ExternalLogin(provider);

            urlHelper.Verify(u => u.Action(It.Is<UrlActionContext>(a => a.Action == "ExternalLoginCallback")),
                Times.Once());
        }

        [Fact]
        public void ExternalLoginInvokesConfigureExternalAuthenticationPropertiesWithCorrectParameters()
        {
            const string provider = "provider";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var properties = new AuthenticationProperties();

            signInManager.Setup(s => s.ConfigureExternalAuthenticationProperties(provider, null, null)).Returns(properties);

            var sut = new AccountController(userManager.Object, signInManager.Object, null, Mock.Of<IMediator>(), null, null);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            sut.ExternalLogin(provider);

            signInManager.VerifyAll();
        }

        [Fact]
        public void ExternalLoginReturnsChallengeResult()
        {
            const string provider = "provider";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var properties = new AuthenticationProperties();

            signInManager.Setup(s => s.ConfigureExternalAuthenticationProperties(provider, null, null)).Returns(properties);

            var sut = new AccountController(userManager.Object, signInManager.Object, null, Mock.Of<IMediator>(), null, null);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            var result = sut.ExternalLogin(provider) as ChallengeResult;
            Assert.NotNull(result);
            Assert.Contains(provider, result.AuthenticationSchemes);
            Assert.Equal(1, result.AuthenticationSchemes.Count);
            Assert.Same(properties, result.Properties);
        }

        [Fact]
        public void ExternalLoginHasPostGetAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ExternalLogin(It.IsAny<string>(), It.IsAny<string>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ExternalLoginHasAllowAnonymousAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ExternalLogin(It.IsAny<string>(), It.IsAny<string>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ExternalLoginHasValidateAntiForgeryTokenAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ExternalLogin(It.IsAny<string>(), It.IsAny<string>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task ExternalLoginCallbackInvokesGetExternalLoginInfoAsync()
        {
            const string returnUrl = "www.returnUrl";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var sut = new AccountController(userManager.Object, signInManager.Object, null, Mock.Of<IMediator>(), null, null);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            await sut.ExternalLoginCallback(returnUrl);

            signInManager.Verify(s => s.GetExternalLoginInfoAsync(null), Times.Once);
        }

        [Fact]
        public async Task ExternalLoginCallbackRedirectsToCorrectAction_WhenExternalLoginInfoIsNull()
        {
            const string returnUrl = "www.returnUrl";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var sut = new AccountController(userManager.Object, signInManager.Object, null, Mock.Of<IMediator>(), null, null);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            var result = await sut.ExternalLoginCallback(returnUrl) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
        }

        [Fact]
        public async Task ExternalLoginCallbackInvokesExternalLoginSignInAsyncWithCorrectParameters_WhenExternalLoginInfoIsNotNull()
        {
            const string returnUrl = "www.returnUrl";
            const string loginProvider = "loginProvider";
            const string providerKey = "ptoviderKey";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var loginInfo = new ExternalLoginInfo(new ClaimsPrincipal(), loginProvider, providerKey, "displayName");
            signInManager.Setup(s => s.GetExternalLoginInfoAsync(null)).ReturnsAsync(loginInfo);
            signInManager.Setup(s => s.ExternalLoginSignInAsync(loginProvider, providerKey, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
            var provideExternalUserInformation = new Mock<IProvideExternalUserInformation>();
            var userInfo = new ExternalUserInformation();
            provideExternalUserInformation.Setup(p => p.GetExternalUserInformation(loginInfo)).ReturnsAsync(userInfo);
            var externalUserInformationProviderFactory = new Mock<IExternalUserInformationProviderFactory>();
            externalUserInformationProviderFactory.Setup(e => e.GetExternalUserInformationProvider(loginProvider))
                .Returns(provideExternalUserInformation.Object);

            var sut = new AccountController(userManager.Object, signInManager.Object, null, Mock.Of<IMediator>(),
                externalUserInformationProviderFactory.Object, null);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            await sut.ExternalLoginCallback(returnUrl);

            signInManager.VerifyAll();
        }

        [Fact]
        public async Task ExternalLoginCallbackInvokesGetExternalUserInformationProviderWithTheCorrectLoginProvider_WhenExternalLoginInfoIsNotNull()
        {
            const string returnUrl = "www.returnUrl";
            const string loginProvider = "loginProvider";
            const string providerKey = "ptoviderKey";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var loginInfo = new ExternalLoginInfo(new ClaimsPrincipal(), loginProvider, providerKey, "displayName");
            signInManager.Setup(s => s.GetExternalLoginInfoAsync(null)).ReturnsAsync(loginInfo);
            signInManager.Setup(s => s.ExternalLoginSignInAsync(loginProvider, providerKey, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
            var provideExternalUserInformation = new Mock<IProvideExternalUserInformation>();
            var userInfo = new ExternalUserInformation();
            provideExternalUserInformation.Setup(p => p.GetExternalUserInformation(loginInfo)).ReturnsAsync(userInfo);
            var externalUserInformationProviderFactory = new Mock<IExternalUserInformationProviderFactory>();
            externalUserInformationProviderFactory.Setup(e => e.GetExternalUserInformationProvider(loginProvider))
                .Returns(provideExternalUserInformation.Object);

            var sut = new AccountController(userManager.Object, signInManager.Object, null, Mock.Of<IMediator>(),
                externalUserInformationProviderFactory.Object, null);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            await sut.ExternalLoginCallback(returnUrl);

            externalUserInformationProviderFactory.Verify(e => e.GetExternalUserInformationProvider(loginProvider), Times.Once);
        }

        [Fact]
        public async Task ExternalLoginCallbackInvokesGetExternalUserInformationWithTheCorrectExternalLoginInfo_WhenExternalLoginInfoIsNotNull()
        {
            const string returnUrl = "www.returnUrl";
            const string loginProvider = "loginProvider";
            const string providerKey = "ptoviderKey";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var loginInfo = new ExternalLoginInfo(new ClaimsPrincipal(), loginProvider, providerKey, "displayName");
            signInManager.Setup(s => s.GetExternalLoginInfoAsync(null)).ReturnsAsync(loginInfo);
            signInManager.Setup(s => s.ExternalLoginSignInAsync(loginProvider, providerKey, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
            var provideExternalUserInformation = new Mock<IProvideExternalUserInformation>();
            var userInfo = new ExternalUserInformation();
            provideExternalUserInformation.Setup(p => p.GetExternalUserInformation(loginInfo)).ReturnsAsync(userInfo);
            var externalUserInformationProviderFactory = new Mock<IExternalUserInformationProviderFactory>();
            externalUserInformationProviderFactory.Setup(e => e.GetExternalUserInformationProvider(loginProvider))
                .Returns(provideExternalUserInformation.Object);

            var sut = new AccountController(userManager.Object, signInManager.Object, null, Mock.Of<IMediator>(),
                externalUserInformationProviderFactory.Object, null);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            await sut.ExternalLoginCallback(returnUrl);

            provideExternalUserInformation.Verify(p => p.GetExternalUserInformation(loginInfo), Times.Once);
        }

        [Fact]
        public async Task ExternalLoginCallbackSendsApplicationUserQueryWithCorrectUsername_WhenExternalLoginSignInAsyncResultIsSuccessful_AndExternalLoginInfoIsNotNull()
        {
            const string returnUrl = "www.returnUrl";
            const string loginProvider = "loginProvider";
            const string providerKey = "ptoviderKey";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var loginInfo = new ExternalLoginInfo(new ClaimsPrincipal(), loginProvider, providerKey, "displayName");
            signInManager.Setup(s => s.GetExternalLoginInfoAsync(null)).ReturnsAsync(loginInfo);
            signInManager.Setup(s => s.ExternalLoginSignInAsync(loginProvider, providerKey, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            var provideExternalUserInformation = new Mock<IProvideExternalUserInformation>();
            var userInfo = new ExternalUserInformation { Email = "email@email.pl" };
            provideExternalUserInformation.Setup(p => p.GetExternalUserInformation(loginInfo)).ReturnsAsync(userInfo);
            var externalUserInformationProviderFactory = new Mock<IExternalUserInformationProviderFactory>();
            externalUserInformationProviderFactory.Setup(e => e.GetExternalUserInformationProvider(loginProvider))
                .Returns(provideExternalUserInformation.Object);
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser();
            mediator.Setup(m => m.SendAsync(It.Is<ApplicationUserQuery>(a => a.UserName == userInfo.Email)))
                .ReturnsAsync(user);
            var redirectAccountControllerRequests = new Mock<IRedirectAccountControllerRequests>();

            var sut = new AccountController(userManager.Object, signInManager.Object, null, mediator.Object,
                externalUserInformationProviderFactory.Object, redirectAccountControllerRequests.Object);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            await sut.ExternalLoginCallback(returnUrl);

            mediator.Verify(m => m.SendAsync(It.Is<ApplicationUserQuery>(a => a.UserName == userInfo.Email)), Times.Once);
        }

        [Fact]
        public async Task ExternalLoginCallbackInvokesRedirectToLocalWithCorrectParameters_WhenExternalLoginSignInAsyncResultIsSuccessful_AndExternalLoginInfoIsNotNull()
        {
            const string returnUrl = "www.returnUrl";
            const string loginProvider = "loginProvider";
            const string providerKey = "ptoviderKey";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var loginInfo = new ExternalLoginInfo(new ClaimsPrincipal(), loginProvider, providerKey, "displayName");
            signInManager.Setup(s => s.GetExternalLoginInfoAsync(null)).ReturnsAsync(loginInfo);
            signInManager.Setup(s => s.ExternalLoginSignInAsync(loginProvider, providerKey, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            var provideExternalUserInformation = new Mock<IProvideExternalUserInformation>();
            var userInfo = new ExternalUserInformation { Email = "email@email.pl" };
            provideExternalUserInformation.Setup(p => p.GetExternalUserInformation(loginInfo)).ReturnsAsync(userInfo);
            var externalUserInformationProviderFactory = new Mock<IExternalUserInformationProviderFactory>();
            externalUserInformationProviderFactory.Setup(e => e.GetExternalUserInformationProvider(loginProvider))
                .Returns(provideExternalUserInformation.Object);
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser();
            mediator.Setup(m => m.SendAsync(It.Is<ApplicationUserQuery>(a => a.UserName == userInfo.Email)))
                .ReturnsAsync(user);
            var redirectAccountControllerRequests = new Mock<IRedirectAccountControllerRequests>();

            var sut = new AccountController(userManager.Object, signInManager.Object, null, mediator.Object,
                externalUserInformationProviderFactory.Object, redirectAccountControllerRequests.Object);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            await sut.ExternalLoginCallback(returnUrl);

            redirectAccountControllerRequests.Verify(r => r.RedirectToLocal(returnUrl, user), Times.Once);
        }

        [Fact]
        public async Task ExternalLoginCallbackReturnsCorrectViewAndViewModel_WhenEmailIsProvidedByExternalUserInfomration_AndExternalLoginSignInAsyncResultIsUnsuccessfulAndExternalLoginInfoIsNotNull()
        {
            const string returnUrl = "www.returnUrl";
            const string loginProvider = "loginProvider";
            const string providerKey = "ptoviderKey";
            const string mail = "email@email.pl";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var loginInfo = new ExternalLoginInfo(new ClaimsPrincipal(), loginProvider, providerKey, "displayName");
            signInManager.Setup(s => s.GetExternalLoginInfoAsync(null)).ReturnsAsync(loginInfo);
            signInManager.Setup(s => s.ExternalLoginSignInAsync(loginProvider, providerKey, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
            var provideExternalUserInformation = new Mock<IProvideExternalUserInformation>();
            var userInfo = new ExternalUserInformation { Email = mail };
            provideExternalUserInformation.Setup(p => p.GetExternalUserInformation(loginInfo)).ReturnsAsync(userInfo);
            var externalUserInformationProviderFactory = new Mock<IExternalUserInformationProviderFactory>();
            externalUserInformationProviderFactory.Setup(e => e.GetExternalUserInformationProvider(loginProvider))
                .Returns(provideExternalUserInformation.Object);
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser();
            mediator.Setup(m => m.SendAsync(It.Is<ApplicationUserQuery>(a => a.UserName == userInfo.Email)))
                .ReturnsAsync(user);
            var redirectAccountControllerRequests = new Mock<IRedirectAccountControllerRequests>();

            var sut = new AccountController(userManager.Object, signInManager.Object, null, mediator.Object,
                externalUserInformationProviderFactory.Object, redirectAccountControllerRequests.Object);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            var result = await sut.ExternalLoginCallback(returnUrl) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("ExternalLoginConfirmation", result.ViewName);
            var viewModel = result.Model as ExternalLoginConfirmationViewModel;
            Assert.NotNull(viewModel);
            Assert.Equal(true, viewModel.EmailIsVerifiedByExternalLoginProvider);
            Assert.Equal(mail, viewModel.Email);
            Assert.Equal(returnUrl, viewModel.ReturnUrl);
            Assert.Equal(loginProvider, viewModel.LoginProvider);
        }

        [Fact]
        public async Task ExternalLoginCallbackReturnsCorrectViewAndViewModel_WhenEmailIsNotProvidedByExternalUserInfomration_AndExternalLoginSignInAsyncResultIsUnsuccessfulAndExternalLoginInfoIsNotNull()
        {
            const string returnUrl = "www.returnUrl";
            const string loginProvider = "loginProvider";
            const string providerKey = "ptoviderKey";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var loginInfo = new ExternalLoginInfo(new ClaimsPrincipal(), loginProvider, providerKey, "displayName");
            signInManager.Setup(s => s.GetExternalLoginInfoAsync(null)).ReturnsAsync(loginInfo);
            signInManager.Setup(s => s.ExternalLoginSignInAsync(loginProvider, providerKey, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
            var provideExternalUserInformation = new Mock<IProvideExternalUserInformation>();
            var userInfo = new ExternalUserInformation();
            provideExternalUserInformation.Setup(p => p.GetExternalUserInformation(loginInfo)).ReturnsAsync(userInfo);
            var externalUserInformationProviderFactory = new Mock<IExternalUserInformationProviderFactory>();
            externalUserInformationProviderFactory.Setup(e => e.GetExternalUserInformationProvider(loginProvider))
                .Returns(provideExternalUserInformation.Object);
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser();
            mediator.Setup(m => m.SendAsync(It.Is<ApplicationUserQuery>(a => a.UserName == userInfo.Email)))
                .ReturnsAsync(user);
            var redirectAccountControllerRequests = new Mock<IRedirectAccountControllerRequests>();

            var sut = new AccountController(userManager.Object, signInManager.Object, null, mediator.Object,
                externalUserInformationProviderFactory.Object, redirectAccountControllerRequests.Object);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            var result = await sut.ExternalLoginCallback(returnUrl) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("ExternalLoginConfirmation", result.ViewName);
            var viewModel = result.Model as ExternalLoginConfirmationViewModel;
            Assert.NotNull(viewModel);
            Assert.Equal(false, viewModel.EmailIsVerifiedByExternalLoginProvider);
            Assert.Equal(null, viewModel.Email);
            Assert.Equal(returnUrl, viewModel.ReturnUrl);
            Assert.Equal(loginProvider, viewModel.LoginProvider);
        }

        [Fact]
        public void ExternalLoginCallbackHasHttpGetAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ExternalLoginCallback(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ExternalLoginCallbackHasAllowAnonymousAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ExternalLoginCallback(It.IsAny<string>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task ExternalLoginConfirmationInvokesIsSignedInWithCorrectUser()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();

            var sut = new AccountController(userManager.Object, signInManager.Object, null, mediator.Object, null, null);
            signInManager.Setup(x => x.IsSignedIn(sut.User)).Returns(true);

            await sut.ExternalLoginConfirmation(new ExternalLoginConfirmationViewModel());

            signInManager.VerifyAll();
        }

        [Fact]
        public async Task ExternalLoginConfirmationRedirectsToCorrectActionIfUserIsSignedIn()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();

            var sut = new AccountController(userManager.Object, signInManager.Object, null, mediator.Object, null, null);
            signInManager.Setup(x => x.IsSignedIn(sut.User)).Returns(true);

            var result = await sut.ExternalLoginConfirmation(new ExternalLoginConfirmationViewModel()) as RedirectToActionResult;

            Assert.Equal<string>(result.ControllerName, "Manage");
            Assert.Equal<string>(result.ActionName, nameof(ManageController.Index));
        }

        [Fact]
        public async Task ExternalLoginConfirmationInvokesGetExternalLoginInfoAsync_WhenModelStateIsValid()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            signInManager.Setup(s => s.GetExternalLoginInfoAsync(It.Is<string>(xsrf => xsrf == null))).ReturnsAsync(default(ExternalLoginInfo));
            var viewmodel = CreateExternalLoginConfirmationViewModel();

            var sut = new AccountController(userManager.Object, signInManager.Object, null, null, null, null);
            sut.SetFakeUser("userId");
            await sut.ExternalLoginConfirmation(viewmodel);

            signInManager.Verify(s => s.GetExternalLoginInfoAsync(It.Is<string>(xsrf => xsrf == null)), Times.Once());
        }

        [Fact]
        public async Task ExternalLoginConfirmationReturnsExternalLoginFailureView_WhenUserIsNull_AndModelStateIsValid()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            signInManager.Setup(s => s.GetExternalLoginInfoAsync(It.Is<string>(xsrf => xsrf == null))).ReturnsAsync(default(ExternalLoginInfo));
            var viewmodel = CreateExternalLoginConfirmationViewModel();

            var sut = new AccountController(userManager.Object, signInManager.Object, null, null, null, null);
            sut.SetFakeUser("userId");
            var result = await sut.ExternalLoginConfirmation(viewmodel) as ViewResult;

            Assert.Equal(result.ViewName, "ExternalLoginFailure");
        }

        [Fact]
        public async Task ExternalLoginConfirmationInvokesCreateAsyncWithCorrectUser_WhenExternalLoginInfoIsSuccessful_AndModelStateIsValid()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new IdentityResult());
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            SetupSignInManagerWithTestExternalLoginValue(signInManager);
            var viewModel = CreateExternalLoginConfirmationViewModel();

            var sut = new AccountController(userManager.Object, signInManager.Object, CreateGeneralSettingsMockObject().Object, null, null, null);
            sut.SetFakeUser("userId");

            await sut.ExternalLoginConfirmation(viewModel);

            userManager.Verify(u => u.CreateAsync(It.Is<ApplicationUser>(au => au.Email == viewModel.Email && au.FirstName == viewModel.FirstName && au.LastName == viewModel.LastName && au.PhoneNumber == viewModel.PhoneNumber)));
        }

        [Fact]
        public async Task ExternalLoginConfirmationInvokesAddLoginAsyncWithCorrectParameters_WhenUserIsCreatedSuccessfully_AndExternalLoginInfoIsSuccessful_AndModelStateIsValid()
        {
            const string loginProvider = "test";
            const string providerKey = "test";
            const string displayName = "testDisplayName";

            var userManager = CreateUserManagerMockWithSucessIdentityResult();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            SetupSignInManagerWithTestExternalLoginValue(signInManager, loginProvider, providerKey, displayName);
            var urlHelperMock = CreateUrlHelperMockObject();
            SetupUrlHelperMockToReturnTrueForLocalUrl(urlHelperMock);
            var viewModel = CreateExternalLoginConfirmationViewModel();

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = "DefaultTimeZone" });

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, Mock.Of<IRedirectAccountControllerRequests>());
            sut.SetFakeUser("userId");
            sut.Url = urlHelperMock.Object;
            await sut.ExternalLoginConfirmation(viewModel, "testUrl");

            userManager.Verify(u => u.AddLoginAsync(It.Is<ApplicationUser>(au => au.Email == viewModel.Email
                && au.FirstName == viewModel.FirstName
                && au.LastName == viewModel.LastName
                && au.PhoneNumber == viewModel.PhoneNumber),
            It.Is<ExternalLoginInfo>(ei => ei.LoginProvider == loginProvider
                && ei.ProviderKey == providerKey
                && ei.ProviderDisplayName == displayName)));
        }

        [Fact]
        public async Task ExternalLoginConfirmationInvokesGenerateEmailConfirmationTokenAsyncWithCorrectApplicationUser_WhenUserIsSignedIn_AndModelStateIsValid_AndExternalLoginInfoIsRetreived_AndUserCreationIsSuccessful_AndExternalLoginInfoIsAddedToUser()
        {
            const string loginProvider = "test";
            const string providerKey = "test";
            const string displayName = "testDisplayName";
            const string timeZone = "DefaultTimeZone";

            var userManager = CreateUserManagerMockWithSucessIdentityResult();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            SetupSignInManagerWithTestExternalLoginValue(signInManager, loginProvider, providerKey, displayName);
            var urlHelperMock = CreateUrlHelperMockObject();
            SetupUrlHelperMockToReturnTrueForLocalUrl(urlHelperMock);
            var viewModel = CreateExternalLoginConfirmationViewModel();

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = timeZone });

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, Mock.Of<IRedirectAccountControllerRequests>());
            sut.SetFakeUser("userId");
            sut.Url = urlHelperMock.Object;
            await sut.ExternalLoginConfirmation(viewModel, "testUrl");

            userManager.Verify(
                u =>
                    u.GenerateEmailConfirmationTokenAsync(
                        It.Is<ApplicationUser>(
                            user =>
                                user.UserName == viewModel.Email && user.Email == viewModel.Email &&
                                user.TimeZoneId == timeZone &&
                                user.FirstName == viewModel.FirstName && user.LastName == viewModel.LastName &&
                                user.PhoneNumber == viewModel.PhoneNumber)));
        }

        [Fact]
        public async Task ExternalLoginConfirmationInvokesUrlAction_WithTheCorrectParameters_WhenUserIsSignedIn_AndModelStateIsValid_AndExternalLoginInfoIsRetreived_AndUserCreationIsSuccessful_AndExternalLoginInfoIsAddedToUser()
        {
            const string loginProvider = "test";
            const string providerKey = "test";
            const string displayName = "testDisplayName";
            const string timeZone = "DefaultTimeZone";
            const string emailConfirmationToken = "ect";

            var userManager = CreateUserManagerMockWithSucessIdentityResult();
            userManager.Setup(u => u.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(emailConfirmationToken);
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            SetupSignInManagerWithTestExternalLoginValue(signInManager, loginProvider, providerKey, displayName);
            var urlHelperMock = CreateUrlHelperMockObject();
            SetupUrlHelperMockToReturnTrueForLocalUrl(urlHelperMock);
            var viewModel = CreateExternalLoginConfirmationViewModel();

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = timeZone });

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, Mock.Of<IRedirectAccountControllerRequests>());
            sut.SetFakeUser("userId");
            sut.Url = urlHelperMock.Object;
            await sut.ExternalLoginConfirmation(viewModel, "testUrl");

            urlHelperMock.Verify(u => u.Action(It.Is<UrlActionContext>(a => a.Action == "ConfirmEmail" && a.Controller == "Account")));
        }

        [Fact]
        public async Task ExternalLoginConfirmationInvokesConfirmEmailAsync_WhenEmailIsVerifiedByExternalLoginProvider_WithTheCorrectParameters_WhenUserIsSignedIn_AndModelStateIsValid_AndExternalLoginInfoIsRetreived_AndUserCreationIsSuccessful_AndExternalLoginInfoIsAddedToUser()
        {
            const string loginProvider = "test";
            const string providerKey = "test";
            const string displayName = "testDisplayName";
            const string timeZone = "DefaultTimeZone";
            const string emailConfirmationToken = "ect";

            var userManager = CreateUserManagerMockWithSucessIdentityResult();
            userManager.Setup(u => u.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(emailConfirmationToken);
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            SetupSignInManagerWithTestExternalLoginValue(signInManager, loginProvider, providerKey, displayName);
            var urlHelperMock = CreateUrlHelperMockObject();
            SetupUrlHelperMockToReturnTrueForLocalUrl(urlHelperMock);
            var viewModel = CreateExternalLoginConfirmationViewModel();
            viewModel.EmailIsVerifiedByExternalLoginProvider = true;

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = timeZone });

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, Mock.Of<IRedirectAccountControllerRequests>());
            sut.SetFakeUser("userId");
            sut.Url = urlHelperMock.Object;
            await sut.ExternalLoginConfirmation(viewModel, "testUrl");

            userManager.Verify(u => u.ConfirmEmailAsync(It.Is<ApplicationUser>(user =>
                user.UserName == viewModel.Email && user.Email == viewModel.Email &&
                user.TimeZoneId == timeZone &&
                user.FirstName == viewModel.FirstName && user.LastName == viewModel.LastName &&
                user.PhoneNumber == viewModel.PhoneNumber), emailConfirmationToken));
        }

        [Fact]
        public async Task ExternalLoginConfirmationSendsSendConfirmAccountEmailWithCorrectParameters_WhenUserIsSignedIn_AndModelStateIsValid_AndExternalLoginInfoIsRetreived_AndUserCreationIsSuccessful_AndExternalLoginInfoIsAddedToUser()
        {
            const string loginProvider = "test";
            const string providerKey = "test";
            const string displayName = "testDisplayName";
            const string timeZone = "DefaultTimeZone";
            const string emailConfirmationToken = "ect";
            const string callback = "www.callback";

            var mediator = new Mock<IMediator>();
            var userManager = CreateUserManagerMockWithSucessIdentityResult();
            userManager.Setup(u => u.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(emailConfirmationToken);
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            SetupSignInManagerWithTestExternalLoginValue(signInManager, loginProvider, providerKey, displayName);
            var urlHelperMock = CreateUrlHelperMockObject();
            urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns(callback);
            var viewModel = CreateExternalLoginConfirmationViewModel();

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = timeZone });

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, mediator.Object, null, Mock.Of<IRedirectAccountControllerRequests>());
            sut.SetFakeUser("userId");
            sut.Url = urlHelperMock.Object;
            await sut.ExternalLoginConfirmation(viewModel, "testUrl");

            mediator.Setup(
                m =>
                    m.SendAsync(
                        It.Is<SendConfirmAccountEmail>(s => s.Email == viewModel.Email && s.CallbackUrl == callback)));
        }

        [Fact]
        public async Task ExternalLoginConfirmationInvokesGenerateChangePhoneNumberTokenAsyncWithTheCorrectParameters_WhenUserIsSignedIn_AndModelStateIsValid_AndExternalLoginInfoIsRetreived_AndUserCreationIsSuccessful_AndExternalLoginInfoIsAddedToUser()
        {
            const string loginProvider = "test";
            const string providerKey = "test";
            const string displayName = "testDisplayName";
            const string timeZone = "DefaultTimeZone";
            const string emailConfirmationToken = "ect";
            const string callback = "www.callback";

            var mediator = new Mock<IMediator>();
            var userManager = CreateUserManagerMockWithSucessIdentityResult();
            userManager.Setup(u => u.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(emailConfirmationToken);
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            SetupSignInManagerWithTestExternalLoginValue(signInManager, loginProvider, providerKey, displayName);
            var urlHelperMock = CreateUrlHelperMockObject();
            urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns(callback);
            var viewModel = CreateExternalLoginConfirmationViewModel();

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = timeZone });

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, mediator.Object, null, Mock.Of<IRedirectAccountControllerRequests>());
            sut.SetFakeUser("userId");
            sut.Url = urlHelperMock.Object;
            await sut.ExternalLoginConfirmation(viewModel, "testUrl");

            userManager.Setup(u => u.GenerateChangePhoneNumberTokenAsync(It.Is<ApplicationUser>(user =>
                user.UserName == viewModel.Email && user.Email == viewModel.Email &&
                user.TimeZoneId == timeZone &&
                user.FirstName == viewModel.FirstName && user.LastName == viewModel.LastName &&
                user.PhoneNumber == viewModel.PhoneNumber), viewModel.PhoneNumber));
        }

        [Fact]
        public async Task ExternalLoginConfirmationSendsSendAccountSecurityTokenSmsWithCorrectParameters_WhenUserIsSignedIn_AndModelStateIsValid_AndExternalLoginInfoIsRetreived_AndUserCreationIsSuccessful_AndExternalLoginInfoIsAddedToUser()
        {
            const string loginProvider = "test";
            const string providerKey = "test";
            const string displayName = "testDisplayName";
            const string timeZone = "DefaultTimeZone";
            const string emailConfirmationToken = "ect";
            const string callback = "www.callback";
            const string changePhoneNumberToken = "cpnt";

            var mediator = new Mock<IMediator>();
            var userManager = CreateUserManagerMockWithSucessIdentityResult();
            userManager.Setup(u => u.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(emailConfirmationToken);
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            SetupSignInManagerWithTestExternalLoginValue(signInManager, loginProvider, providerKey, displayName);
            var urlHelperMock = CreateUrlHelperMockObject();
            urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns(callback);
            var viewModel = CreateExternalLoginConfirmationViewModel();
            userManager.Setup(
                u => u.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(), viewModel.PhoneNumber))
                .ReturnsAsync(changePhoneNumberToken);

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = timeZone });

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, mediator.Object, null, Mock.Of<IRedirectAccountControllerRequests>());
            sut.SetFakeUser("userId");
            sut.Url = urlHelperMock.Object;
            await sut.ExternalLoginConfirmation(viewModel, "testUrl");

            mediator.Setup(
                m =>
                    m.SendAsync(
                        It.Is<SendAccountSecurityTokenSms>(
                            s => s.PhoneNumber == viewModel.PhoneNumber && s.Token == changePhoneNumberToken)));
        }

        [Fact]
        public async Task ExternalLoginConfirmationInvokesSignInAsyncWithCorrectParameters_WhenUserIsSignedIn_AndModelStateIsValid_AndExternalLoginInfoIsRetreived_AndUserCreationIsSuccessful_AndExternalLoginInfoIsAddedToUser()
        {
            const string loginProvider = "test";
            const string providerKey = "test";
            const string displayName = "testDisplayName";
            const string timeZone = "DefaultTimeZone";
            const string emailConfirmationToken = "ect";
            const string callback = "www.callback";
            const string changePhoneNumberToken = "cpnt";

            var mediator = new Mock<IMediator>();
            var userManager = CreateUserManagerMockWithSucessIdentityResult();
            userManager.Setup(u => u.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(emailConfirmationToken);
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            SetupSignInManagerWithTestExternalLoginValue(signInManager, loginProvider, providerKey, displayName);
            var urlHelperMock = CreateUrlHelperMockObject();
            urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns(callback);
            var viewModel = CreateExternalLoginConfirmationViewModel();
            userManager.Setup(
                u => u.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(), viewModel.PhoneNumber))
                .ReturnsAsync(changePhoneNumberToken);

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = timeZone });

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, mediator.Object, null, Mock.Of<IRedirectAccountControllerRequests>());
            sut.SetFakeUser("userId");
            sut.Url = urlHelperMock.Object;
            await sut.ExternalLoginConfirmation(viewModel, "testUrl");

            signInManager.Verify(s => s.SignInAsync(
                It.Is<ApplicationUser>(au => au.Email == viewModel.Email
                && au.FirstName == viewModel.FirstName
                && au.LastName == viewModel.LastName
                && au.PhoneNumber == viewModel.PhoneNumber),
                false, null));
        }

        [Fact]
        public async Task ExternalLoginConfirmationInvokesSignInAsyncWithCorrectParameters_WhenExternalLoginIsAddedSuccessfully()
        {
            var userManager = CreateUserManagerMockWithSucessIdentityResult();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            SetupSignInManagerWithTestExternalLoginValue(signInManager, "test", "testKey", "testDisplayName");
            var urlHelperMock = CreateUrlHelperMockObject();
            SetupUrlHelperMockToReturnTrueForLocalUrl(urlHelperMock);
            var viewModel = CreateExternalLoginConfirmationViewModel();

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = "DefaultTimeZone" });

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, Mock.Of<IRedirectAccountControllerRequests>());
            sut.SetFakeUser("userId");
            sut.Url = urlHelperMock.Object;

            await sut.ExternalLoginConfirmation(viewModel, "testUrl");

            signInManager.Verify(s => s.SignInAsync(It.Is<ApplicationUser>(au => au.Email == viewModel.Email
                && au.FirstName == viewModel.FirstName
                && au.LastName == viewModel.LastName
                && au.PhoneNumber == viewModel.PhoneNumber),
                It.Is<bool>(p => p == false),
                It.Is<string>(auth => auth == null)));
        }

        [Fact]
        public async Task ExternalLoginConfirmationInvokesRedirectToLocalWithCorrectParameters_WhenExternalLoginIsAddedSuccessfully()
        {
            var userManager = CreateUserManagerMockWithSucessIdentityResult();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            SetupSignInManagerWithTestExternalLoginValue(signInManager, "test", "testKey", "testDisplayName");
            SetupSignInManagerWithTestExternalLoginValue(signInManager);
            var urlHelperMock = CreateUrlHelperMockObject();
            SetupUrlHelperMockToReturnTrueForLocalUrl(urlHelperMock);
            var viewModel = CreateExternalLoginConfirmationViewModel();
            var redirectAccountControllerRequests = new Mock<IRedirectAccountControllerRequests>();

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = "DefaultTimeZone" });

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, redirectAccountControllerRequests.Object);
            sut.SetFakeUser("userId");
            sut.Url = urlHelperMock.Object;

            await sut.ExternalLoginConfirmation(viewModel, "testUrl");

            redirectAccountControllerRequests.Verify(r => r.RedirectToLocal("testUrl",
                It.Is<ApplicationUser>(au => au.Email == viewModel.Email
                && au.FirstName == viewModel.FirstName
                && au.LastName == viewModel.LastName
                && au.PhoneNumber == viewModel.PhoneNumber)));
        }

        [Fact]
        public async Task ExternalLoginConfirmationAddsIdentityResultErrorsToModelStateError_WhenUserIsCreatedSuccessfully()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(u => u.AddLoginAsync(It.IsAny<ApplicationUser>(), It.IsAny<ExternalLoginInfo>()))
                .ReturnsAsync(IdentityResult.Failed(
                    new IdentityError { Code = "TestCode1", Description = "TestDescription1" },
                    new IdentityError { Code = "TestCode2", Description = "TestDescription2" }
                ));

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            SetupSignInManagerWithTestExternalLoginValue(signInManager, "test", "testKey", "testDisplayName");
            var urlHelperMock = CreateUrlHelperMockObject();
            SetupUrlHelperMockToReturnResultBaseOnLineBegining(urlHelperMock);
            var viewModel = CreateExternalLoginConfirmationViewModel();
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = "DefaultTimeZone" });

            var sut = new AccountController(userManager.Object, signInManager.Object, generalSettings.Object, Mock.Of<IMediator>(), null, null);
            sut.SetFakeUser("userId");
            sut.Url = urlHelperMock.Object;
            var result = await sut.ExternalLoginConfirmation(viewModel, "http://localUrl") as ViewResult;

            Assert.Equal(result.ViewData.ModelState.ErrorCount, 2);
            var firstModelStateError = result.ViewData.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault();
            var secondModelStateError = result.ViewData.ModelState.Values.FirstOrDefault().Errors.Skip(1).FirstOrDefault();
            Assert.Equal(firstModelStateError.ErrorMessage, "TestDescription1");
            Assert.Equal(secondModelStateError.ErrorMessage, "TestDescription2");
        }

        [Fact]
        public async Task ExternalLoginConfirmationPutsCorrectDataInViewDataWithCorrectKey_WhenModelStateIsInvalid()
        {
            const string returnUrlKey = "ReturnUrl";
            const string returnUrlValue = "http:\\test.url.com";
            var model = new ExternalLoginConfirmationViewModel();

            var controller = AccountController();
            controller.AddModelStateError();
            controller.SetFakeUser("test");

            var result = await controller.ExternalLoginConfirmation(model, returnUrlValue) as ViewResult;
            var viewDataKey = result.ViewData.Keys.FirstOrDefault(k => k == returnUrlKey);
            var viewDataValue = result.ViewData[returnUrlKey] as string;
            Assert.Equal<string>(viewDataValue, returnUrlValue);
            Assert.NotNull(viewDataKey);
        }

        [Fact]
        public async Task ExternalLoginConfirmationReturnsCorrectViewModel_WhenModelStateIsInvalid()
        {
            var model = new ExternalLoginConfirmationViewModel();

            var controller = AccountController();
            controller.AddModelStateError();
            controller.SetFakeUser("test");
            var result = await controller.ExternalLoginConfirmation(model) as ViewResult;
            var modelResult = result.ViewData.Model as ExternalLoginConfirmationViewModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<ExternalLoginConfirmationViewModel>(modelResult);
            Assert.Same(modelResult, model);
        }

        [Fact]
        public void ExternalLoginConfirmationHasHttpPostAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ExternalLoginConfirmation(It.IsAny<ExternalLoginConfirmationViewModel>(), It.IsAny<string>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ExternalLoginConfirmationHasAllowAnonymousAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ExternalLoginConfirmation(It.IsAny<ExternalLoginConfirmationViewModel>(), It.IsAny<string>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ExternalLoginConfirmationHasValidateAntiForgeryTokenAttribute()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ExternalLoginConfirmation(It.IsAny<ExternalLoginConfirmationViewModel>(), It.IsAny<string>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        private static AccountController AccountController(Microsoft.AspNetCore.Identity.SignInResult signInResult = default(Microsoft.AspNetCore.Identity.SignInResult))
        {
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(mock => mock.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(signInResult == default(Microsoft.AspNetCore.Identity.SignInResult) ? Microsoft.AspNetCore.Identity.SignInResult.Success : signInResult);

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(mock => mock.IsLocalUrl(It.Is<string>(x => x.StartsWith("http")))).Returns(false);
            urlHelperMock.Setup(mock => mock.IsLocalUrl(It.Is<string>(x => !x.StartsWith("http")))).Returns(true);
            var controller = new AccountController(userManagerMock.Object, signInManagerMock.Object, Mock.Of<IOptions<GeneralSettings>>(), Mock.Of<IMediator>(), null, null)
            {
                Url = urlHelperMock.Object,
            };

            return controller;
        }

        private static AccountController CreateAccountControllerWithNoInjectedDependencies() => new AccountController(null, null, null, null, null, null);

        private static Mock<UserManager<ApplicationUser>> CreateUserManagerMockWithSucessIdentityResult()
        {
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(u => u.AddLoginAsync(It.IsAny<ApplicationUser>(), It.IsAny<ExternalLoginInfo>())).ReturnsAsync(IdentityResult.Success);

            return userManagerMock;
        }

        private static void SetupSignInManagerWithTestExternalLoginValue(Mock<SignInManager<ApplicationUser>> signInManager, string loginProvider = "test", string providerKey = "test",
            string displayName = "test")
        {
            signInManager.Setup(s => s.GetExternalLoginInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(new ExternalLoginInfo(new ClaimsPrincipal(), loginProvider, providerKey, displayName));
        }

        private static ExternalLoginConfirmationViewModel CreateExternalLoginConfirmationViewModel(string email = "test@test.com", string firstName = "FirstName", string lastName = "LastName", string phoneNumber = "(111)111-11-11")
        {
            var result = new ExternalLoginConfirmationViewModel
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber
            };

            return result;
        }

        private static Mock<IOptions<GeneralSettings>> CreateGeneralSettingsMockObject()
        {
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            return generalSettings;
        }

        private static Mock<IUrlHelper> CreateUrlHelperMockObject()
        {
            var urlHelperMock = new Mock<IUrlHelper>();
            return urlHelperMock;
        }

        private static void SetupUrlHelperMockToReturnTrueForLocalUrl(Mock<IUrlHelper> urlHelperMock)
        {
            urlHelperMock
                .Setup(mock => mock.IsLocalUrl(It.IsAny<string>()))
                .Returns(true);
        }

        private static void SetupUrlHelperMockToReturnResultBaseOnLineBegining(Mock<IUrlHelper> urlHelperMock, string urlBegining = "http")
        {
            urlHelperMock.Setup(mock => mock.IsLocalUrl(It.Is<string>(x => x.StartsWith(urlBegining)))).Returns(false);
            urlHelperMock.Setup(mock => mock.IsLocalUrl(It.Is<string>(x => !x.StartsWith(urlBegining)))).Returns(true);
        }
    }
}
