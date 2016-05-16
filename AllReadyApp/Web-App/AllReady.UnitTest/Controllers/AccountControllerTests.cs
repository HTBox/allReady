using System.Linq;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Models;
using AllReady.Services;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Moq;
using Xunit;
using MediatR;
using AllReady.UnitTest.Extensions;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc.Routing;
using System.Security.Claims;
using AllReady.Extensions;
using AllReady.Features.Login;

namespace AllReady.UnitTest.Controllers
{
    public class AccountControllerTests
    {
        //delete this line when all unit tests using it have been completed
        private readonly Task taskFromResultZero = Task.FromResult(0);

        [Fact]
        public void LoginGetPopulatesViewDataWithTheCorrectTestUrl()
        {
            var sut = AccountController();

            var result = (ViewResult) sut.Login();
            Assert.Null(result.ViewData["ReturnUrl"]);

            const string testUrl = "return url";
            result = (ViewResult)sut.Login(testUrl);
            Assert.Equal(testUrl, result.ViewData["ReturnUrl"]);
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
        public async void LoginPostRedirectsToLocalWithCorrectParameters()
        {
            var loginViewModel = new LoginViewModel { Email = "", Password = "", RememberMe = false };
            const string testLocalUrl = "/foo/bar";

            var controller = AccountController();
            var result = await controller.Login(loginViewModel, testLocalUrl);

            Assert.IsType<RedirectResult>(result);
            var redirectToLocalUrl = (RedirectResult)result;
            Assert.Equal(testLocalUrl, redirectToLocalUrl.Url);
        }

        [Fact]
        public async Task LoginPostReturnsSameViewAndViewModelWhenModelStatIsInvalid()
        {
            const string testUrl = "return url";
            var loginViewModel = new LoginViewModel();

            var sut = AccountController();
            sut.ModelState.AddModelError("foo", "bar");

            var result = await sut.Login(loginViewModel, testUrl);
            Assert.IsType<ViewResult>(result);

            var resultViewModel = ((ViewResult) result).ViewData.Model;
            Assert.IsType<LoginViewModel>(resultViewModel);
            Assert.Equal(resultViewModel, loginViewModel);
        }

        [Fact]
        public void LoginPostRedirectRemoteUrlTests()
        {
            const string testRemoteUrl = "http://foo.com/t";
            var loginViewModel = new LoginViewModel {Email = "", Password = "", RememberMe = false};
            
            var sut = AccountController();
            var result = sut.Login(loginViewModel, testRemoteUrl).GetAwaiter().GetResult();

            Assert.IsType<RedirectToActionResult>(result);

            var redirectToAction = (RedirectToActionResult)result;
            Assert.Equal("Home", redirectToAction.ControllerName);
            Assert.Equal(nameof(HomeController.Index), redirectToAction.ActionName);
        }

        [Fact]
        public async Task LoginPostFailureTests()
        {
            var loginViewModel = new LoginViewModel { Email = "", Password = "", RememberMe = false };
            const string testLocalUrl = "/foo/bar";

            var controller = AccountController(SignInResult.Failed);
            var result = await controller.Login(loginViewModel, testLocalUrl);

            Assert.IsType<ViewResult>(result);

            var resultViewModel = ((ViewResult)result).ViewData.Model;
            Assert.IsType<LoginViewModel>(resultViewModel);
            Assert.Equal(resultViewModel, loginViewModel);
            Assert.True(controller.ModelState[string.Empty].Errors
                .Select(x => x.ErrorMessage)
                .Contains("Invalid login attempt."));
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
        public async Task RegisterPostReturnsSameViewAndViewModelWhenModelStateIsInvalid()
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
        public async Task RegisterPostInvokesCreateAsyncWithTheCorrectParametersWhenModelStateIsValid()
        {
            const string defaultTimeZone = "DefaultTimeZone";

            var model = new RegisterViewModel { Email = "email", Password = "Password" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = defaultTimeZone });

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Failed()));

            var sut = new AccountController(userManager.Object, null, null, generalSettings.Object, null);
            await sut.Register(model);

            userManager.Verify(x => x.CreateAsync(It.Is<ApplicationUser>(au =>
                au.UserName == model.Email &&
                au.Email == model.Email &&
                au.TimeZoneId == defaultTimeZone),
                model.Password), Times.Once);
        }

