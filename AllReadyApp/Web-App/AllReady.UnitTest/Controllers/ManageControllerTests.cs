using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using AllReady.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MediatR;
using AllReady.ViewModels.Manage;
using static AllReady.Controllers.ManageController;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using AllReady.Features.Manage;

namespace AllReady.UnitTest.Controllers
{
    public class ManageControllerTests
    {
        //delete this line when all unit tests using it have been completed
        private static readonly Task TaskCompletedTask = Task.CompletedTask;

        [Fact]
        public async Task IndexGetAddsCorrectMessageToViewDataWhenMessageEqualsChangePasswordSuccess()
        {
            //Arrange
            //Mock controller dependencies UserManager, signinmanager and IMediator, set behaviour of called methods
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser("userId");

            //Act
            var result = await controller.Index(ManageMessageId.ChangePasswordSuccess);
            var resultViewModel = ((ViewResult)result);
            var message = resultViewModel.ViewData["StatusMessage"].ToString();

            //Assert
            Assert.Equal("Your password has been changed.", message);
        }

        [Fact]
        public async Task IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsSetPasswordSuccess()
        {
            //Arrange
            //Mock controller dependencies UserManager, signinmanager and IMediator, set behaviour of called methods
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser("userId");

            //Act
            var result = await controller.Index(ManageMessageId.SetPasswordSuccess);
            var resultViewModel = ((ViewResult)result);
            var message = resultViewModel.ViewData["StatusMessage"].ToString();

            //Assert
            Assert.Equal("Your password has been set.", message);
        }

        [Fact]
        public async Task IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsSetTwoFactorSuccess()
        {
            //Arrange
            //Mock controller dependencies UserManager, signinmanager and IMediator, set behaviour of called methods
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser("userId");

            //Act
            var result = await controller.Index(ManageMessageId.SetTwoFactorSuccess);
            var resultViewModel = ((ViewResult)result);
            var message = resultViewModel.ViewData["StatusMessage"].ToString();

            //Assert
            Assert.Equal("Your two-factor authentication provider has been set.", message);
        }

        [Fact]
        public async Task IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsError()
        {
            //Arrange
            //Mock controller dependencies UserManager, signinmanager and IMediator, set behaviour of called methods
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser("userId");

            //Act
            var result = await controller.Index(ManageMessageId.Error);
            var resultViewModel = ((ViewResult)result);
            var message = resultViewModel.ViewData["StatusMessage"].ToString();

            //Assert
            Assert.Equal("An error has occurred.", message);
        }

        [Fact]
        public async Task IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsAddPhoneSuccess()
        {
            //Arrange
            //Mock controller dependencies UserManager, signinmanager and IMediator, set behaviour of called methods
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser("userId");

            //Act
            var result = await controller.Index(ManageMessageId.AddPhoneSuccess);
            var resultViewModel = ((ViewResult)result);
            var message = resultViewModel.ViewData["StatusMessage"].ToString();

            //Assert
            Assert.Equal("Your mobile phone number was added.", message);
        }

        [Fact]
        public async Task IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsRemovePhoneSuccess()
        {
            //Arrange
            //Mock controller dependencies UserManager, signinmanager and IMediator, set behaviour of called methods
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser("userId");

            //Act
            var result = await controller.Index(ManageMessageId.RemovePhoneSuccess);
            var resultViewModel = ((ViewResult)result);
            var message = resultViewModel.ViewData["StatusMessage"].ToString();

            //Assert
            Assert.Equal("Your mobile phone number was removed.", message);
        }

        [Fact]
        public async Task IndexGetSendsUserByUserIdQueryWithCorrectUserId()
        {
            //Arrange
            var userId = "userId";

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<AllReady.Features.Manage.UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser ());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            await controller.Index();
            mediator.Verify(m => m.SendAsync(It.Is<AllReady.Features.Manage.UserByUserIdQuery>(u => u.UserId == userId)),Times.Once);
        }

        [Fact]
        public async Task IndexGetReturnsCorrectView()
        {
            //Arrange
            //Mock controller dependencies UserManager, signinmanager and IMediator, set behaviour of called methods
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser("userId");

            //Act
            var result = await controller.Index();

            //Assert
            Assert.IsType(typeof(ViewResult),result);
        }

