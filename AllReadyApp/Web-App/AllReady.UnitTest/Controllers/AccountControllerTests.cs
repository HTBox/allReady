using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Controllers;
using AllReady.Models;
using AllReady.Services;
using AutoMoq;
using AutoMoq.Helpers;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Moq;
using Xunit;
using MediatR;
using Shouldly;

namespace AllReady.UnitTest.Controllers
{
    public class AccountControllerTests
    {
        public static dynamic CommonSetup(AutoMoqer mocker)
        {
            var store = mocker.GetMock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null,
                null, null);
            var mockHttpContext = new Mock<HttpContext>();

            var contextAccessor = mocker.GetMock<IHttpContextAccessor>();
            contextAccessor.Setup(mock => mock.HttpContext).Returns(() => mockHttpContext.Object);

            var claimsFactory = mocker.GetMock<IUserClaimsPrincipalFactory<ApplicationUser>>();

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null, null);

            var signInResult = default(SignInResult);

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

            mocker.SetInstance(signInManagerMock.Object);
            mocker.SetInstance(userManagerMock.Object);

            dynamic result = new ExpandoObject();
            result.UserManagerMock = userManagerMock;
            return result;
        }

        public class RegisterGetTests : AutoMoqTestFixture<AccountController>
        {
            public RegisterGetTests()
            {
                ResetSubject();
                CommonSetup(Mocker);
            }

            [Fact]
            public void Returns_a_view()
            {
                var result = Subject.Register();
                result.ShouldBeOfType(typeof (ViewResult));
            }
        }

        public class RegisterPostTests : AutoMoqTestFixture<AccountController>
        {
            private RegisterViewModel registerViewModel;
            private Mock<UserManager<ApplicationUser>> userManagerMock;
            private string defaultTimeZone;

            public RegisterPostTests()
            {
                ResetSubject();
                var setup = CommonSetup(Mocker);
                userManagerMock = setup.UserManagerMock;

                defaultTimeZone = Guid.NewGuid().ToString();
                var generalSettings = new GeneralSettings {DefaultTimeZone = defaultTimeZone};
                Mocked<IOptions<GeneralSettings>>()
                    .Setup(x => x.Value)
                    .Returns(generalSettings);

                registerViewModel = new RegisterViewModel
                {
                    Password = Guid.NewGuid().ToString(),
                    Email = Guid.NewGuid().ToString()
                };

                userManagerMock.Setup(
                    x =>
                        x.CreateAsync(
                            It.Is<ApplicationUser>(
                                y => y.Email == registerViewModel.Email && y.UserName == registerViewModel.Email && y.TimeZoneId == defaultTimeZone),
                            registerViewModel.Password))
                    .Returns(Task.FromResult(IdentityResult.Success));
            }

            [Fact]
            public void New_test()
            {
                Subject.Register(registerViewModel).Wait();
            }

            [Fact]
            public void It_should_redirect_to_home_page_after_a_success_user_creation()
            {
                var result = Subject.Register(registerViewModel);

                result.Result.ShouldBeOfType(typeof(RedirectToActionResult));

                var redirect = result.Result as RedirectToActionResult;
                redirect.ActionName.ShouldBe("Index");
                redirect.ControllerName.ShouldBe("Home");
            }

            [Fact]
            public void It_should_stay_on_the_registration_page_if_the_user_creation_fails()
            {
                userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerViewModel.Password))
                    .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError())));

                var result = Subject.Register(registerViewModel);

                result.Result.ShouldBeOfType(typeof(ViewResult));

                (result.Result as ViewResult).ViewData.Model.ShouldBeSameAs(registerViewModel);
            }
        }

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

        [Fact(Skip = "NotImplmented")]
        public void LoginGetHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplmented")]
        public void LoginGetHasAllowAnonymousAttribute()
        {
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

        [Fact(Skip = "NotImplemented")]
        public void LoginPostHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void LoginPostHasAllowAnonymousAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void LoginPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void RegisterGetReturnsAView()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void RegisterGetHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void RegisterGetHasAllowAnonymousAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RegisterPostReturnsSameViewAndViewModelWhenModelStateIsInvalid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RegisterPostInvokesCreateAsyncWithTheCorrectParametersWhenModelStateIsValid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RegisterPostInvokesGenerateEmailConfirmationTokenAsyncWithTheCorrectParametersWhenModelStateIsValidAndUserCreationIsSuccessful()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RegisterPostInvokesUrlActionWithTheCorrectParametersWhenModelStateIsValidAndUserCreationIsSuccessful()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RegisterPostInvokesSendEmailAsyncWithTheCorrectParametersWhenModelStateIsValidAndUserCreationIsSuccessful()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RegisterPostInvokesAddClaimAsyncWithTheCorrectParametersWhenModelStateIsValidAndUserCreationIsSuccessful()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RegisterPostInvokesSignInAsyncWithTheCorrectParametersWhenModelStateIsValidAndUserCreationIsSuccessful()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RegisterPostRedirectsToCorrectActionAndControllerWhenModelStateIsValidAndUserCreationIsSuccessful()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RegisterPostAddsIdentityResultErrorsToModelStateErrorWhenUserCreationFails()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RegisterPostReturnsTheSameViewAndViewModelWhenUserCreationFails()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void RegisterPostHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void RegisterPostHasAllowAnonymousAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void RegisterPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task LogOffInvokesSignOutAsync()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task LogOffRedirectToCorrectActionAndController()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmEmailReturnsErrorViewWhenUserIdIsNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmEmailReturnsErrorViewWhenTokenIsNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmEmailInvokesFindByIdAsyncWithCorrectUserIdWhenUserIdAndTokenAreNotNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmEmailReturnsErrorViewWhenUserIsNullAndUserIdAndTokenAreNotNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmEmailInvokesConfirmEmailAsyncWithTheCorrectParametersWhenUserAndUserIdAndTokenAreNotNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmEmailInvokesSendAsyncWithTheCorrectParametersWhenUsersProfileIsCompleteAndUsersEmailIsConfirmedAndUserAndUserIdAndTokenAreNotNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmEmailInvokesRefreshSignInAsyncWithTheCorrectParametersWhenUserIsSignedInAndUsersProfileIsCompleteAndUsersEmailIsConfirmedAndUserAndUserIdAndTokenAreNotNull()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmEmailReturnsErrorViewWhenUsersEmailCannotBeConfirmed()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmEmailReturnsConfirmEmailViewWhenUsersEmailCanBeConfirmed()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void ConfirmEmailHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ConfirmEmailHasAllowAnonymousAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ForgotPasswordGetReturnsAView()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ForgotPasswordGetHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ForgotPasswordGetHasAllowAnonymousAttribute()
        {
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

        [Fact(Skip = "NotImplemented")]
        public void ForgotPasswordPostHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ForgotPasswordPostHasAllowAnonymousAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ForgotPasswordPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ResetPasswordGetReturnsErrorViewIfCodeIsNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ResetPasswordGetReturnsAViewIfCodeIsNotNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ResetPasswordGetHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ResetPasswordGetHasAllowAnonymousAttribute()
        {
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

        [Fact(Skip = "NotImplemented")]
        public void ResetPasswordPostHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ResetPasswordPostHasAllowAnonymousAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ResetPasswordPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ResetPasswordConfirmationReturnsAView()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ResetPasswordConfirmationHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ResetPasswordConfirmationHasAllowAnonymousAttribute()
        {
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

        [Fact(Skip = "NotImplemented")]
        public void ExternalLoginHasPostGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ExternalLoginHasAllowAnonymousAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ExternalLoginHasValidateAntiForgeryTokenAttribute()
        {
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

        [Fact(Skip = "NotImplemented")]
        public void ExternalLoginCallbackHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ExternalLoginCallbackHasAllowAnonymousAttribute()
        {
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

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginConfirmationPutsCorrectDataInViewDataWithCorrectKeyWhenModelStateIsInvalid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ExternalLoginConfirmationReturnsCorrectViewModelWhenModelStateIsInvalid()
        {
            //delete this line when starting work on this unit test
            await taskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void ExternalLoginConfirmationHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ExternalLoginConfirmationHasAllowAnonymousAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ExternalLoginConfirmationHasValidateAntiForgeryTokenAttribute()
        {
        }

        private static AccountController AccountController(SignInResult signInResult = default(SignInResult))
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null,
                null, null);
            var mockHttpContext = new Mock<HttpContext>();

            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(mock => mock.HttpContext).Returns(() => mockHttpContext.Object);
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null, null);
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
    }
}