        [Fact]
        public async Task RegisterPostInvokesGenerateEmailConfirmationTokenAsyncWithTheCorrectParametersWhenModelStateIsValidAndUserCreationIsSuccessful()
        {
            const string defaultTimeZone = "DefaultTimeZone";

            var model = new RegisterViewModel { Email = "email", Password = "Password" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = defaultTimeZone });

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));

            var signInManager = CreateSignInManagerMock(userManager);
            var emailSenderMock = new Mock<IEmailSender>();

            var sut = new AccountController(userManager.Object, signInManager.Object, emailSenderMock.Object, generalSettings.Object, null);
            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = Mock.Of<IUrlHelper>();

            await sut.Register(model);

            userManager.Verify(x => x.GenerateEmailConfirmationTokenAsync(It.Is<ApplicationUser>(au =>
                au.UserName == model.Email &&
                au.Email == model.Email &&
                au.TimeZoneId == defaultTimeZone)), Times.Once);
        }

        [Fact]
        public async Task RegisterPostInvokesUrlActionWithTheCorrectParametersWhenModelStateIsValidAndUserCreationIsSuccessful()
        {
            const string requestScheme = "requestScheme";

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).Returns(() => Task.FromResult(It.IsAny<string>()));

            var signInManager = CreateSignInManagerMock(userManager);
            var emailSenderMock = new Mock<IEmailSender>();

            var sut = new AccountController(userManager.Object, signInManager.Object, emailSenderMock.Object, generalSettings.Object, null);

            sut.SetFakeHttpRequestSchemeTo(requestScheme);
            var urlHelper = new Mock<IUrlHelper>();
            sut.Url = urlHelper.Object;

            await sut.Register(new RegisterViewModel());

            urlHelper.Verify(mock => mock.Action(It.Is<UrlActionContext>(uac =>
                uac.Action == "ConfirmEmail" &&
                uac.Controller == "Account" &&
                uac.Protocol == requestScheme)),
                Times.Once);
        }

        [Fact]
        public async Task RegisterPostInvokesSendEmailAsyncWithTheCorrectParametersWhenModelStateIsValidAndUserCreationIsSuccessful()
        {
            const string callbackUrl = "callbackUrl";

            var model = new RegisterViewModel { Email = "email" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).Returns(() => Task.FromResult(It.IsAny<string>()));

            var signInManager = CreateSignInManagerMock(userManager);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(callbackUrl);

            var emailSenderMock = new Mock<IEmailSender>();

            var sut = new AccountController(userManager.Object, signInManager.Object, emailSenderMock.Object, generalSettings.Object, null);

            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            await sut.Register(model);

            emailSenderMock.Verify(x => x.SendEmailAsync(
                It.Is<string>(y => y == model.Email), 
                It.IsAny<string>(), 
                It.Is<string>(y => y.Contains(callbackUrl))), Times.Once);
        }

        [Fact]
        public async Task RegisterPostInvokesAddClaimAsyncWithTheCorrectParametersWhenModelStateIsValidAndUserCreationIsSuccessful()
        {
            const string defaultTimeZone = "DefaultTimeZone";

            var model = new RegisterViewModel { Email = "email" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = defaultTimeZone });

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).Returns(() => Task.FromResult(It.IsAny<string>()));

            var signInManager = CreateSignInManagerMock(userManager);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var emailSenderMock = new Mock<IEmailSender>();
            emailSenderMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(()=>Task.FromResult(It.IsAny<Task>()));

            var sut = new AccountController(userManager.Object, signInManager.Object, emailSenderMock.Object, generalSettings.Object, null);

            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            await sut.Register(model);

            userManager.Verify(x => x.AddClaimAsync(It.Is<ApplicationUser>(au =>
                au.UserName == model.Email &&
                au.Email == model.Email &&
                au.TimeZoneId == defaultTimeZone)
                , It.IsAny<Claim>()), Times.Once);
        }

        [Fact]
        public async Task RegisterPostInvokesSignInAsyncWithTheCorrectParametersWhenModelStateIsValidAndUserCreationIsSuccessful()
        {
            const string defaultTimeZone = "DefaultTimeZone";

            var model = new RegisterViewModel { Email = "email" };

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings { DefaultTimeZone = defaultTimeZone });

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).Returns(() => Task.FromResult(It.IsAny<string>()));

            var signInManager = CreateSignInManagerMock(userManager);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var emailSenderMock = new Mock<IEmailSender>();
            emailSenderMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(() => Task.FromResult(It.IsAny<Task>()));

            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).Returns(() => Task.FromResult(IdentityResult.Success));

            var sut = new AccountController(userManager.Object, signInManager.Object, emailSenderMock.Object, generalSettings.Object, null);

            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            await sut.Register(model);

            signInManager.Verify(x => x.SignInAsync(It.Is<ApplicationUser>(au =>
                au.UserName == model.Email &&
                au.Email == model.Email &&
                au.TimeZoneId == defaultTimeZone)
                , It.IsAny<bool>(), null), Times.Once);
        }

        [Fact]
        public async Task RegisterPostRedirectsToCorrectActionAndControllerWhenModelStateIsValidAndUserCreationIsSuccessful()
        {
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Success));
            userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).Returns(() => Task.FromResult(It.IsAny<string>()));

            var signInManager = CreateSignInManagerMock(userManager);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var emailSenderMock = new Mock<IEmailSender>();
            emailSenderMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(() => Task.FromResult(It.IsAny<Task>()));

            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).Returns(() => Task.FromResult(IdentityResult.Success));
            signInManager.Setup(x => x.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null)).Returns(() => Task.FromResult(It.IsAny<Task>()));

            var sut = new AccountController(userManager.Object, signInManager.Object, emailSenderMock.Object, generalSettings.Object, null);

            sut.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            sut.Url = urlHelper.Object;

            var result = await sut.Register(new RegisterViewModel()) as RedirectToActionResult;

            Assert.Equal(result.ActionName, nameof(HomeController.Index));
            Assert.Equal(result.ControllerName, "Home");
        }

        [Fact]
        public async Task RegisterPostAddsIdentityResultErrorsToModelStateErrorWhenUserCreationFails()
        {
            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "IdentityErrorDescription" });

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(identityResult));

            var sut = new AccountController(userManager.Object, null,null, generalSettings.Object, null);

            await sut.Register(new RegisterViewModel());

            var errorMessages = sut.ModelState.GetErrorMessages();
            Assert.Equal(errorMessages.Single(), identityResult.Errors.Select(x => x.Description).Single());
        }

        [Fact]
        public async Task RegisterPostReturnsTheSameViewAndViewModelWhenUserCreationFails()
        {
            var model = new RegisterViewModel();

            var generalSettings = new Mock<IOptions<GeneralSettings>>();
            generalSettings.Setup(x => x.Value).Returns(new GeneralSettings());

            var userManager = CreateUserManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(() => Task.FromResult(IdentityResult.Failed()));

            var sut = new AccountController(userManager.Object, null, null, generalSettings.Object, null);

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
            var userManager = CreateUserManagerMock();
            var signInManager = CreateSignInManagerMock(userManager);

            var sut = new AccountController(userManager.Object, signInManager.Object, null,null, null);

            await sut.LogOff();
            signInManager.Verify(x => x.SignOutAsync(), Times.Once);
        }

        [Fact]
        public async Task LogOffRedirectToCorrectActionAndController()
        {
            var userManager = CreateUserManagerMock();
            var signInManager = CreateSignInManagerMock(userManager);
            signInManager.Setup(x => x.SignOutAsync()).Returns(() => Task.FromResult(It.IsAny<Task>()));

            var sut = new AccountController(userManager.Object, signInManager.Object, null, null, null);

            var result = await sut.LogOff() as RedirectToActionResult;

            Assert.Equal(result.ActionName, nameof(HomeController.Index));
            Assert.Equal(result.ControllerName, "Home");
        }

        [Fact]
        public async Task ConfirmEmailReturnsErrorViewWhenUserIdIsNull()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();

            var result = await sut.ConfirmEmail(null,"sometoken") as ViewResult;

            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task ConfirmEmailReturnsErrorViewWhenTokenIsNull()
        {
            var sut = CreateAccountControllerWithNoInjectedDependencies();

            var result = await sut.ConfirmEmail("someuserid", null) as ViewResult;

            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task ConfirmEmailInvokesFindByIdAsyncWithCorrectUserIdWhenUserIdAndTokenAreNotNull()
        {
            var userId = "userId";
            var token = "someToken";
            var userManager = CreateUserManagerMock();

            var sut = new AccountController(userManager.Object, null, null, null, null);

            await sut.ConfirmEmail(userId, token);

            userManager.Verify(x => x.FindByIdAsync(It.Is<string>(y => y == userId)), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailReturnsErrorViewWhenUserIsNullAndUserIdAndTokenAreNotNull()
        {
            var userId = "userId";
            var token = "someToken";
            var userManager = CreateUserManagerMock();

            userManager.Setup(x => x.FindByIdAsync(userId)).Returns(() => Task.FromResult((ApplicationUser)null));

            var sut = new AccountController(userManager.Object, null, null, null, null);

            var result = await sut.ConfirmEmail(userId,token) as ViewResult;

            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public async Task ConfirmEmailInvokesConfirmEmailAsyncWithTheCorrectParametersWhenUserAndUserIdAndTokenAreNotNull()
        {
            var userId = "userId";
            var token = "someToken";
            var userManager = CreateUserManagerMock();

            var user = new ApplicationUser();
 
            userManager.Setup(x => x.FindByIdAsync(userId)).Returns(() => Task.FromResult(user));
            userManager.Setup(x=>x.ConfirmEmailAsync(user,token)).Returns(()=> Task.FromResult(IdentityResult.Success));
            var sut = new AccountController(userManager.Object, null, null, null, null);

            await sut.ConfirmEmail(userId, token);

            userManager.Verify(x=>x.ConfirmEmailAsync(It.Is<ApplicationUser>(y => y == user),It.Is<string>(y=> y == token)), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailInvokesSendAsyncWithTheCorrectParametersWhenUsersProfileIsCompleteAndUsersEmailIsConfirmedAndUserAndUserIdAndTokenAreNotNull()
        {
            var userId = "userId";
            var token = "someToken";
            var userManager = CreateUserManagerMock();

            var user = new ApplicationUser()
            {
                Id=userId,
                Name="test",
                PhoneNumber="test",
                PhoneNumberConfirmed=true,
                Email="test@email.com",
                EmailConfirmed=true
            };

            userManager.Setup(x => x.FindByIdAsync(userId)).Returns(() => Task.FromResult(user));
            userManager.Setup(x => x.ConfirmEmailAsync(user, token)).Returns(() => Task.FromResult(IdentityResult.Success));

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(new RemoveUserProfileIncompleteClaimCommand { UserId = user.Id })).Returns(()=> Task.FromResult(It.IsAny<Unit>()));

            var sut = new AccountController(userManager.Object, null, null, null, mediator.Object);
            sut.SetFakeUser(userId);

            await sut.ConfirmEmail(userId, token);

            mediator.Verify(x => x.SendAsync(It.Is<RemoveUserProfileIncompleteClaimCommand>(y => y.UserId == userId)), Times.Once);

        }

        [Fact]
        public async Task ConfirmEmailInvokesRefreshSignInAsyncWithTheCorrectParametersWhenUserIsSignedInAndUsersProfileIsCompleteAndUsersEmailIsConfirmedAndUserAndUserIdAndTokenAreNotNull()
        {
            var userId = "userId";
            var token = "someToken";
            var userManager = CreateUserManagerMock();

            var user = new ApplicationUser()
            {
                Id = userId,
                Name = "test",
                PhoneNumber = "test",
                PhoneNumberConfirmed = true,
                Email = "test@email.com",
                EmailConfirmed = true
            };

            userManager.Setup(x => x.FindByIdAsync(userId)).Returns(() => Task.FromResult(user));
            userManager.Setup(x => x.ConfirmEmailAsync(user, token)).Returns(() => Task.FromResult(IdentityResult.Success));

            var signInManager = CreateSignInManagerMock(userManager);
            signInManager.Setup(x => x.RefreshSignInAsync(user)).Returns(() => Task.FromResult(It.IsAny<Task>()));

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(new RemoveUserProfileIncompleteClaimCommand { UserId = user.Id })).Returns(() => Task.FromResult(It.IsAny<Unit>()));

            var sut = new AccountController(userManager.Object, signInManager.Object, null, null, mediator.Object);
            sut.SetFakeUserWithCookieAuthenticationType(userId);

            await sut.ConfirmEmail(userId, token);

            signInManager.Verify(x => x.RefreshSignInAsync(It.Is<ApplicationUser>(y => y.Id == user.Id)));
        }

        [Fact]
        public async Task ConfirmEmailReturnsErrorViewWhenUsersEmailCannotBeConfirmed()
        {
            var userId = "userId";
            var token = "someToken";
            var userManager = CreateUserManagerMock();

            var user = new ApplicationUser();

            userManager.Setup(x => x.FindByIdAsync(userId)).Returns(() => Task.FromResult(user));
            userManager.Setup(x => x.ConfirmEmailAsync(user, token)).Returns(() => Task.FromResult(IdentityResult.Failed()));
            var sut = new AccountController(userManager.Object, null, null, null, null);

            var result = await sut.ConfirmEmail(userId, token) as ViewResult;

            Assert.Equal(result.ViewName, "Error");
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmEmailReturnsConfirmEmailViewWhenUsersEmailCanBeConfirmed()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
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

        [Fact(Skip = "NotImplemented")]
        public async Task ForgotPasswordPostInvokesFindByNameAsyncWithTheCorrectEmailwhenModelStateIsValid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ForgotPasswordPostInvokesIsEmailConfirmedAsyncWithThecorrectUserWhenModelStateIsValid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ForgotPasswordPostReturnsForgotPasswordConfirmationViewWhenModelStateIsValidAndUserIsNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ForgotPasswordPostReturnsForgotPasswordConfirmationViewWhenModelStateIsValidAndUsersEmailIsUnverified()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ForgotPasswordPostInvokesGeneratePasswordResetTokenAsyncWithCorrectUserWhenModelStateIsValidAndUserIsNotNullAndUsersEmailHasBeenVerified()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ForgotPasswordPostInvokesUrlActionWithCorrectParametersWhenModelStateIsValidAndUserIsNotNullAndUsersEmailHasBeenVerified()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ForgotPasswordPostInvokesSendEmailAsyncWithCorrectParametersWhenModelStateIsValidAndUserIsNotNullAndUsersEmailHasBeenVerified()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ForgotPasswordPostReturnsForgotPasswordConfirmationViewWhenModelStateIsValidAndUserIsNotNullAndUsersEmailHasBeenVerified()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ForgotPasswordPostReturnsTheSameViewAndViewModelWhenModelStateIsInvalid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
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
            var code = "1234";
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

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordPostReturnsTheSameViewAndViewModelWhenModelStateIsInvalid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordPostInvokesFindByNameAsyncWithTheCorrecEmailWhenModelStateIsValid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordPostRedirectsToCorrectActionWhenUserIsNullAndModelStateIsValid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordPostInvokesResetPasswordAsyncWithCorrectParametersWhenUserIsNotNullAndModelStateIsValid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordPostRedirectsToCorrectActionWhenUsersPasswordResetSucceededAndUserIsNotNullAndModelStateIsValid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordPostAddsIdentityResultErrorsToModelStateErrorsWhenUsersPasswordResetFailedAndUserIsNotNullAndModelStateIsValid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordPostReturnsAViewWhenUsersPasswordResetFailedAndUserIsNotNullAndModelStateIsValid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
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

        [Fact(Skip = "NotImplemented")]
        public void ExternalLoginInvokesUrlActionWithCorrectParameters()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ExternalLoginInvokesConfigureExternalAuthenticationPropertiesWithCorrectParameters()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ExternalLoginReturnsChallengeResultWithCorrectParameters()
        {
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

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginCallbackInvokesGetExternalLoginInfoAsync()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginCallbackRedirectsToCorrectActionWhenExternalLoginInfoIsNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginCallbackInvokesExternalLoginSignInAsyncWithCorrectParametersWhenExternalLoginInfoIsNotNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginCallbackSendsAsyncApplicationUserQueryWhenExternalLoginIsSuccessfulAndExternalLoginInfoIsNotNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginCallbackRedirectsToLocalWithCorrectUrlWhenReturnUrlIsLocalUrl()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginCallbackRedirectsToCorrectActionAndControllerWhenUserIsASiteAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginCallbackRedirectsToCorrectActionAndControllerWithCorrerctRouteValuesWhenUserIsAnOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginCallbackRedirectsToCorrectActionAndControllerWhenUrlIsNotALocalUrlAndUserTypeIsNotASiteAdminOrAnOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginCallbackAddsCorrectDataToViewDataWhenWhenExternalLoginIsUnsuccessfulOrExternalLoginInfoIsNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginCallbackReturnsExternalLoginConfirmationViewAndCorrecgtViewModelWhenWhenExternalLoginIsUnsuccessfulOrExternalLoginInfoIsNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
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

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginConfirmationRedirectsToCorrectActionIfUserIsSignedIn()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginConfirmationInvokesGetExternalLoginInfoAsyncWhenModelStateIsValid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginConfirmationReturnsExternalLoginFailureViewUserIsNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginConfirmationInvokesCreateAsyncWithCorrectUserWhenExternalLoginInfoIsSuccessfulAndModelStateIsValid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginConfirmationInvokesAddLoginAsyncWithCorrectParametersWhenUserIsCreatedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginConfirmationInvokesSignInAsyncWithCorrectParametersWhenExternalLoginIsAddedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginConfirmationRedirectsToCorrectUrlWhenUrlIsLocalUrl()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginConfirmationRedirectsToCorrectActionAndControllerWithCorrectRouteValuesWhenUserIsSiteAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginConfirmationRedirectsToCorrectActionAndContrllerWithCorrectRouteValuesWhenUserIsOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginConfirmationRedirectsToCorrectActionAndContrllerWhenUrlIsNotLocalUrlAndUserIsNeitherSiteAdminOrOrgAdmin()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginConfirmationAddsIdentityResultErrorsToModelStateErrorWhenUserIsCreatedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact]
        public async Task ExternalLoginConfirmationPutsCorrectDataInViewDataWithCorrectKeyWhenModelStateIsInvalid()
        {
            var returnUrlKey = "ReturnUrl";
            var returnUrlValue = "http:\\test.url.com";
            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel();
            AccountController controller = AccountController();
            controller.ViewData.ModelState.AddModelError("Error", "test");
            controller.SetFakeUser("test");
            var result = await controller.ExternalLoginConfirmation(model, returnUrlValue) as ViewResult;
            var viewDataKey = result.ViewData.Keys.FirstOrDefault(k => k == returnUrlKey);
            var viewDataValue = result.ViewData[returnUrlKey] as string;
            Assert.Equal<string>(viewDataValue, returnUrlValue);
            Assert.NotNull(viewDataKey);
            
        }

        [Fact]
        public async Task ExternalLoginConfirmationReturnsCorrectViewModelWhenModelStateIsInvalid()
        {
            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel();
            AccountController controller = AccountController();
            controller.ViewData.ModelState.AddModelError("Error", "test");
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

        private static AccountController AccountController(SignInResult signInResult = default(SignInResult))
        {
            var userManagerMock = CreateUserManagerMock();
            var signInManagerMock = CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(mock => mock
                .PasswordSignInAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(signInResult == default(SignInResult) 
                            ? SignInResult.Success 
                            : signInResult
                );
            var emailSenderMock = new Mock<IEmailSender>();
            var generalSettingsMock = new Mock<IOptions<GeneralSettings>>();
            var mediatorMock = new Mock<IMediator>();

            var controller = new AccountController(
                userManagerMock.Object,
                signInManagerMock.Object,
                emailSenderMock.Object,
                generalSettingsMock.Object,
                mediatorMock.Object);
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(mock => mock.IsLocalUrl(It.Is<string>(x => x.StartsWith("http"))))
                .Returns(false);
            urlHelperMock
                .Setup(mock => mock.IsLocalUrl(It.Is<string>(x => !x.StartsWith("http"))))
                .Returns(true);
            controller.Url = urlHelperMock.Object;
            return controller;
        }

        private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock() => 
            new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null, null);

        private static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(Mock<UserManager<ApplicationUser>> userManager)
        {
            var httpContext = new Mock<HttpContext>();

            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(mock => mock.HttpContext).Returns(() => httpContext.Object);
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var signInManager = new Mock<SignInManager<ApplicationUser>>(
            userManager.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            null, null);

            return signInManager;
    }

        private static AccountController CreateAccountControllerWithNoInjectedDependencies() => new AccountController(null, null, null, null, null);
    }
}