        [Fact]
        public void IndexGetHasHttpGetAttribute()
        {
            //Arrange
            var controller = new ManageController(null, null, null);
            //Act
            var attribute = controller.GetAttributesOn(x => x.Index(It.IsAny<ManageMessageId>())).OfType<HttpGetAttribute>().SingleOrDefault();
            //Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task IndexGetReturnsCorrectViewModel()
        {
            //Arrange
            //Mock controller dependencies UserManager, signinmanager and IMediator, set behaviour of called methods
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser("userId");

            //Act
            var result = await controller.Index(ManageMessageId.RemovePhoneSuccess);
            var resultViewModel = ((ViewResult)result);
            var vm = (IndexViewModel)resultViewModel.ViewData.Model;

            //Assert
            Assert.IsType<IndexViewModel>(vm);
        }

        [Fact]
        public async Task IndexPostSendsUserByUserIdQueryWithCorrectUserId()
        {
            //Arrange
            var userId = "userId";

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<AllReady.Features.Manage.UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            var vm = new IndexViewModel();

            await controller.Index(vm);
            mediator.Verify(m => m.SendAsync(It.Is<AllReady.Features.Manage.UserByUserIdQuery>(u => u.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task IndexPostReturnsCorrectViewWhenModelStateIsInvalid()
        {
            //Arrange
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());
            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser("userId");
            IndexViewModel invalidVm = new IndexViewModel();
            controller.ModelState.AddModelError("FirstName", "Can't be a number");

            //Act
            var result = await controller.Index(invalidVm);

            //Assert
            Assert.IsType(typeof(ViewResult), result);
        }

        [Fact]
        public async Task IndexPostReturnsCorrectViewModelWhenModelStateIsInvalid()
        {
            //Arrange
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());
            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser("userId");
            IndexViewModel invalidVm = new IndexViewModel();
            controller.ModelState.AddModelError("FirstName", "Can't be a number");

            //Act
            var result = await controller.Index(invalidVm);
            var resultViewModel = ((ViewResult)result);
            var vm = (IndexViewModel)resultViewModel.ViewData.Model;

            //Assert
            Assert.IsType<IndexViewModel>(vm);
        }

        [Fact]
        public async Task IndexPostInvokesRemoveClaimsAsyncWithCorrectParametersWhenUsersTimeZoneDoesNotEqualModelsTimeZone()
        {
            //Arrange
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(x => x.RemoveClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>())).ReturnsAsync(IdentityResult.Success);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser { TimeZoneId = "timeZoneId" };
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser("userId");
            var vM = new IndexViewModel { TimeZoneId = "differentTimeZoneId" };

            //Act
            await controller.Index(vM);

            //Assert
            IEnumerable<Claim> claims = controller.User.Claims.Where(c => c.Type == AllReady.Security.ClaimTypes.TimeZoneId).ToList();
            userManagerMock.Verify(x => x.RemoveClaimsAsync(user, claims), Times.Once);
        }

        [Fact]
        public async Task IndexPostInvokesAddClaimAsyncWithCorrectParametersWhenUsersTimeZoneDoesNotEqualModelsTimeZone()
        {
            //Arrange
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser { TimeZoneId = "timeZoneId" };
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser("userId");
            var vM = new IndexViewModel { TimeZoneId = "differentTimeZoneId" };

            //Act
            await controller.Index(vM);

            //Assert
            userManagerMock.Verify(x => x.AddClaimAsync(user, It.Is<Claim>(c=>c.Type == AllReady.Security.ClaimTypes.TimeZoneId)), Times.Once);
        }

        //TODO: come back to finsih these stubs... there is a lot going on in Index Post

        [Fact]
        public void IndexPostHasHttpPostAttrbiute()
        {
            //Arrange
            var controller = new ManageController(null, null, null);
            //Act
            var attribute = controller.GetAttributesOn(x => x.Index(It.IsAny<IndexViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            //Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void IndexPostHasValidateAntiForgeryTokenAttribute()
        {
            var controller = new ManageController(null, null, null);
            //Act
            var attribute = controller.GetAttributesOn(x => x.Index(It.IsAny<IndexViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            //Assert
            Assert.NotNull(attribute);
        }

        [Fact(Skip = "NotImplemented")]
        public async Task UpdateUserProfileCompletenessSendsRemoveUserProfileIncompleteClaimCommandWithCorrectUserIdWhenUsersProfileIsComplete()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task UpdateUserProfileCompletenessInvokesRefreshSignInAsyncWithCorrectUserWhenUsersProfileIsComplete()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResendEmailConfirmationInvokesFindByIdAsyncWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResendEmailConfirmationInvokesGenerateEmailConfirmationTokenAsyncWithCorrectUser()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResendEmailConfirmationInvokesUrlActionWithCorrectParameters()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResendEmailConfirmationSendsSendConfirmAccountEmailAsyncWithCorrectData()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResendEmailConfirmationRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public void ResendEmailConfirmationHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ResendEmailConfirmationHasHttpValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EmailConfirmationSentReturnsAView()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RemoveLoginSendsUserByUserIdQueryWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RemoveLoginInvokesRemoveLoginAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RemoveLoginInvokesSignInAsyncWithCorrectParametersWhenUserIsNotNullAndRemoveLoginSucceeds()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RemoveLoginRedirectsToCorrectActionWithCorrectRouteValues()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public void RemoveLoginHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void RemoveLoginHasValidateAntiForgeryTokenAttribute()
        {
        }

        public async Task EnableTwoFactorAuthenticationSendsUserByUserIdQueryWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EnableTwoFactorAuthenticationInvokesSetTwoFactorEnabledAsyncWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EnableTwoFactorAuthenticationInvokesSignInAsyncWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EnableTwoFactorAuthenticationRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public void EnbaleTwoFactorAuthenticationHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EnableTwoFactorAuthenticationHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DisableTwoFactorAuthenticationSendsUserByUserIdQueryWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DisableTwoFactorAuthenticationInvokesSetTwoFactorEnabledAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DisableTwoFactorAuthenticationInvokesSignInAsyncWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DisableTwoFactorAuthenticationRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public void DisableTwoFactorAuthenticationHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void DisableTwoFactorAuthenticationHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangePasswordGetReturnsAView()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangePasswordGetHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangePasswordPostReturnsSameViewAndModelWhenModelStateIsInvalid()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangePasswordPostSendsUserByUserIdQueryWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangePasswordPostInvokesChangePasswordAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangePasswordPostInvokesSignInAsyncWithCorrectParametersWhenUserIsNotNullAndPasswordWasChangedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangePasswordPostRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNotNullAndPasswordWasChangedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangePasswordPostAddsIdentityResultErrorsToModelStateErrorsWhenUserIsNotNullAndPasswordWasNotChangedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangePasswordPostReturnsCorrectViewModelWhenUserIsNotNullAndPasswordWasNotChangedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangePasswordPostRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangePasswordHPostasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangePasswordPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangeEmailGetReturnsAView()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangeEmailGetHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostReturnsSameViewAndViewModelWhenModelStateIsInvalid()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostSendsUserByUserIdQueryWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostInvokesCheckPasswordAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostAddsCorrectErrorMessageToModelStateWhenChangePasswordIsUnsuccessful()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostReturnsCorrectVieModelWhenChangePasswordIsUnsuccessful()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostInvokesFindByEmailAsyncWithCorrectParametersWhenChangePasswordIsSuccessful()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostAddsCorrectErrorToModelStateWhenChangePasswordIsSuccessfulAndEmailCannotBeFound()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostReturnsCorrectViewModelWhenChangePasswordIsSuccessfulAndEmailCannotBeFound()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostInvokesUpdateAsyncWithCorrectParametersWhenUserIsNotNullAndChangePasswordIsSuccessfulAndUsersEmailIsFound()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostInvokesGenerateChangeEmailTokenAsyncWithCorrectParametersWhenUserIsNotNullAndChangePasswordIsSuccessfulAndUsersEmailIsFound()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostInvokesUrlActioncWithCorrectParametersWhenUserIsNotNullAndChangePasswordIsSuccessfulAndUsersEmailIsFound()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostSendsSendNewEmailAddressConfirmationEmailAsyncWithCorrectDataWhenUserIsNotNullAndChangePasswordIsSuccessfulAndUsersEmailIsFound()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNotNullAndChangePasswordIsSuccessfulAndUsersEmailIsFound()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostRedirectsToTheCorrectActionWithTheCorrectRouteValuesWhenUserIsNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangeEmailPostHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ChangeEmailPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmNewEmailReturnsErrorViewWhenTokenIsNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmNewEmailSendsUserByUserIdQueryWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmNewEmailReturnsErrorViewWhenUserIsNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmNewEmailInvokesChangeEmailAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmNewEmailInvokesSetUserNameAsyncWithCorrectParametersWhenUserIsNotNullAndSettingUserNameIsSuccessful()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmNewEmailInvokesUpdateAsyncWithCorrectParametersWhenUserIsNotNullAndSettingUserNameIsSuccessful()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmNewEmailRedirectsToCorrectActionWithCorrectRouteValues()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public void ConfirmEmailHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResendChangeEmailConfirmationSendsUserByUserIdQueryWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResendChangeEmailConfirmationReturnsErrorViewWhenUsersPendingNewEmailIsNullOrEmpty()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResendChangesEmailConfirmationInvokesGenerateChangeEmailTokenAsyncWithCorrectParameters()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResendChangesEmailConfirmationInvokesUrlActionWithCorrectParameters()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        //public async Task ResendChangesEmailConfirmationInvokesSendEmailAsyncWithCorrectParameters()
        public async Task ResendChangesEmailConfirmationSendsSendNewEmailAddressConfirmationEmailAsyncWithCorrectData()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResendChangesEmailConfirmationRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public void ResendChangesEmailConfirmationHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ResendChangesEmailConfirmationHasVAlidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task CancelChangeEmailSendsUserByUserIdQueryWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task CancelChangeEmailInvokesUpdateAsyncWithCorrectParameters()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task CancelChangeEmailRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public void CancelChangeEmailHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void CancelChangeEmailHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void SetPasswordGetReturnsAView()
        {   
        }

        [Fact(Skip = "NotImplemented")]
        public void SetPasswordGetHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SetPasswordPostReturnsSameViewAndViewModelWhenModelStateIsInvalid()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SetPasswordPostSendsUserByUserIdQueryWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SetPasswordPostInvokesAddPasswordAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SetPasswordPostInvokesSignInAsyncWithCorrectParametersWhenUserIsNotNullAndPasswordAddedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SetPasswordPostRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNotNullAndPasswordAddedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SetPasswordPostAddsCorrectErrorMessageToModelStateWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SetPasswordPostReturnsCorrectViewModelWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task SetPasswordPostRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public void SetPasswordPostHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void SetPasswordPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ManageLoginsAddsCorrectMessageToViewDataWhenMessageIdIsRemoveLoginSuccess()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ManageLoginsAddsCorrectMessageToViewDataWhenMessageIdIsAddLoginSuccess()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ManageLoginsAddsCorrectMessageToViewDataWhenMessageIdIsError()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ManageLoginsSendsUserByUserIdQueryWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ManageLoginsReturnsErrorViewWhenUserIsNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ManageLoginsInvokesGetLoginsAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ManageLoginsInvokesGetExternalAuthenticationSchemesWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ManageLoginsAddsCorrectValueToSHOW_REMOVE_BUTTONWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ManageLoginsReturnsCorrectViewModelWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public void ManageLoginsHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void LinkLoginInvokesUrlActionWithTheCorrectParameters()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void LinkLoginInvokesConfigureExternalAuthenticationPropertiesWithCorrectParameters()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void LinkLoginReturnsCorrectResult()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void LinkLoginHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void LinkLoginHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task LinkLoginCallbackSendsUserByUserIdQueryWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task LinkLoginCallbackReturnsErrorViewWhenUserIsNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task LinkLoginCallbackInvokesGetExternalLoginInfoAsyncWithCorrectUserIdWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task LinkLoginCallbackRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNotNullAndExternalLoginInfoIsNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task LinkLoginCallbackInvokesAddLoginAsyncWithCorrectParametersWhenUserIsNotNullAndExternalLoginInfoIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task LinkLoginCallbackRedirectsToCorrectActionWithCorrectRouteParametersWhenUserIsNotNullAndExternalLoginInfoIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ControllerHasAuthorizeAtttribute()
        {
            //delete this line when starting work on this unit test
            await TaskCompletedTask;
        }
    }
}