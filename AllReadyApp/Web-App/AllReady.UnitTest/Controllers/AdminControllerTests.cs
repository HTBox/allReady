using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Configuration;
using AllReady.Constants;
using AllReady.Controllers;
using AllReady.Extensions;
using AllReady.Features.Admin;
using AllReady.Features.Manage;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels.Account;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AllReady.UnitTest.Controllers
{
    public class AdminControllerTests
    {
        [Fact]
        public void RegisterReturnsViewResult()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var result = sut.Register();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void RegisterHasHttpGetAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Register()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void RegisterHasAllowAnonymousAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.Register()).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task RegisterReturnsViewResultWhenModelStateIsNotValid()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            sut.AddModelStateError();

            var result = await sut.Register(It.IsAny<RegisterViewModel>());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task RegisterReturnsCorrectModelWhenModelStateIsNotValid()
        {
            var model = new RegisterViewModel();

            var sut = CreateAdminControllerWithNoInjectedDependencies();
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

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var sut = new AdminController(userManager.Object, null, null, null, generalSettings.Object);

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

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var sut = new AdminController(userManager.Object, null, Mock.Of<IMediator>(), null, generalSettings.Object);
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

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(It.IsAny<string>());

            var sut = new AdminController(userManager.Object, null, Mock.Of<IMediator>(), null, generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(requestScheme);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            await sut.Register(new RegisterViewModel());

            //note: I can't test the Values part here b/c I do not have control over the Id generation (which is a guid represented as as string) on ApplicationUser b/c it's new'ed up in the controller
            urlHelper.Verify(mock => mock.Action(It.Is<UrlActionContext>(uac =>
                uac.Action == "ConfirmEmail" &&
                uac.Controller == ControllerNames.Admin &&
                uac.Protocol == requestScheme)),
                Times.Once);
        }

        [Fact]
        public async Task RegisterSendsSendConfirmAccountEmailWithCorrectDataWhenUserCreationIsSuccessful()
        {
            const string callbackUrl = "callbackUrl";

            var model = new RegisterViewModel { Email = "email" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(It.IsAny<string>());

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(callbackUrl);

            var mediator = new Mock<IMediator>();

            var sut = new AdminController(userManager.Object, null, mediator.Object, null, generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            await sut.Register(model);

            mediator.Verify(x => x.SendAsync(It.Is<SendConfirmAccountEmail>(y => y.Email == model.Email && y.CallbackUrl == callbackUrl)));
        }

        [Fact]
        public async Task RegisterRedirectsToCorrectActionWhenUserCreationIsSuccessful()
        {
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync((IdentityResult.Success));
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(It.IsAny<string>());

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new AdminController(userManager.Object, null, Mock.Of<IMediator>(), null, generalSettings.Object);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            var result = await sut.Register(new RegisterViewModel()) as RedirectToActionResult;

            Assert.Equal(nameof(AdminController.DisplayEmail), result.ActionName);
            Assert.Equal(ControllerNames.Admin, result.ControllerName);
        }

        [Fact]
        public async Task RegisterAddsIdentityResultErrorsToModelStateErrorsWhenUserCreationIsNotSuccessful()
        {
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "IdentityErrorDescription" });

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(identityResult);

            var sut = new AdminController(userManager.Object, null, null, null, generalSettings.Object);
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

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var sut = new AdminController(userManager.Object, null, null, null, generalSettings.Object);
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
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var result = sut.DisplayEmail();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void DisplayEmailHasHttpGetAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.DisplayEmail()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DisplayEmailHasAllowAnonymousAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.DisplayEmail()).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task ConfirmEmailReturnsErrorWhenCodeIsNull()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var result = await sut.ConfirmEmail(null, null) as ViewResult;
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task ConfirmEmailReturnsErrorWhenCannotFindUserByUserId()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var sut = new AdminController(userManager.Object, null, null, null, null);
            var result = await sut.ConfirmEmail(null, "code") as ViewResult;
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task ConfirmEmailInvokesFindByIdAsyncWithCorrectUserId()
        {
            const string userId = "userId";
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var sut = new AdminController(userManager.Object, null, null, null, null);
            await sut.ConfirmEmail(userId, "code");

            userManager.Verify(x => x.FindByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailInvokesConfirmEmailAsyncWithCorrectUserAndCode()
        {
            const string code = "code";
            var user = new ApplicationUser();

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var sut = new AdminController(userManager.Object, null, null, null, null);
            await sut.ConfirmEmail(null, code);

            userManager.Verify(x => x.ConfirmEmailAsync(user, code), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailInvokesUrlActionWithCorrectParametersWhenUsersEmailIsConfirmedSuccessfully()
        {
            const string requestScheme = "requestScheme";
            const string userId = "1";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser { Id = userId });
            userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var settings = new Mock<IOptions<SampleDataSettings>>();
            settings.Setup(x => x.Value).Returns(new SampleDataSettings());

            var urlHelper = new Mock<IUrlHelper>();

            var sut = new AdminController(userManager.Object, null, Mock.Of<IMediator>(), settings.Object, null);
            sut.SetFakeHttpRequestSchemeTo(requestScheme);
            sut.Url = urlHelper.Object;

            await sut.ConfirmEmail(It.IsAny<string>(), "code");

            urlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(uac =>
                uac.Action == "EditUser" &&
                uac.Controller == "Site" &&
                uac.Protocol == requestScheme &&
                uac.Values.ToString() == $"{{ area = Admin, userId = {userId} }}")),
                Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailSendsSendApproveOrganizationUserAccountEmailWithCorrectDataWhenUsersEmailIsConfirmedSuccessfully()
        {
            const string defaultAdminUserName = "requestScheme";
            const string callbackUrl = "callbackUrl";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
            userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var settings = new Mock<IOptions<SampleDataSettings>>();
            settings.Setup(x => x.Value).Returns(new SampleDataSettings { DefaultAdminUsername = defaultAdminUserName });

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(callbackUrl);

            var mediator = new Mock<IMediator>();

            var sut = new AdminController(userManager.Object, null, mediator.Object, settings.Object, null);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            await sut.ConfirmEmail(It.IsAny<string>(), "code");

            mediator.Verify(x => x.SendAsync(It.Is<SendApproveOrganizationUserAccountEmail>(y => y.DefaultAdminUsername == defaultAdminUserName && y.CallbackUrl == callbackUrl)));
        }

        [Fact]
        public async Task ConfirmEmailReturnsCorrectViewWhenUsersConfirmationIsSuccessful()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
            userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var settings = new Mock<IOptions<SampleDataSettings>>();
            settings.Setup(x => x.Value).Returns(new SampleDataSettings { DefaultAdminUsername = It.IsAny<string>() });

            var sut = new AdminController(userManager.Object, null, Mock.Of<IMediator>(), settings.Object, null);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            var result = await sut.ConfirmEmail("userId", "code") as ViewResult;

            Assert.Equal("ConfirmEmail", result.ViewName);
        }

        [Fact]
        public async Task ConfirmEmailReturnsCorrectViewWhenUsersConfirmationIsUnsuccessful()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
            userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var sut = new AdminController(userManager.Object, null, null, null, null);
            var result = await sut.ConfirmEmail("userId", "code") as ViewResult;

            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public void ConfirmEmailHasHttpGetAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ConfirmEmailHasAllowAnonymousAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task SendCodeGetInvokesGetTwoFactorAuthenticationUserAsync()
        {
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock();
            var sut = new AdminController(null, signInManager.Object, null, null, null);
            await sut.SendCode(It.IsAny<string>(), It.IsAny<bool>());

            signInManager.Verify(x => x.GetTwoFactorAuthenticationUserAsync(), Times.Once);
        }

        [Fact]
        public async Task SendCodeGetReturnsErrorViewWhenCannotFindUser()
        {
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock();
            var sut = new AdminController(null, signInManager.Object, null, null, null);
            var result = await sut.SendCode(null, It.IsAny<bool>()) as ViewResult;

            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task SendCodeGetInvokesGetValidTwoFactorProvidersAsyncWithCorrectUser()
        {
            var applicationUser = new ApplicationUser();

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            signInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(applicationUser);
            userManager.Setup(x => x.GetValidTwoFactorProvidersAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>());

            var sut = new AdminController(userManager.Object, signInManager.Object, null, null, null);

            await sut.SendCode(null, It.IsAny<bool>());

            userManager.Verify(x => x.GetValidTwoFactorProvidersAsync(applicationUser), Times.Once);
        }

        [Fact]
        public async Task SendCodeGetReturnsSendCodeViewModelWithCorrectData()
        {
            const string returnUrl = "returnUrl";
            const bool rememberMe = true;

            var userFactors = new List<string> { "userFactor1", "userFactor2" };
            var expectedProviders = userFactors.Select(factor => new SelectListItem { Text = factor, Value = factor }).ToList();

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            signInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(new ApplicationUser());
            userManager.Setup(x => x.GetValidTwoFactorProvidersAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(userFactors);

            var sut = new AdminController(userManager.Object, signInManager.Object, null, null, null);
        
            var result = await sut.SendCode(returnUrl, rememberMe) as ViewResult;
            var modelResult = result.ViewData.Model as SendCodeViewModel;

            Assert.Equal(modelResult.ReturnUrl, returnUrl);
            Assert.Equal(modelResult.RememberMe, rememberMe);
            Assert.Equal(expectedProviders, modelResult.Providers, new SelectListItemComparer());
        }

        [Fact]
        public void SendCodeGetHasHttpGetAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.SendCode(It.IsAny<string>(), It.IsAny<bool>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void SendCodeGetHasAllowAnonymousAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.SendCode(It.IsAny<string>(), It.IsAny<bool>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task SendCodePostWithInvalidModelStateReturnsView()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            sut.AddModelStateError();
            var result = await sut.SendCode(It.IsAny<SendCodeViewModel>());
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task SendCodePostInvokesGetTwoFactorAuthenticationUserAsync()
        {
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock();
            var sut = new AdminController(null, signInManager.Object, null, null, null);
            await sut.SendCode(It.IsAny<SendCodeViewModel>());

            signInManager.Verify(x => x.GetTwoFactorAuthenticationUserAsync(), Times.Once);
        }

        [Fact]
        public async Task SendCodePosReturnsErrorViewWhenUserIsNotFound()
        {
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock();

            var sut = new AdminController(null, signInManager.Object, null, null, null);
            var result = await sut.SendCode(It.IsAny<SendCodeViewModel>()) as ViewResult;

            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task SendCodePostInvokesGenerateTwoFactorTokenAsyncWithCorrectUserAndTokenProvider()
        {
            var applicationUser = new ApplicationUser();
            var model = new SendCodeViewModel { SelectedProvider = "Email" };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            signInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(applicationUser);

            var sut = new AdminController(userManager.Object, signInManager.Object, null, null, null);
            await sut.SendCode(model);

            userManager.Verify(x => x.GenerateTwoFactorTokenAsync(applicationUser, model.SelectedProvider), Times.Once);
        }

        [Fact]
        public async Task SendCodePostReturnsErrorViewWhenAuthenticationTokenIsNull()
        {
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            signInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(new ApplicationUser());

            var sut = new AdminController(userManager.Object, signInManager.Object, null, null, null);
            var result = await sut.SendCode(new SendCodeViewModel()) as ViewResult;

            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task SendCodePostSendsSendSecurityCodeEmailWithCorrectDataWhenSelectedProviderIsEmail()
        {
            const string token = "token";
            const string usersEmailAddress = "usersEmailAddress";

            var applicationUser = new ApplicationUser();
            var model = new SendCodeViewModel { SelectedProvider = "Email" };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();

            userManager.Setup(x => x.GenerateTwoFactorTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(token);
            userManager.Setup(x => x.GetEmailAsync(applicationUser)).ReturnsAsync(usersEmailAddress);
            signInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(applicationUser);

            var sut = new AdminController(userManager.Object, signInManager.Object, mediator.Object, null, null);
            await sut.SendCode(model);

            mediator.Verify(x => x.SendAsync(It.Is<SendSecurityCodeEmail>(y => y.Email == usersEmailAddress && y.Token == token)));
        }

        [Fact]
        public async Task SendCodePostSendsSendSecurityCodeSmsWithCorrectDataWhenSelectedProviderIsPhone()
        {
            const string token = "token";
            const string usersPhoneNumber = "usersPhoneNumber";

            var applicationUser = new ApplicationUser();
            var model = new SendCodeViewModel { SelectedProvider = "Phone" };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();

            userManager.Setup(x => x.GenerateTwoFactorTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(token);
            userManager.Setup(x => x.GetPhoneNumberAsync(applicationUser)).ReturnsAsync(usersPhoneNumber);
            signInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(applicationUser);

            var sut = new AdminController(userManager.Object, signInManager.Object, mediator.Object, null, null);
            await sut.SendCode(model);

            mediator.Verify(x => x.SendAsync(It.Is<SendAccountSecurityTokenSms>(y => y.PhoneNumber == usersPhoneNumber && y.Token == token)));
        }

        [Fact]
        public async Task SendCodePostReturnsRedirectToActionResult()
        {
            var model = new SendCodeViewModel { SelectedProvider = string.Empty, ReturnUrl = "ReturnUrl", RememberMe = true };

            var routeValues = new Dictionary<string, object>
            {
                ["Provider"] = model.SelectedProvider,
                ["ReturnUrl"] = model.ReturnUrl,
                ["RememberMe"] = model.RememberMe
            };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            signInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(new ApplicationUser());
            userManager.Setup(x => x.GenerateTwoFactorTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync("token");

            var sut = new AdminController(userManager.Object, signInManager.Object, null, null, null);
            var result = await sut.SendCode(model) as RedirectToActionResult;

            Assert.Equal(nameof(AdminController.VerifyCode), result.ActionName);
            Assert.Equal(result.RouteValues, routeValues);
        }

        [Fact]
        public void SendCodePostGetHasHttpPostAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.SendCode(It.IsAny<SendCodeViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void SendCodePostHasAllowAnonymousAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.SendCode(It.IsAny<SendCodeViewModel>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void SendCodePostHasValidateAntiForgeryTokenAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.SendCode(It.IsAny<SendCodeViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task VerifyCodeGetInvokesGetTwoFactorAuthenticationUserAsync()
        {
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock();
            var sut = new AdminController(null, signInManager.Object, null, null, null);
            await sut.VerifyCode(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>());

            signInManager.Verify(x => x.GetTwoFactorAuthenticationUserAsync(), Times.Once);
        }

        [Fact]
        public async Task VerifyCodeGetReturnsErrorViewWhenUserIsNull()
        {
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock();
            var sut = new AdminController(null, signInManager.Object, null, null, null);
            var result = await sut.VerifyCode(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()) as ViewResult;

            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task VerifyCodeGetReturnsCorrectViewModel()
        {
            const string provider = "provider";
            const bool rememberMe = true;
            const string returnUrl = "returnUrl";

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock();
            signInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(new ApplicationUser());

            var sut = new AdminController(null, signInManager.Object, null, null, null);

            var result = await sut.VerifyCode(provider, rememberMe, returnUrl) as ViewResult;
            var modelResult = result.ViewData.Model as VerifyCodeViewModel;

            Assert.Equal(modelResult.Provider, provider);
            Assert.Equal(modelResult.ReturnUrl, returnUrl);
            Assert.Equal(modelResult.RememberMe, rememberMe);
        }

        [Fact]
        public void VerifyCodeGetHasAllowAnonymousAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.VerifyCode(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void VerifyCodeGetHasHttpGetAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.VerifyCode(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task VerifyCodePostReturnsReturnsCorrectViewAndCorrectModelWhenModelStateIsInvalid()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            sut.AddModelStateError();
            var result = await sut.VerifyCode(new VerifyCodeViewModel()) as ViewResult;
            var modelResult = result.ViewData.Model as VerifyCodeViewModel;

            Assert.IsType<ViewResult>(result);
            Assert.IsType<VerifyCodeViewModel>(modelResult);
        }

        [Fact]
        public async Task VerifyCodePostInvokesTwoFactorSignInAsyncWithCorrectParameters()
        {
            var model = new VerifyCodeViewModel
            {
                Provider = "provider",
                Code = "code",
                RememberBrowser = true,
                RememberMe = true
            };

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock();
            signInManager.Setup(x => x.TwoFactorSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new Microsoft.AspNetCore.Identity.SignInResult());

            var sut = new AdminController(null, signInManager.Object, null, null, null);
            await sut.VerifyCode(model);

            signInManager.Verify(x => x.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser));
        }

        [Fact]
        public async Task VerifyCodePostAddsErrorMessageToModelStateErrorWhenTwoFactorSignInAsyncIsNotSuccessful()
        {
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock();
            signInManager.Setup(x => x.TwoFactorSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new Microsoft.AspNetCore.Identity.SignInResult());

            var sut = new AdminController(null, signInManager.Object, null, null, null);
            await sut.VerifyCode(new VerifyCodeViewModel());

            var errorMessage = sut.ModelState.GetErrorMessages().Single();
            Assert.Equal("Invalid code.", errorMessage);
        }

        [Fact]
        public async Task VerifyCodePostReturnsLockoutViewIfTwoFactorSignInAsyncFailsAndIsLockedOut()
        {
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock();
            signInManager.Setup(x => x.TwoFactorSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

            var sut = new AdminController(null, signInManager.Object, null, null, null);
            var result = await sut.VerifyCode(new VerifyCodeViewModel()) as ViewResult;

            Assert.Equal("Lockout", result.ViewName);
        }

        [Fact]
        public async Task VerifyCodePostRedirectsToReturnUrlWhenTwoFactorSignInAsyncSucceedsAndReturnUrlIsLocalUrl()
        {
            var model = new VerifyCodeViewModel { ReturnUrl = "returnUrl" };

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock();
            signInManager.Setup(x => x.TwoFactorSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.IsLocalUrl(model.ReturnUrl)).Returns(true);

            var sut = new AdminController(null, signInManager.Object, null, null, null) { Url = urlHelper.Object };
            var result = await sut.VerifyCode(model) as RedirectResult;

            Assert.Equal(result.Url, model.ReturnUrl);
        }

        [Fact]
        public async Task VerifyCodePostRedirectsToHomeControllerIndexWhenTwoFactorSignInAsyncSucceedsAndReturnUrlIsNotLocalUrl()
        {
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock();
            signInManager.Setup(x => x.TwoFactorSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(false);

            var sut = new AdminController(null, signInManager.Object, null, null, null) { Url = urlHelper.Object };
            var result = await sut.VerifyCode(new VerifyCodeViewModel()) as RedirectToPageResult;

            Assert.Equal(result.PageName, "/Index");
        }

        [Fact]
        public void VerifyCodePostHasAllowAnonymousAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.VerifyCode(It.IsAny<VerifyCodeViewModel>())).OfType<AllowAnonymousAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void VerifyCodePostHasHttpPostAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.VerifyCode(It.IsAny<VerifyCodeViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void VerifyCodePostHasValidateAntiForgeryTokenAttribute()
        {
            var sut = CreateAdminControllerWithNoInjectedDependencies();
            var attribute = sut.GetAttributesOn(x => x.VerifyCode(It.IsAny<VerifyCodeViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        private static AdminController CreateAdminControllerWithNoInjectedDependencies() => new AdminController(null, null, null, null, null);
    }
}
