using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using AllReady.Extensions;
using Microsoft.AspNetCore.Identity;
using AllReady.Features.Manage;
using AllReady.Features.Login;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AllReady.UnitTest.Controllers
{
    public class ManageControllerTests
    {
        //delete this line when all unit tests using it have been completed
        private static readonly Task TaskCompletedTask = Task.CompletedTask;

        [Fact]
        public async Task IndexGetAddsCorrectMessageToViewDataWhenMessageEqualsChangePasswordSuccess()
        {           
            ManageController controller = InitializeControllerWithValidUser(new ApplicationUser()).controller;
            controller.SetFakeUser("userId");
          
            var result = await controller.Index(ManageMessageId.ChangePasswordSuccess);
            var resultViewModel = ((ViewResult)result);
            var message = resultViewModel.ViewData["StatusMessage"].ToString();
         
            Assert.Equal("Your password has been changed.", message);
        }

        [Fact]
        public async Task IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsSetPasswordSuccess()
        {
            ManageController controller = InitializeControllerWithValidUser(new ApplicationUser()).controller;
            controller.SetFakeUser("userId");
            
            var result = await controller.Index(ManageMessageId.SetPasswordSuccess);
            var resultViewModel = ((ViewResult)result);
            var message = resultViewModel.ViewData["StatusMessage"].ToString();
           
            Assert.Equal("Your password has been set.", message);
        }

        [Fact]
        public async Task IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsSetTwoFactorSuccess()
        {
            ManageController controller = InitializeControllerWithValidUser(new ApplicationUser()).controller;
            controller.SetFakeUser("userId");
            
            var result = await controller.Index(ManageMessageId.SetTwoFactorSuccess);
            var resultViewModel = ((ViewResult)result);
            var message = resultViewModel.ViewData["StatusMessage"].ToString();
          
            Assert.Equal("Your two-factor authentication provider has been set.", message);
        }

        [Fact]
        public async Task IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsError()
        {
            ManageController controller = InitializeControllerWithValidUser(new ApplicationUser()).controller;
            controller.SetFakeUser("userId");
           
            var result = await controller.Index(ManageMessageId.Error);
            var resultViewModel = ((ViewResult)result);
            var message = resultViewModel.ViewData["StatusMessage"].ToString();
           
            Assert.Equal("An error has occurred.", message);
        }

        [Fact]
        public async Task IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsAddPhoneSuccess()
        {
            ManageController controller = InitializeControllerWithValidUser(new ApplicationUser()).controller;
            controller.SetFakeUser("userId");
           
            var result = await controller.Index(ManageMessageId.AddPhoneSuccess);
            var resultViewModel = ((ViewResult)result);
            var message = resultViewModel.ViewData["StatusMessage"].ToString();
            
            Assert.Equal("Your mobile phone number was added.", message);
        }

        [Fact]
        public async Task IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsRemovePhoneSuccess()
        {
            ManageController controller = InitializeControllerWithValidUser(new ApplicationUser()).controller;
            controller.SetFakeUser("userId");
          
            var result = await controller.Index(ManageMessageId.RemovePhoneSuccess);
            var resultViewModel = ((ViewResult)result);
            var message = resultViewModel.ViewData["StatusMessage"].ToString();
            
            Assert.Equal("Your mobile phone number was removed.", message);
        }

        [Fact]
        public async Task IndexGetSendsUserByUserIdQueryWithCorrectUserId()
        {
            var userId = "userId";
            var controllerAndMocks = InitializeControllerWithValidUser(new ApplicationUser{Id = userId});
            ManageController controller = controllerAndMocks.controller;
            controller.SetFakeUser(userId);

            await controller.Index();
            controllerAndMocks.mediatorMock.Verify(m => m.SendAsync(It.Is<AllReady.Features.Manage.UserByUserIdQuery>(u => u.UserId == userId)),Times.Once);
        }

        [Fact]
        public async Task IndexGetReturnsCorrectView()
        {
            ManageController controller = InitializeControllerWithValidUser(new ApplicationUser()).controller;
            controller.SetFakeUser("userId");
            
            var result = await controller.Index();
            
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void IndexGetHasHttpGetAttribute()
        {
            CheckManageControllerMethodAttribute<HttpGetAttribute>(nameof(ManageController.Index), new [] {typeof(ManageMessageId) });
        }

        [Fact]
        public async Task IndexGetReturnsCorrectViewModel()
        {
            ManageController controller = InitializeControllerWithValidUser(new ApplicationUser()).controller;
            controller.SetFakeUser("userId");
           
            var result = await controller.Index(ManageMessageId.RemovePhoneSuccess);
            var resultViewModel = ((ViewResult)result);
            var vm = (IndexViewModel)resultViewModel.ViewData.Model;
           
            Assert.IsType<IndexViewModel>(vm);
        }

        [Fact]
        public async Task IndexPostSendsUserByUserIdQueryWithCorrectUserId()
        {           
            var userId = "userId";

            var controllerAndMocks = InitializeControllerWithValidUser(new ApplicationUser { Id = userId });
            ManageController controller = controllerAndMocks.controller;
            controller.SetFakeUser(userId);

            var vm = new IndexViewModel();

            await controller.Index(vm);
            controllerAndMocks.mediatorMock.Verify(m => m.SendAsync(It.Is<AllReady.Features.Manage.UserByUserIdQuery>(u => u.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task IndexPostReturnsCorrectViewWhenModelStateIsInvalid()
        {
            ManageController controller = InitializeControllerWithValidUser(new ApplicationUser()).controller;
            controller.SetFakeUser("userId");
            IndexViewModel invalidVm = new IndexViewModel();
            controller.ModelState.AddModelError("FirstName", "Can't be a number");
            
            var result = await controller.Index(invalidVm);
          
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task IndexPostReturnsCorrectViewModelWhenModelStateIsInvalid()
        {
            ManageController controller = InitializeControllerWithValidUser(new ApplicationUser()).controller;
            controller.SetFakeUser("userId");
            IndexViewModel invalidVm = new IndexViewModel();
            controller.ModelState.AddModelError("FirstName", "Can't be a number");
           
            var result = await controller.Index(invalidVm);
            var resultViewModel = ((ViewResult)result);
            var vm = (IndexViewModel)resultViewModel.ViewData.Model;
            
            Assert.IsType<IndexViewModel>(vm);
        }

        [Fact]
        public async Task IndexPostInvokesRemoveClaimsAsyncWithCorrectParametersWhenUsersTimeZoneDoesNotEqualModelsTimeZone()
        {
            var user = new ApplicationUser { TimeZoneId = "timeZoneId" };
            var controllerAndMocks = InitializeControllerWithValidUser(user);
            ManageController controller = controllerAndMocks.controller;
            controller.SetFakeUser("userId");
            controllerAndMocks.userManagerMock.Setup(x => x.RemoveClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>())).ReturnsAsync(IdentityResult.Success);

            var vM = new IndexViewModel { TimeZoneId = "differentTimeZoneId" };
           
            await controller.Index(vM);
         
            IEnumerable<Claim> claims = controller.User.Claims.Where(c => c.Type == AllReady.Security.ClaimTypes.TimeZoneId).ToList();
            controllerAndMocks.userManagerMock.Verify(x => x.RemoveClaimsAsync(user, claims), Times.Once);
        }

        [Fact]
        public async Task IndexPostInvokesAddClaimAsyncWithCorrectParametersWhenUsersTimeZoneDoesNotEqualModelsTimeZone()
        {            
            var user = new ApplicationUser { TimeZoneId = "timeZoneId" };
            var controllerAndMocks = InitializeControllerWithValidUser(user);
            ManageController controller = controllerAndMocks.controller;
            controller.SetFakeUser("userId");
            controllerAndMocks.userManagerMock.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var vM = new IndexViewModel { TimeZoneId = "differentTimeZoneId" };
            
            await controller.Index(vM);
           
            controllerAndMocks.userManagerMock.Verify(x => x.AddClaimAsync(user, It.Is<Claim>(c=>c.Type == AllReady.Security.ClaimTypes.TimeZoneId)), Times.Once);
        }

        //TODO: come back to finsih these stubs... there is a lot going on in Index Post

        [Fact]
        public void IndexPostHasHttpPostAttribute()
        {
            CheckManageControllerMethodAttribute<HttpPostAttribute>(nameof(ManageController.Index), new [] {typeof(IndexViewModel) });
        }

        [Fact]
        public void IndexPostHasValidateAntiForgeryTokenAttribute()
        {
            CheckManageControllerMethodAttribute<ValidateAntiForgeryTokenAttribute>(nameof(ManageController.Index), new[] { typeof(IndexViewModel) });
        }
        [Fact]
        public async Task IndexPostSendsRemoveUserProfileIncompleteClaimCommandWithCorrectUserIdWhenUsersProfileIsComplete()
        {
            ApplicationUser user = new ApplicationUser
            {
                Id = "Some UserID",
                Email = "email@company.com",
                FirstName = "Name",
                LastName = "Last Name",
                PhoneNumber = "01234567890",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                TimeZoneId = "TimeZonedID",
            };


            var controllerAndMocks = InitializeControllerWithValidUser(user);
            controllerAndMocks.signInManagerMock.Setup(m => m.RefreshSignInAsync(It.IsAny<ApplicationUser>())).Returns(Task.FromResult(user));
            controllerAndMocks.controller.SetFakeUser(user.Id);
            
            var viewModel = new IndexViewModel { FirstName = "Name", LastName = "Last Name", TimeZoneId = "TimeZonedID"};

            await controllerAndMocks.controller.Index(viewModel);

            controllerAndMocks.mediatorMock.Verify(m => m.SendAsync(It.Is<RemoveUserProfileIncompleteClaimCommand>(u => u.UserId == user.Id)), Times.Once);
        }

        [Fact]
        public async Task UpdateUserProfileCompletenessInvokesRefreshSignInAsyncWithCorrectUserWhenUsersProfileIsComplete()
        {
            //Set properties of user required for ApplicationUser.IsProfileComplete() to return true
            ApplicationUser user = new ApplicationUser
            {
                Id = "Some UserID",
                Email = "email@company.com",
                FirstName = "Name",
                LastName = "Last Name",
                PhoneNumber = "01234567890",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                TimeZoneId = "TimeZonedID"
            };

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(m => m.RefreshSignInAsync(It.IsAny<ApplicationUser>())).Returns(Task.FromResult(user));

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var manageController = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            manageController.SetFakeUser(user.Id);

            //Only set props required for modelstate to be valid.
            IndexViewModel viewModel = new IndexViewModel { FirstName = "Name", LastName = "Last Name", TimeZoneId = "TimeZonedID" };

            await manageController.Index(viewModel);

            signInManagerMock.Verify(s=>s.RefreshSignInAsync(It.Is<ApplicationUser>(u=>u == user)),Times.AtLeastOnce);
        }

        [Fact]
        public async Task ResendEmailConfirmationInvokesGetUserAsyncWithCorrectUserId()
        {
            ApplicationUser user = new ApplicationUser { Id = "MyUserID" };

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var mediator = new Mock<IMediator>();

            ManageController controller = new ManageController(userManagerMock.Object, null, mediator.Object);
            controller.SetFakeIUrlHelper();
            controller.SetFakeUser(user.Id);

            await controller.ResendEmailConfirmation();

            userManagerMock.Verify(u => u.GetUserAsync(controller.User), Times.Once);

        }

        [Fact]
        public async Task ResendEmailConfirmationInvokesGenerateEmailConfirmationTokenAsyncWithCorrectUser()
        {
            ApplicationUser user = new ApplicationUser { Id = "MyUserID" };
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            userManagerMock.Setup(u => u.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("token");

            var mediator = new Mock<IMediator>();

            ManageController controller = new ManageController(userManagerMock.Object, null, mediator.Object);
            controller.SetFakeIUrlHelper();
            controller.SetFakeUser(user.Id);

            await controller.ResendEmailConfirmation();

            userManagerMock.Verify(u => u.GenerateEmailConfirmationTokenAsync(It.Is<ApplicationUser>(i => i == user)), Times.Once);
        }

        [Fact]
        public async Task ResendEmailConfirmationInvokesUrlActionWithCorrectParameters()
        {
            ApplicationUser user = new ApplicationUser { Id = "MyUserID" };
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            const string code = "token";
            userManagerMock.Setup(u => u.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(code);

            var controller = new ManageController(userManagerMock.Object, null, Mock.Of<IMediator>());
            controller.SetFakeUser(user.Id);
            var urlMock = new Mock<IUrlHelper>();
            controller.Url = urlMock.Object;
            urlMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("callbackUrl");

            controller.SetFakeHttpRequestSchemeTo("scheme");

            await controller.ResendEmailConfirmation();

            var urlcontext = new UrlActionContext
            {
                Action = nameof(AccountController.ConfirmEmail),
                Controller = "Account",
                Values = new { userId = user.Id, code }
            };

            urlMock.Verify(a => a.Action(It.Is<UrlActionContext>(i => i.Action == urlcontext.Action && i.Controller == urlcontext.Controller && i.Values.ToString() == $"{{ userId = {user.Id}, token = token }}" && i.Protocol == controller.HttpContext.Request
            .Scheme)),Times.Once);
        }

        [Fact]
        public async Task ResendEmailConfirmationSendsSendConfirmAccountEmailAsyncWithCorrectData()
        {
            const string callBackUrl = "callbackUrl";

            ApplicationUser user = new ApplicationUser { Id = "MyUserID",Email="me@email.com" };
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            userManagerMock.Setup(u => u.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("");

            var mediator = new Mock<IMediator>();

            ManageController controller = new ManageController(userManagerMock.Object, null, mediator.Object);
            controller.SetFakeUser(user.Id);
            var urlMock = new Mock<IUrlHelper>();
            controller.Url = urlMock.Object;
            urlMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns(callBackUrl);

            await controller.ResendEmailConfirmation();

            mediator.Verify(m => m.SendAsync(It.Is<SendConfirmAccountEmail>(e => e.Email == user.Email && e.CallbackUrl == callBackUrl)));
        }

        [Fact]
        public async Task ResendEmailConfirmationRedirectsToCorrectAction()
        {
            var user = new ApplicationUser { Id = "MyUserID", Email = "me@email.com" };
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            userManagerMock.Setup(u => u.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("");

            var mediator = new Mock<IMediator>();

            var controller = new ManageController(userManagerMock.Object, null, mediator.Object);
            controller.SetFakeUser(user.Id);
            var urlMock = new Mock<IUrlHelper>();
            controller.Url = urlMock.Object;
            urlMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("callbackUrl");

            IActionResult actionResult = await controller.ResendEmailConfirmation();

            CheckRedirectionToAction(actionResult, nameof(ManageController.EmailConfirmationSent));
        }

        [Fact]
        public void ResendEmailConfirmationHasHttpPostAttribute()
        {
            CheckManageControllerMethodAttribute<HttpPostAttribute>(nameof(ManageController.ResendEmailConfirmation), new Type[] { });
        }

        [Fact]
        public void ResendEmailConfirmationHasHttpValidateAntiForgeryTokenAttribute()
        {
            CheckManageControllerMethodAttribute<ValidateAntiForgeryTokenAttribute>(nameof(ManageController.ResendEmailConfirmation), new Type[] { });
        }

        [Fact]
        public void EmailConfirmationSentReturnsAView()
        {
            var controller = new ManageController(null, null, null);

            var result = controller.EmailConfirmationSent();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task RemoveLoginSendsUserByUserIdQueryWithCorrectUserId()
        {
            const string UserId = "userID";
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(UserId);
            userManagerMock.Setup(u => u.GetLoginsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<UserLoginInfo>());

            var mediatorMock = new Mock<IMediator>();
            
            var controller = new ManageController(userManagerMock.Object, null, mediatorMock.Object);

            await controller.RemoveLogin();

            mediatorMock.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(u => u.UserId == UserId)),Times.Once);
        }

        [Fact]
        public async Task RemoveLoginInvokesRemoveLoginAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            const string loginProvider = "loginProvider";
            const string providerKey = "providerKey";

            var user = new ApplicationUser ();

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.RemoveLoginAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new IdentityResult());

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManagerMock.Object, null, mediatorMock.Object);

            await controller.RemoveLogin(loginProvider, providerKey);

            userManagerMock.Verify(u => u.RemoveLoginAsync(user, loginProvider, providerKey));
        }

        [Fact]
        public async Task RemoveLoginInvokesSignInAsyncWithCorrectParametersWhenUserIsNotNullAndRemoveLoginSucceeds()
        {
            const string unusedLoginProvider = "loginProvider";
            const string unusedProviderKey = "providerKey";

            var user = new ApplicationUser();

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.RemoveLoginAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediatorMock.Object);

            await controller.RemoveLogin(unusedLoginProvider, unusedProviderKey);

            signInManagerMock.Verify(s => s.SignInAsync(user, false, It.IsAny<string>()));
        }

        [Fact]
        public async Task RemoveLoginRedirectsToCorrectActionWithRemoveLoginSuccessMessageRouteValueWhenUserIsNotNullAndRemoveLoginSucceeds()
        {
            const string unusedLoginProvider = "loginProvider";
            const string unusedProviderKey = "providerKey";

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.RemoveLoginAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediatorMock.Object);

            IActionResult actionResult = await controller.RemoveLogin(unusedLoginProvider, unusedProviderKey);

            CheckRedirectionToActionWithMessageRouteValue(actionResult, nameof(ManageController.ManageLogins), ManageMessageId.RemoveLoginSuccess);
        }

        [Fact]
        public async Task RemoveLoginRedirectsToCorrectActionWithErrorMessageRouteValueWhenUserIsNull()
        {
            const string unusedLoginProvider = "loginProvider";
            const string unusedProviderKey = "providerKey";

            ManageController controller = InitializeControllerWithNullUser();

            IActionResult actionResult = await controller.RemoveLogin(unusedLoginProvider, unusedProviderKey);

            CheckRedirectionToActionWithMessageRouteValue(actionResult, nameof(ManageController.ManageLogins), ManageMessageId.Error);
        }

        private static void CheckRedirectionToActionWithMessageRouteValue(IActionResult actionResult, string expectedActionName, ManageMessageId expectedMessageRouteValue)
        {
            var result = CheckRedirectionToAction(actionResult, expectedActionName);
            Assert.Equal(expectedMessageRouteValue, result.RouteValues["Message"]);
        }

        private static RedirectToActionResult CheckRedirectionToAction(IActionResult actionResult, string expectedActionName)
        {
            var result = (RedirectToActionResult) actionResult;
            Assert.NotNull(result);
            Assert.Equal(expectedActionName, result.ActionName);
            return result;
        }

        [Fact]
        public async Task RemoveLoginRedirectsToCorrectActionWithErrorMessageRouteValueWhenUserIsNotNullAndRemoveLoginFails()
        {
            const string unusedLoginProvider = "loginProvider";
            const string unusedProviderKey = "providerKey";

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.RemoveLoginAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, null, mediatorMock.Object);

            IActionResult actionResult = await controller.RemoveLogin(unusedLoginProvider, unusedProviderKey);

            CheckRedirectionToActionWithMessageRouteValue(actionResult, nameof(ManageController.ManageLogins), ManageMessageId.Error);
        }

        [Fact]
        public void RemoveLoginHasHttpPostAttribute()
        {
            CheckManageControllerMethodAttribute<HttpPostAttribute>(nameof(ManageController.RemoveLogin), new [] { typeof(string), typeof(string)});
        }

        [Fact]
        public void RemoveLoginHasValidateAntiForgeryTokenAttribute()
        {
            CheckManageControllerMethodAttribute<ValidateAntiForgeryTokenAttribute>(nameof(ManageController.RemoveLogin), new[] { typeof(string), typeof(string) });
        }

        [Fact]
        public async Task EnableTwoFactorAuthenticationSendsUserByUserIdQueryWithCorrectUserId()
        {
            const string UserId = "userID";
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(U => U.SetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>(), true)).ReturnsAsync(new IdentityResult());
            userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(UserId);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null)).Returns(Task.FromResult(new ApplicationUser { Id = UserId }));

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser { Id = UserId });

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(UserId);

            await controller.EnableTwoFactorAuthentication();

            mediator.Verify(u => u.SendAsync(It.Is<UserByUserIdQuery>(i => i.UserId == UserId)), Times.Once);
        }

        [Fact]
        public async Task EnableTwoFactorAuthenticationInvokesSetTwoFactorEnabledAsyncWhenUserIsNotNull()
        {
            const string userId = "userID";
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(U => U.SetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>(), true)).ReturnsAsync(new IdentityResult());

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null)).Returns(Task.FromResult(new ApplicationUser { Id = userId }));

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser { Id= userId });

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            await controller.EnableTwoFactorAuthentication();

            userManagerMock.Verify(u => u.SetTwoFactorEnabledAsync(It.Is<ApplicationUser>(i => i.Id == userId),true), Times.Once);
        }

        [Fact]
        public async Task EnableTwoFactorAuthenticationInvokesSignInAsyncWhenUserIsNotNull()
        {
            const string userId = "userID";
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(U => U.SetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>(), true)).ReturnsAsync(new IdentityResult()); ;

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null)).Returns(Task.FromResult(new ApplicationUser { Id = userId }));

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser { Id = userId });

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            await controller.EnableTwoFactorAuthentication();

            signInManagerMock.Verify(s => s.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(),null), Times.Once);
        }

        [Fact]
        public async Task EnableTwoFactorAuthenticationRedirectsToCorrectAction()
        {
            const string userId = "userID";

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(U => U.SetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>(), true)).ReturnsAsync(new IdentityResult()); ;

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null)).Returns(Task.FromResult(new ApplicationUser { Id = userId}));

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser { Id = userId });

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            IActionResult actionResult = await controller.EnableTwoFactorAuthentication();

            CheckRedirectionToAction(actionResult, nameof(ManageController.Index));
        }

        [Fact]
        public void EnbaleTwoFactorAuthenticationHasHttpPostAttribute()
        {
            CheckManageControllerMethodAttribute<HttpPostAttribute>(nameof(ManageController.EnableTwoFactorAuthentication), new Type[] { });
        }

        [Fact]
        public void EnableTwoFactorAuthenticationHasValidateAntiForgeryTokenAttribute()
        {
            CheckManageControllerMethodAttribute<ValidateAntiForgeryTokenAttribute>(nameof(ManageController.EnableTwoFactorAuthentication), new Type[] { });
        }

        [Fact]
        public async Task DisableTwoFactorAuthenticationSendsUserByUserIdQueryWithCorrectUserId()
        {
            const string userId = "UserID";
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(U => U.SetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>(), true)).ReturnsAsync(new IdentityResult());
            userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null)).Returns(Task.FromResult(new ApplicationUser { Id = userId }));

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser { Id = userId });

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);
      
            await controller.DisableTwoFactorAuthentication();

            mediator.Verify(u => u.SendAsync(It.Is<UserByUserIdQuery>(i => i.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task DisableTwoFactorAuthenticationInvokesSetTwoFactorEnabledAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            const string userId = "UserID";

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(U => U.SetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>(), false)).ReturnsAsync(new IdentityResult());

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null)).Returns(Task.FromResult(new ApplicationUser { Id = userId }));

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser { Id = userId });

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            await controller.DisableTwoFactorAuthentication();

            userManagerMock.Verify(u => u.SetTwoFactorEnabledAsync(It.Is<ApplicationUser>(i => i.Id == userId), false), Times.Once);
        }

        [Fact]
        public async Task DisableTwoFactorAuthenticationInvokesSignInAsyncWhenUserIsNotNull()
        {
            const string userId = "userID";

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(U => U.SetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>(), false)).ReturnsAsync(new IdentityResult()); ;

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null)).Returns(Task.FromResult(new ApplicationUser { Id = userId }));

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser { Id = userId });

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            await controller.DisableTwoFactorAuthentication();

            signInManagerMock.Verify(s => s.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null), Times.Once);
        }

        [Fact]
        public async Task DisableTwoFactorAuthenticationRedirectsToCorrectAction()
        {
            const string userId = "userID";
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(U => U.SetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>(), false)).ReturnsAsync(new IdentityResult()); ;

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null)).Returns(Task.FromResult(new ApplicationUser { Id = userId }));

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser { Id = userId });

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            IActionResult actionResult = await controller.DisableTwoFactorAuthentication();

            CheckRedirectionToAction(actionResult, nameof(ManageController.Index));
        }

        [Fact]
        public void DisableTwoFactorAuthenticationHasHttpPostAttribute()
        {
            CheckManageControllerMethodAttribute<HttpPostAttribute>(nameof(ManageController.DisableTwoFactorAuthentication), new Type[] { });
        }

        [Fact]
        public void DisableTwoFactorAuthenticationHasValidateAntiForgeryTokenAttribute()
        {
            CheckManageControllerMethodAttribute<ValidateAntiForgeryTokenAttribute>(nameof(ManageController.DisableTwoFactorAuthentication), new Type[] { });
        }

        [Fact]
        public void ChangePasswordGetReturnsAView()
        {
            var controller = new ManageController(null, null, null);

            var result = controller.ChangePassword();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ChangePasswordGetHasHttpGetAttribute()
        {
            CheckManageControllerMethodAttribute<HttpGetAttribute>(nameof(ManageController.ChangePassword), new Type[] { });
        }

        [Fact]
        public async Task ChangePasswordPostReturnsSameViewAndModelWhenModelStateIsInvalid()
        {
            const string oldPassword = "password";

            var controller = new ManageController(null, null, null);
            controller.ModelState.AddModelError("error", "error msg");
            
            var changePasswordViewModel = new ChangePasswordViewModel{OldPassword = oldPassword};

            var result = await controller.ChangePassword(changePasswordViewModel);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            var resultViewModel = viewResult.ViewData.Model;
            var resultChangePasswordViewModel = resultViewModel as ChangePasswordViewModel;
            Assert.NotNull(resultChangePasswordViewModel);
            Assert.Equal(oldPassword, resultChangePasswordViewModel.OldPassword);
        }

        [Fact]
        public async Task ChangePasswordPostSendsUserByUserIdQueryWithCorrectUserId()
        {
            const string userId = "UserID";
            var validVm = new ChangePasswordViewModel { OldPassword = "oldPassword", NewPassword = "newPassword" };

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new IdentityResult());
            userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            signInManagerMock.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null)).Returns(Task.FromResult(new ApplicationUser { Id = userId }));

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser { Id = userId });


            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            await controller.ChangePassword(validVm);

            mediator.Verify(u => u.SendAsync(It.Is<UserByUserIdQuery>(i => i.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordPostInvokesChangePasswordAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            var validVm = new ChangePasswordViewModel { OldPassword = "oldPassword", NewPassword = "newPassword" };

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new IdentityResult());
            userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userID");

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            ApplicationUser user = new ApplicationUser { Id = "userID" };
            signInManagerMock.Setup(s => s.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), null)).Returns(Task.FromResult(user));

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser("userID");

            await controller.ChangePassword(validVm);

            userManagerMock.Verify(u => u.ChangePasswordAsync(It.Is<ApplicationUser>(usr => usr == user), validVm.OldPassword, validVm.NewPassword));
        }

        private static async Task<(Mock<SignInManager<ApplicationUser>> SignInManagerMock, IActionResult Result)> ChangePasswordSuccessfully(ApplicationUser user)
        {
            var validVm = new ChangePasswordViewModel { OldPassword = "oldPassword", NewPassword = "newPassword" };

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user.Id);
            userManagerMock.Setup(u => u.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);

            var result = await controller.ChangePassword(validVm);
            return (signInManagerMock, result);
        }

        [Fact]
        public async Task ChangePasswordPostInvokesSignInAsyncWithCorrectParametersWhenUserIsNotNullAndPasswordWasChangedSuccessfully()
        {
            const string userId = "userID";
            ApplicationUser user = new ApplicationUser { Id = userId };

            var changePasswordResult = await ChangePasswordSuccessfully(user);

            changePasswordResult.SignInManagerMock.Verify(s => s.SignInAsync(user, false, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordPostRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNotNullAndPasswordWasChangedSuccessfully()
        {
            const string userId = "userID";
            ApplicationUser user = new ApplicationUser { Id = userId };

            var changePasswordResult = await ChangePasswordSuccessfully(user);
            
            CheckRedirectionToActionWithMessageRouteValue(changePasswordResult.Result, nameof(ManageController.Index), ManageMessageId.ChangePasswordSuccess);
        }

        private static async Task<(ManageController Controller, IActionResult Result)> ChangePasswordUnsuccessfully(IdentityResult identityResult, ChangePasswordViewModel changePasswordViewModel)
        {
            ApplicationUser user = new ApplicationUser { Id = "userID" };

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user.Id);
            userManagerMock.Setup(u => u.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(identityResult);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManagerMock.Object, null, mediator.Object);

            var result = await controller.ChangePassword(changePasswordViewModel);
            return (controller, result);
        }

        [Fact]
        public async Task ChangePasswordPostAddsIdentityResultErrorsToModelStateErrorsWhenUserIsNotNullAndPasswordWasNotChangedSuccessfully()
        {
            var identityResult = IdentityResult.Failed(new IdentityError { Description = "ChangePasswordFailureDescription" });
            ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel();

            ManageController controller = (await ChangePasswordUnsuccessfully(identityResult, changePasswordViewModel)).Controller;

            var errorMessages = controller.ModelState.GetErrorMessages();
            Assert.Equal(1, errorMessages.Count);
            Assert.Equal(identityResult.Errors.Select(x => x.Description).Single(), errorMessages.Single());
        }

        [Fact]
        public async Task ChangePasswordPostReturnsCorrectViewModelWhenUserIsNotNullAndPasswordWasNotChangedSuccessfully()
        {
            var identityResult = IdentityResult.Failed(new IdentityError { Description = "ChangePasswordFailureDescription" });
            ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel();

            IActionResult result = (await ChangePasswordUnsuccessfully(identityResult, changePasswordViewModel)).Result;
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            var resultViewModel = viewResult.ViewData.Model;
            var resultChangePasswordViewModel = resultViewModel as ChangePasswordViewModel;
            Assert.NotNull(resultChangePasswordViewModel);
            Assert.Equal(changePasswordViewModel, resultChangePasswordViewModel);
        }

        [Fact]
        public async Task ChangePasswordPostRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNull()
        {
            ManageController controller = InitializeControllerWithNullUser();

            IActionResult actionResult = await controller.ChangePassword(new ChangePasswordViewModel());

            CheckRedirectionToActionWithMessageRouteValue(actionResult, nameof(ManageController.Index), ManageMessageId.Error);
        }

        [Fact]
        public void ChangePasswordPostHasHttpPostAttribute()
        {
            CheckManageControllerMethodAttribute<HttpPostAttribute>(nameof(ManageController.ChangePassword), new[] {typeof(ChangePasswordViewModel)});
        }

        [Fact]
        public void ChangePasswordPostHasValidateAntiForgeryTokenAttribute()
        {
            CheckManageControllerMethodAttribute<ValidateAntiForgeryTokenAttribute>(nameof(ManageController.ChangePassword), new[] { typeof(ChangePasswordViewModel) });
        }

        [Fact]
        public void ChangeEmailGetReturnsAView()
        {
            var controller = new ManageController(null, null, null);

            var result = controller.ChangeEmail();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ChangeEmailGetHasHttpGetAttribute()
        {
            CheckManageControllerMethodAttribute<HttpGetAttribute>(nameof(ManageController.ChangeEmail), new Type[] { });
        }

        [Fact]
        public async Task ChangeEmailPostReturnsSameViewAndViewModelWhenModelStateIsInvalid()
        {
            const string newMail = "email";

            var controller = new ManageController(null, null, null);
            controller.ModelState.AddModelError("error", "error msg");

            var changeEmailViewModel = new ChangeEmailViewModel() { NewEmail = newMail };

            var result = await controller.ChangeEmail(changeEmailViewModel);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            var resultViewModel = viewResult.ViewData.Model;
            var resultChangeEmailViewModel = resultViewModel as ChangeEmailViewModel;
            Assert.NotNull(resultChangeEmailViewModel);
            Assert.Equal(newMail, resultChangeEmailViewModel.NewEmail);
        }

        [Fact]
        public async Task ChangeEmailPostSendsUserByUserIdQueryWithCorrectUserId()
        {
            const string userId = "UserID";
            var validVm = new ChangeEmailViewModel();

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser { Id = userId });
            
            var controller = new ManageController(userManagerMock.Object, null, mediator.Object);
           
            await controller.ChangeEmail(validVm);

            mediator.Verify(u => u.SendAsync(It.Is<UserByUserIdQuery>(i => i.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task ChangeEmailPostInvokesCheckPasswordAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            const string userId = "UserID";
            const string password = "password";

            var user = new ApplicationUser { Id = userId };
            
            var validVm = new ChangeEmailViewModel{Password = password };

            var controllerAndMocks = InitializeControllerWithValidUser(user);

            await controllerAndMocks.controller.ChangeEmail(validVm);

            controllerAndMocks.userManagerMock.Verify(u => u.CheckPasswordAsync(user, password), Times.Once);
        }

        [Fact]
        public async Task ChangeEmailPostAddsCorrectErrorMessageToModelStateWhenCheckPasswordIsUnsuccessful()
        {
            const string userId = "UserID";
            const string password = "password";

            var user = new ApplicationUser { Id = userId };

            var validVm = new ChangeEmailViewModel { Password = password };

            var controllerAndMocks = InitializeControllerWithValidUser(user);

            controllerAndMocks.userManagerMock.Setup(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(false);

            await controllerAndMocks.controller.ChangeEmail(validVm);

            var errorMessages = controllerAndMocks.controller.ModelState.GetErrorMessagesByKey(nameof(ChangeEmailViewModel.Password));
            Assert.Equal(1, errorMessages.Count);
            Assert.NotNull(errorMessages.Single());
        }

        [Fact]
        public async Task ChangeEmailPostReturnsCorrectViewModelWhenCheckPasswordIsUnsuccessful()
        {
            const string userId = "UserID";
            const string password = "password";

            var user = new ApplicationUser { Id = userId };

            var validVm = new ChangeEmailViewModel { Password = password };

            var controllerAndMocks = InitializeControllerWithValidUser(user);
            controllerAndMocks.userManagerMock.Setup(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await controllerAndMocks.controller.ChangeEmail(validVm);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            var resultViewModel = viewResult.ViewData.Model;
            var resultChangeEmailViewModel = resultViewModel as ChangeEmailViewModel;
            Assert.NotNull(resultChangeEmailViewModel);
            Assert.Equal(password, resultChangeEmailViewModel.Password);
        }

        [Fact]
        public async Task ChangeEmailPostInvokesFindByEmailAsyncWithCorrectParametersWhenCheckPasswordIsSuccessful()
        {
            const string userId = "UserID";
            const string email = "newEmail";

            var user = new ApplicationUser { Id = userId };

            var validVm = new ChangeEmailViewModel { NewEmail = email };

            var controllerAndMocks = InitializeControllerWithValidUser(user);
            controllerAndMocks.userManagerMock.Setup(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
            controllerAndMocks.userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

            await controllerAndMocks.controller.ChangeEmail(validVm);

            controllerAndMocks.userManagerMock.Verify(u => u.FindByEmailAsync(email.Normalize()), Times.Once);
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ChangeEmailPostAddsCorrectErrorToModelStateWhenCheckPasswordIsSuccessfulAndEmailCannotBeFound()
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

        [Fact]
        public async Task ChangeEmailPostRedirectsToTheCorrectActionWithTheCorrectRouteValuesWhenUserIsNull()
        { 
            ManageController controller = InitializeControllerWithNullUser();

            IActionResult actionResult = await controller.ChangeEmail(new ChangeEmailViewModel());

            CheckRedirectionToActionWithMessageRouteValue(actionResult, nameof(ManageController.Index), ManageMessageId.Error);
        }

        [Fact]
        public void ChangeEmailPostHasHttpPostAttribute()
        {
            CheckManageControllerMethodAttribute<HttpPostAttribute>(nameof(ManageController.ChangeEmail), new [] { typeof(ChangeEmailViewModel) });
        }

        [Fact]
        public void ChangeEmailPostHasValidateAntiForgeryTokenAttribute()
        {
            CheckManageControllerMethodAttribute<ValidateAntiForgeryTokenAttribute>(nameof(ManageController.ChangeEmail), new[] { typeof(ChangeEmailViewModel) });
        }

        [Fact]
        public async Task ConfirmNewEmailReturnsErrorViewWhenTokenIsNull()
        {
            var controller = InitializeControllerWithNullUser();

            var actionResult = await controller.ConfirmNewEmail(null);

            CheckReturnsErrorView(actionResult);
        }

        [Fact]
        public async Task ConfirmNewEmailSendsUserByUserIdQueryWithCorrectUserId()
        {
            const string userId = "UserID";

            var controllerAndMocks = InitializeControllerWithValidUser(new ApplicationUser{Id = userId});
            controllerAndMocks.userManagerMock.Setup(u => u.ChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new IdentityResult());

            await controllerAndMocks.controller.ConfirmNewEmail("");

            controllerAndMocks.mediatorMock.Verify(u => u.SendAsync(It.Is<UserByUserIdQuery>(i => i.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task ConfirmNewEmailReturnsErrorViewWhenUserIsNull()
        {
            var controller = InitializeControllerWithNullUser();

            var actionResult = await controller.ConfirmNewEmail("");

            CheckReturnsErrorView(actionResult);
        }

        [Fact]
        public async Task ConfirmNewEmailInvokesChangeEmailAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            const string pendingNewEmail = "email";
            var user = new ApplicationUser{PendingNewEmail = pendingNewEmail};
            const string token = "token";

            var controllerAndMocks = InitializeControllerWithValidUser(user);
            controllerAndMocks.userManagerMock.Setup(u => u.ChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new IdentityResult());

            await controllerAndMocks.controller.ConfirmNewEmail(token);

            controllerAndMocks.userManagerMock.Verify(u => u.ChangeEmailAsync(user, pendingNewEmail, token));
        }

        [Fact]
        public async Task ConfirmNewEmailInvokesSetUserNameAsyncWithCorrectParametersWhenUserIsNotNullAndChangeEmailIsSuccessful()
        {
            const string pendingNewEmail = "email";
            var user = new ApplicationUser { PendingNewEmail = pendingNewEmail };

            var controllerAndMocks = InitializeControllerWithValidUser(user);
            controllerAndMocks.userManagerMock.Setup(u => u.ChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            await controllerAndMocks.controller.ConfirmNewEmail("");

            controllerAndMocks.userManagerMock.Verify(u => u.SetUserNameAsync(user, pendingNewEmail));
        }

        [Fact]
        public async Task ConfirmNewEmailInvokesUpdateAsyncWithCorrectParametersWhenUserIsNotNullAndChangeEmailIsSuccessful()
        {
            const string pendingNewEmail = "email";
            var user = new ApplicationUser { PendingNewEmail = pendingNewEmail };

            var controllerAndMocks = InitializeControllerWithValidUser(user);
            controllerAndMocks.userManagerMock.Setup(u => u.ChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            await controllerAndMocks.controller.ConfirmNewEmail("");

            controllerAndMocks.userManagerMock.Verify(u => u.UpdateAsync(user));
        }

        [Fact]
        public async Task ConfirmNewEmailRedirectsToCorrectActionWithCorrectRouteValues()
        {
            const string pendingNewEmail = "email";
            var user = new ApplicationUser { PendingNewEmail = pendingNewEmail };
            const string token = "token";

            var controllerAndMocks = InitializeControllerWithValidUser(user);
            controllerAndMocks.userManagerMock.Setup(u => u.ChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new IdentityResult());

            IActionResult actionResult = await controllerAndMocks.controller.ConfirmNewEmail(token);

            CheckRedirectionToActionWithMessageRouteValue(actionResult, nameof(ManageController.Index), ManageMessageId.ChangeEmailSuccess);
        }

        [Fact]
        public void ConfirmEmailHasHttpGetAttribute()
        {
            CheckManageControllerMethodAttribute<HttpGetAttribute>(nameof(ManageController.ConfirmNewEmail), new [] { typeof(string)});
        }

        [Fact]
        public async Task ResendChangeEmailConfirmationSendsUserByUserIdQueryWithCorrectUserId()
        {
            const string userId = "UserID";

            var controllerAndMocks = InitializeControllerWithValidUser(new ApplicationUser { Id = userId });

            await controllerAndMocks.controller.ResendChangeEmailConfirmation();

            controllerAndMocks.mediatorMock.Verify(u => u.SendAsync(It.Is<UserByUserIdQuery>(i => i.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task ResendChangeEmailConfirmationReturnsErrorViewWhenUserIsNull()
        {
            var controller = InitializeControllerWithNullUser();

            IActionResult actionResult = await controller.ResendEmailConfirmation();

            CheckReturnsErrorView(actionResult);
        }

        [Fact]
        public async Task ResendChangeEmailConfirmationReturnsErrorViewWhenUsersPendingNewEmailIsNullOrEmpty()
        {
            var controllerAndMocks = InitializeControllerWithValidUser(new ApplicationUser { PendingNewEmail = ""});

            IActionResult actionResult = await controllerAndMocks.controller.ResendEmailConfirmation();

            CheckReturnsErrorView(actionResult);
        }

        [Fact]
        public async Task ResendChangesEmailConfirmationInvokesGenerateChangeEmailTokenAsyncWithCorrectParameters()
        {
            const string newEmail = "email";
            var user = new ApplicationUser {PendingNewEmail = newEmail};

            var controllerAndMocks = InitializeControllerWithValidUser(user);
            controllerAndMocks.controller.SetFakeIUrlHelper();
            controllerAndMocks.controller.SetFakeHttpRequestSchemeTo(It.IsAny<string>());
            
            await controllerAndMocks.controller.ResendChangeEmailConfirmation();

            controllerAndMocks.userManagerMock.Verify(u => u.GenerateChangeEmailTokenAsync(user, newEmail));
        }

        [Fact]
        public async Task ResendChangesEmailConfirmationInvokesUrlActionWithCorrectParameters()
        {
            const string newEmail = "email";
            const string requestScheme = "requestScheme";
            const string token = "token";
            var user = new ApplicationUser { PendingNewEmail = newEmail };

            var controllerAndMocks = InitializeControllerWithValidUser(user);
            controllerAndMocks.controller.SetFakeIUrlHelper();
            controllerAndMocks.controller.SetFakeHttpRequestSchemeTo(requestScheme);
            controllerAndMocks.userManagerMock.Setup(x => x.GenerateChangeEmailTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(token);

            await controllerAndMocks.controller.ResendChangeEmailConfirmation();

            controllerAndMocks.controller.GetMockIUrlHelper().Verify(mock => mock.Action(It.Is<UrlActionContext>(uac =>
                    uac.Action == nameof(ManageController.ConfirmNewEmail) &&
                    uac.Controller == "Manage" &&
                    uac.Protocol == requestScheme)),
                Times.Once);

        }

        [Fact]
        public async Task ResendChangesEmailConfirmationSendsSendNewEmailAddressConfirmationEmailAsyncWithCorrectData()
        {
            const string requestScheme = "requestScheme";
            const string email = "email";

            var user = new ApplicationUser {PendingNewEmail = email};
            var controllerAndMocks = InitializeControllerWithValidUser(user);
            controllerAndMocks.controller.SetFakeIUrlHelper();
            controllerAndMocks.controller.SetFakeHttpRequestSchemeTo(requestScheme);

            await controllerAndMocks.controller.ResendChangeEmailConfirmation();

            controllerAndMocks.mediatorMock.Verify(m => m.SendAsync(It.Is<SendNewEmailAddressConfirmationEmail>(s => s.Email == email)), Times.Once);
        }

        [Fact]
        public async Task ResendChangesEmailConfirmationRedirectsToCorrectAction()
        {
            var user = new ApplicationUser{PendingNewEmail = "email"};

            var controllerAndMocks = InitializeControllerWithValidUser(user);
            controllerAndMocks.controller.SetFakeIUrlHelper();
            controllerAndMocks.controller.SetFakeHttpRequestSchemeTo(It.IsAny<string>());

            IActionResult actionResult = await controllerAndMocks.controller.ResendChangeEmailConfirmation();

            CheckRedirectionToAction(actionResult, nameof(ManageController.EmailConfirmationSent));
        }

        [Fact]
        public void ResendChangesEmailConfirmationHasHttpPostAttribute()
        {
            CheckManageControllerMethodAttribute<HttpPostAttribute>(nameof(ManageController.ResendChangeEmailConfirmation), new Type[] { });
        }

        [Fact]
        public void ResendChangesEmailConfirmationHasVAlidateAntiForgeryTokenAttribute()
        {
            CheckManageControllerMethodAttribute<ValidateAntiForgeryTokenAttribute>(nameof(ManageController.ResendChangeEmailConfirmation), new Type[] { });
        }

        [Fact]
        public async Task CancelChangeEmailSendsUserByUserIdQueryWithCorrectUserId()
        {
            const string userId = "UserID";

            var controllerAndMocks = InitializeControllerWithValidUser(new ApplicationUser{Id = userId});
            
            await controllerAndMocks.controller.CancelChangeEmail();

            controllerAndMocks.mediatorMock.Verify(u => u.SendAsync(It.Is<UserByUserIdQuery>(i => i.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task CancelChangeEmailInvokesUpdateAsyncWithCorrectParameters()
        {
            const string userId = "UserID";
            var user = new ApplicationUser { Id = userId };

            var controllerAndMocks = InitializeControllerWithValidUser(user);

            await controllerAndMocks.controller.CancelChangeEmail();

            controllerAndMocks.userManagerMock.Verify(u => u.UpdateAsync(user));
        }

        [Fact]
        public async Task CancelChangeEmailRedirectsToCorrectAction()
        {
            const string userId = "UserID";
            var user = new ApplicationUser { Id = userId };

            var controllerAndMocks = InitializeControllerWithValidUser(user);

            var actionResult = await controllerAndMocks.controller.CancelChangeEmail();

            CheckRedirectionToAction(actionResult, nameof(ManageController.Index));
        }

        [Fact]
        public void CancelChangeEmailHasHttpPostAttribute()
        {
            CheckManageControllerMethodAttribute<HttpPostAttribute>(nameof(ManageController.CancelChangeEmail), new Type[] { });
        }

        [Fact]
        public void CancelChangeEmailHasValidateAntiForgeryTokenAttribute()
        {
            CheckManageControllerMethodAttribute<ValidateAntiForgeryTokenAttribute>(nameof(ManageController.CancelChangeEmail), new Type[] { });
        }

        [Fact]
        public void SetPasswordGetReturnsAView()
        {
            var controllerAndMocks = InitializeControllerWithValidUser(new ApplicationUser());

            var actionResult = controllerAndMocks.controller.SetPassword();

            Assert.IsType<ViewResult>(actionResult); 
        }

        [Fact]
        public void SetPasswordGetHasHttpGetAttribute()
        {
            CheckManageControllerMethodAttribute<HttpGetAttribute>(nameof(ManageController.SetPassword), new Type[] { });
        }

        [Fact]
        public async Task SetPasswordPostReturnsSameViewAndViewModelWhenModelStateIsInvalid()
        {
            const string newPassword = "password";

            var controller = new ManageController(null, null, null);
            controller.ModelState.AddModelError("error", "error msg");

            var setPasswordViewModel = new SetPasswordViewModel { NewPassword = newPassword };

            var result = await controller.SetPassword(setPasswordViewModel);

            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            var resultViewModel = viewResult.ViewData.Model;
            var resultSetPasswordViewModel = resultViewModel as SetPasswordViewModel;
            Assert.NotNull(resultSetPasswordViewModel);
            Assert.Equal(newPassword, resultSetPasswordViewModel.NewPassword);
        }

        [Fact]
        public async Task SetPasswordPostSendsUserByUserIdQueryWithCorrectUserId()
        {
            const string userId = "UserID";

            var controllerAndMocks = InitializeControllerWithValidUser(new ApplicationUser{Id = userId});

            await controllerAndMocks.controller.SetPassword(new SetPasswordViewModel());

            controllerAndMocks.mediatorMock.Verify(u => u.SendAsync(It.Is<UserByUserIdQuery>(i => i.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task SetPasswordPostInvokesAddPasswordAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            const string password = "password";
            var user = new ApplicationUser();
            var controllerAndMocks = InitializeControllerWithValidUser(user);
            controllerAndMocks.userManagerMock.Setup(u => u.AddPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(new IdentityResult());

            await controllerAndMocks.controller.SetPassword(new SetPasswordViewModel(){NewPassword = password});

            controllerAndMocks.userManagerMock.Verify(u => u.AddPasswordAsync(user, password));
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

        [Fact]
        public void SetPasswordPostHasHttpPostAttribute()
        {
            CheckManageControllerMethodAttribute<HttpPostAttribute>(nameof(ManageController.SetPassword), new [] { typeof(SetPasswordViewModel) });
        }

        [Fact]
        public void SetPasswordPostHasValidateAntiForgeryTokenAttribute()
        {
            CheckManageControllerMethodAttribute<ValidateAntiForgeryTokenAttribute>(nameof(ManageController.SetPassword), new[] { typeof(SetPasswordViewModel) });
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

        [Fact]
        public async Task ManageLoginsSendsUserByUserIdQueryWithCorrectUserId()
        {
            const string userId = "UserID";

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var mediator = new Mock<IMediator>();

            var controller = new ManageController(userManagerMock.Object, null, mediator.Object);

            await controller.ManageLogins();

            mediator.Verify(u => u.SendAsync(It.Is<UserByUserIdQuery>(i => i.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task ManageLoginsReturnsErrorViewWhenUserIsNull()
        {
            var controller = InitializeControllerWithNullUser();

            var actionResult = await controller.ManageLogins();

            CheckReturnsErrorView(actionResult);
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

        [Fact]
        public void ManageLoginsHasHttpGetAttribute()
        {
            CheckManageControllerMethodAttribute<HttpGetAttribute>(nameof(ManageController.ManageLogins), new[] { typeof(ManageMessageId) });
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

        [Fact]
        public void LinkLoginHasHttpPostAttribute()
        {
            CheckManageControllerMethodAttribute<HttpPostAttribute>(nameof(ManageController.LinkLogin), new[] { typeof(string) });
        }

        [Fact]
        public void LinkLoginHasValidateAntiForgeryTokenAttribute()
        {
            CheckManageControllerMethodAttribute<ValidateAntiForgeryTokenAttribute>(nameof(ManageController.LinkLogin), new[] { typeof(string) });
        }

        [Fact]
        public async Task LinkLoginCallbackSendsUserByUserIdQueryWithCorrectUserId()
        {
            const string userId = "UserID";

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var mediator = new Mock<IMediator>();

            var controller = new ManageController(userManagerMock.Object, null, mediator.Object);

            await controller.LinkLoginCallback();

            mediator.Verify(u => u.SendAsync(It.Is<UserByUserIdQuery>(i => i.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task LinkLoginCallbackReturnsErrorViewWhenUserIsNull()
        {
            var controller = InitializeControllerWithNullUser();

            var actionResult = await controller.LinkLoginCallback();

            CheckReturnsErrorView(actionResult);
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

        private static (ManageController controller, Mock<UserManager<ApplicationUser>> userManagerMock, Mock<IMediator> mediatorMock, Mock<SignInManager<ApplicationUser>> signInManagerMock) InitializeControllerWithValidUser(ApplicationUser applicationUser)
        {
            //Mock controller dependencies UserManager, signinmanager and IMediator, set behaviour of called methods
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            if (applicationUser.Id != null)
            {
                userManagerMock.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(applicationUser.Id);
            }
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(applicationUser);

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            return (controller, userManagerMock, mediator, signInManagerMock);
        }

        private static ManageController InitializeControllerWithNullUser()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync((ApplicationUser)null);

            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();

            return new ManageController(userManagerMock.Object, null, mediatorMock.Object);
        }

        private void CheckReturnsErrorView(IActionResult actionResult)
        {
            var result = actionResult as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(result.ViewName, "Error");
        }

        private static void CheckManageControllerMethodAttribute<T>(string methodName, Type[] parametersTypes)
        {
            var t = typeof(ManageController);
            MethodInfo methodInfo = t.GetMethod(methodName, parametersTypes);
            var attribute = methodInfo.GetCustomAttributes().OfType<T>().SingleOrDefault();
            Assert.NotNull(attribute);
        }
    }
}
