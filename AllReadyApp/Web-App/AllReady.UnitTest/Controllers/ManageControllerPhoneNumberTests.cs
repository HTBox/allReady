using System.Linq;
using System.Threading.Tasks;
using Xunit;
using AllReady.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MediatR;
using AllReady.ViewModels.Manage;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using System.Security.Claims;
using AllReady.Features.Login;
using AllReady.Features.Manage;
using Microsoft.AspNetCore.Identity;

namespace AllReady.UnitTest.Controllers
{
    public class ManageControllerPhoneNumberTests
    {
        [Fact]
        public void AddPhoneNumberGetReturnsAView()
        {
            //Arrange
            var controller = new ManageController(null, null, null);

            //Act
            var view = controller.AddPhoneNumber();
            
            //Assert
            Assert.NotNull(view);
        }

        [Fact]
        public async Task AddPhoneNumberPostReturnsTheSameViewAndModelWhenModelStateIsInvalid()
        {
            //Arrange
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            var mediator = new Mock<IMediator>();
            var phoneNumber = "number";

            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            var invalidModel = new AddPhoneNumberViewModel {PhoneNumber = phoneNumber};
            controller.ModelState.AddModelError("PhoneNumber", "Must be a number");

            //Act
            var result = await controller.AddPhoneNumber(invalidModel);
            var resultViewModel = (ViewResult)result;
            var viewModel = (AddPhoneNumberViewModel)resultViewModel.ViewData.Model;

            //Assert
            Assert.Equal(phoneNumber, viewModel.PhoneNumber);
        }

        [Fact]
        public async Task AddPhoneNumberPostSendsUserByUserIdQueryWithCorrectUserIdWhenModelStateIsValid()
        {
            //Arrange
            var userId = "userId";
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.AddPhoneNumber(new AddPhoneNumberViewModel());

            //Assert
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(u => u.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task AddPhoneNumberPostInvokesGenerateChangePhoneNumberTokenAsyncWithCorrectParametersWhenModelStateIsValid()
        {
            //Arrange
            var userId = "userId";
            var token = "token";
            var user = new ApplicationUser {Id = userId};
            var model = new AddPhoneNumberViewModel {PhoneNumber = "phone"};

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(),
                                                                         It.IsAny<string>())).ReturnsAsync(token);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.AddPhoneNumber(model);

            //Assert
            userManager.Verify(x => x.GenerateChangePhoneNumberTokenAsync(It.Is<ApplicationUser>(u => u.Id == userId),
                                                                          It.Is<string>(p => p == model.PhoneNumber)),
                               Times.Once);
        }

        [Fact]
        public async Task AddPhoneNumberPostSendsSendAccountSecurityTokenSmsAsyncWithCorrectDataWhenModelStateIsValid()
        {
            //Arrange
            var userId = "userId";
            var token = "token";
            var user = new ApplicationUser { Id = userId };
            var model = new AddPhoneNumberViewModel { PhoneNumber = "phone" };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(),
                                                                         It.IsAny<string>())).ReturnsAsync(token);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.AddPhoneNumber(model);

            //Assert
            mediator.Verify(m => m.SendAsync(It.Is<SendAccountSecurityTokenSms>(x => x.Token == token && x.PhoneNumber == model.PhoneNumber)), Times.Once);
        }

        [Fact]
        public async Task AddPhonNumberPostRedirectsToCorrectActionWithCorrectRouteValuesWhenModelStateIsValid()
        {
            //Arrange
            var userId = "userId";
            var token = "token";
            var user = new ApplicationUser { Id = userId };
            var model = new AddPhoneNumberViewModel { PhoneNumber = "phone" };

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(),
                                                                         It.IsAny<string>())).ReturnsAsync(token);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            var result = await controller.AddPhoneNumber(model) as RedirectToActionResult;

            //Assert
            Assert.Equal("VerifyPhoneNumber", result.ActionName);
        }

        [Fact]
        public void AddPhoneNumberHasHttpPostAttribute()
        {
            //Arrange
            var controller = new ManageController(null, null, null);

            //Act
            var attribute = controller.GetAttributesOn(x => x.AddPhoneNumber(It.IsAny<AddPhoneNumberViewModel>()))
                                      .OfType<HttpPostAttribute>()
                                      .SingleOrDefault();

            //Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void AddPhoneNumberHasValidateAntiForgeryTokenAttribute()
        {
            //Arrange
            var controller = new ManageController(null, null, null);

            //Act
            var attribute = controller.GetAttributesOn(x => x.AddPhoneNumber(It.IsAny<AddPhoneNumberViewModel>()))
                                      .OfType<ValidateAntiForgeryTokenAttribute>()
                                      .SingleOrDefault();

            //Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task ResendPhoneNumberConfirmationSendsUserByUserIdQueryWithCorrectUserId()
        {
            //Arrange
            var userId = "userId";
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync((ApplicationUser)null);

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.ResendPhoneNumberConfirmation("phone");

            //Assert
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(u => u.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task ResendPhoneNumberConfirmationInvokesGenerateChangePhoneNumberTokenAsyncWithCorrectParameters()
        {
            //Arrange
            var userId = "userId";
            var token = "token";
            var user = new ApplicationUser { Id = userId };
            var phoneNumber = "phone";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(),
                                                                         It.IsAny<string>())).ReturnsAsync(token);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.ResendPhoneNumberConfirmation(phoneNumber);

            //Assert
            userManager.Verify(x => x.GenerateChangePhoneNumberTokenAsync(It.Is<ApplicationUser>(u => u.Id == userId),
                                                                          It.Is<string>(p => p == phoneNumber)),
                               Times.Once);
        }

        [Fact]
        public async Task ResendPhoneNumberSendsSendAccountSecurityTokenSmsAsyncWithCorrectData()
        {
            //Arrange
            var userId = "userId";
            var token = "token";
            var user = new ApplicationUser { Id = userId };
            var phoneNumber = "phone";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(),
                                                                         It.IsAny<string>())).ReturnsAsync(token);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.ResendPhoneNumberConfirmation(phoneNumber);

            //Assert
            mediator.Verify(m => m.SendAsync(It.Is<SendAccountSecurityTokenSms>(x => x.Token == token && x.PhoneNumber == phoneNumber)), Times.Once);
        }

        [Fact]
        public async Task ResendPhoneNumberRedirectsToCorrectActionWithCorrectRouteValues()
        {
            //Arrange
            var userId = "userId";
            var token = "token";
            var user = new ApplicationUser { Id = userId };
            var phoneNumber = "phone";

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(),
                                                                         It.IsAny<string>())).ReturnsAsync(token);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            var result = await controller.ResendPhoneNumberConfirmation(phoneNumber) as RedirectToActionResult;

            //Assert
            Assert.Equal("VerifyPhoneNumber", result.ActionName);
        }

        [Fact]
        public void ResendPhoneNumberHasHttpPostAttribute()
        {
            //Arrange
            var controller = new ManageController(null, null, null);

            //Act
            var attribute = controller.GetAttributesOn(x => x.ResendPhoneNumberConfirmation(It.IsAny<string>()))
                                      .OfType<HttpPostAttribute>()
                                      .SingleOrDefault();

            //Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ResendPhoneNumberHasValidateAntiForgeryTokenAttribute()
        {
            //Arrange
            var controller = new ManageController(null, null, null);

            //Act
            var attribute = controller.GetAttributesOn(x => x.ResendPhoneNumberConfirmation(It.IsAny<string>()))
                                      .OfType<ValidateAntiForgeryTokenAttribute>()
                                      .SingleOrDefault();

            //Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void VerifyPhoneNumberGetReturnsErrorViewWhenPhoneNumberIsNull()
        {
            //Arrange
            var controller = new ManageController(null, null, null);

            //Act
            var view = controller.VerifyPhoneNumber((string)null) as ViewResult;

            //Assert
            Assert.Equal("Error", view.ViewName);
        }

        [Fact]
        public void VerifyPhoneNumberGetReturnsTheCorrectViewandViewModelWhenPhoneNumberIsNotNull()
        {
            //Arrange
            var phoneNumber = "phone";
            var controller = new ManageController(null, null, null);

            //Act
            var view = controller.VerifyPhoneNumber(phoneNumber) as ViewResult;
            var viewModel = view.ViewData.Model as VerifyPhoneNumberViewModel;

            //Assert
            Assert.Equal(phoneNumber, viewModel.PhoneNumber);
        }

        [Fact]
        public void VerifyPhoneNumberGetHasHttpGetAttribute()
        {
            //Arrange
            var controller = new ManageController(null, null, null);

            //Act
            var attribute = controller.GetAttributesOn(x => x.VerifyPhoneNumber(It.IsAny<string>()))
                                      .OfType<HttpGetAttribute>()
                                      .SingleOrDefault();

            //Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task VerifyPhoneNumberPostReturnsTheSameViewAndModelWhenModelStateIsInvalid()
        {
            //Arrange
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            var mediator = new Mock<IMediator>();
            var phoneNumber = "number";

            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            var invalidModel = new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber };
            controller.ModelState.AddModelError("PhoneNumber", "Must be a number");

            //Act
            var result = await controller.VerifyPhoneNumber(invalidModel);
            var resultViewModel = (ViewResult)result;
            var viewModel = (VerifyPhoneNumberViewModel)resultViewModel.ViewData.Model;

            //Assert
            Assert.Equal(phoneNumber, viewModel.PhoneNumber);
        }

        [Fact]
        public async Task VerifyPhoneNumberPostSendsUserByUserIdQueryWithCorrectUserId()
        {
            //Arrange
            var userId = "userId";
            var userManagerMock = UserManagerMockHelper.CreateUserManagerMock();
            userManagerMock.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManagerMock);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync((ApplicationUser)null);

            var controller = new ManageController(userManagerMock.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.VerifyPhoneNumber(new VerifyPhoneNumberViewModel());

            //Assert
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(u => u.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task VerifyPhoneNumberPostInvokesChangePhoneNumberAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            //Arrange
            var userId = "userId";
            var token = "token";
            var user = new ApplicationUser {Id = userId};
            var model = new VerifyPhoneNumberViewModel {PhoneNumber = "phone", Code = token};

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.ChangePhoneNumberAsync(It.IsAny<ApplicationUser>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>())).ReturnsAsync(new IdentityResult());

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.VerifyPhoneNumber(model);

            //Assert
            userManager.Verify(x => x.ChangePhoneNumberAsync(It.Is<ApplicationUser>(u => u.Id == userId),
                                                             It.Is<string>(p => p == model.PhoneNumber),
                                                             It.Is<string>(t => t == token)),
                               Times.Once);
        }

        [Fact]
        public async Task VerifyPhoneNumberPostSendsRemoveUserProfileIncompleteClaimCommandWhenUserIsNotNullAndPhoneNumberChangeWasSuccessfulAndUserProfileIsComplete()
        {
            //Arrange
            var userId = "userId";
            var token = "token";
            var phoneNumber = "phone";
            var user = UserWithCompleteProfile(userId, phoneNumber);

            var model = new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber, Code = token };
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.ChangePhoneNumberAsync(It.IsAny<ApplicationUser>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.VerifyPhoneNumber(model);

            //Assert
            mediator.Verify(x => x.SendAsync(It.IsAny<RemoveUserProfileIncompleteClaimCommand>()), Times.Once);
        }

        [Fact]
        public async Task VerifyPhoneNumberPostInvokesRefreshSignInAsyncWithCorrectParametersWhenUserIsNotNullAndPhoneNumberChangeWasSuccessfulAndUserProfileIsComplete()
        {
            //Arrange
            var userId = "userId";
            var token = "token";
            var phoneNumber = "phone";
            var user = UserWithCompleteProfile(userId, phoneNumber);

            var model = new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber, Code = token };
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.ChangePhoneNumberAsync(It.IsAny<ApplicationUser>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManager.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.VerifyPhoneNumber(model);

            //Assert
            signInManager.Verify(x => x.RefreshSignInAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [Fact]
        public async Task VerifyPhoneNumberPostInvokesSignInAsyncWithCorrectPaarmetersWhenUserIsNotNullAndPhoneNumberChangeWasSuccessful()
        {
            //Arrange
            var userId = "userId";
            var token = "token";
            var phoneNumber = "phone";
            var user = UserWithCompleteProfile(userId, phoneNumber);

            var model = new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber, Code = token };
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.ChangePhoneNumberAsync(It.IsAny<ApplicationUser>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManager.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.VerifyPhoneNumber(model);

            //Assert
            signInManager.Verify(x => x.SignInAsync(It.Is<ApplicationUser>(u => u == user),
                                                    It.Is<bool>(p => !p),
                                                    It.IsAny<string>()),
                                 Times.Once);
        }

        [Fact]
        public async Task VerifyPhoneNumberPostRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNotNullAndPhoneNumberChangeWasSuccessful()
        {
            //Arrange
            var userId = "userId";
            var token = "token";
            var phoneNumber = "phone";
            var user = UserWithCompleteProfile(userId, phoneNumber);

            var model = new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber, Code = token };
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.ChangePhoneNumberAsync(It.IsAny<ApplicationUser>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManager.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            var result = await controller.VerifyPhoneNumber(model) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(ManageController.ManageMessageId.AddPhoneSuccess, result.RouteValues["Message"]);
        }

        [Fact]
        public async Task VerifyPhoneNumberPostAddsCorrectErrorMessageToModelStateWhenUserIsNull()
        {
            //Arrange
            var token = "token";
            var phoneNumber = "phone";

            var model = new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber, Code = token };
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            var controller = new ManageController(userManager.Object, signInManager.Object, mediator.Object);

            //Act
            await controller.VerifyPhoneNumber(model);

            //Assert
            Assert.Equal("Failed to verify mobile phone number",
                         controller.ViewData.ModelState[""].Errors.First().ErrorMessage);
        }

        [Fact]
        public async Task VerifyPhoneNumberPostReturnsCorrectViewModelWhenUserIsNull()
        {
            //Arrange
            var token = "token";
            var phoneNumber = "phone";

            var model = new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber, Code = token };
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            var controller = new ManageController(userManager.Object, signInManager.Object, mediator.Object);

            //Act
            var result = await controller.VerifyPhoneNumber(model) as ViewResult;

            //Assert
            Assert.Equal(model, result.Model);
       }

        [Fact]
        public void VerifyPhoneNumberPostHasHttpPostAttribute()
        {
            //Arrange
            var controller = new ManageController(null, null, null);

            //Act
            var attribute = controller.GetAttributesOn(x => x.VerifyPhoneNumber(It.IsAny<VerifyPhoneNumberViewModel>()))
                                      .OfType<HttpPostAttribute>()
                                      .SingleOrDefault();

            //Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public void VerifyPhoneNumberPostHasValidateAntiForgeryTokenAttribute()
        {
            //Arrange
            var controller = new ManageController(null, null, null);

            //Act
            var attribute = controller.GetAttributesOn(x => x.VerifyPhoneNumber(It.IsAny<VerifyPhoneNumberViewModel>()))
                                      .OfType<ValidateAntiForgeryTokenAttribute>()
                                      .SingleOrDefault();

            //Assert
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task RemovePhoneNumberSendsUserByUserIdQueryWithCorrectUserId()
        {
            //Arrange
            var userId = "userId";
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.SetPhoneNumberAsync(It.IsAny<ApplicationUser>(),
                                                            It.IsAny<string>())).ReturnsAsync(new IdentityResult());

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new ManageController(userManager.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.RemovePhoneNumber();

            //Assert
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(u => u.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task RemovePhoneNumberInvokesSetPhoneNumberAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            //Arrange
            var userId = "userId";
            var user = new ApplicationUser {Id = userId};
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.SetPhoneNumberAsync(It.IsAny<ApplicationUser>(),
                                                         It.IsAny<string>())).ReturnsAsync(new IdentityResult());

            var signInManagerMock = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManagerMock.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.RemovePhoneNumber();

            //Assert
            userManager.Verify(x => x.SetPhoneNumberAsync(It.Is<ApplicationUser>(u => u == user),
                                                          It.Is<string>(p => p == null)),
                               Times.Once);
        }

        [Fact]
        public async Task RemovePhoneNumberInvokesSignInAsyncWithCorrectParametersWhenUserIsNotNullAndPhoneNumberWasSetSuccessfully()
        {
            //Arrange
            var userId = "userId";
            var phoneNumber = "phone";
            var user = UserWithCompleteProfile(userId, phoneNumber);

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.SetPhoneNumberAsync(It.IsAny<ApplicationUser>(),
                                                         It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManager.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.RemovePhoneNumber();

            //Assert
            signInManager.Verify(x => x.SignInAsync(It.Is<ApplicationUser>(u => u == user),
                                                    It.Is<bool>(p => !p),
                                                    It.IsAny<string>()),
                                 Times.Once);
        }

        [Fact]
        public async Task RemovePhoneNumberSendsRemoveUserProfileIncompleteClaimCommandWithCorrectDataWhenUserIsNotNullAndPhoneNumberWasSetSuccessfullyAndUsersProfileIsComplete()
        {
            //Arrange
            var userId = "userId";
            var phoneNumber = "phone";
            var user = UserWithCompleteProfile(userId, phoneNumber);

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.SetPhoneNumberAsync(It.IsAny<ApplicationUser>(),
                                                         It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManager.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.RemovePhoneNumber();

            //Assert
            mediator.Verify(x => x.SendAsync(It.IsAny<RemoveUserProfileIncompleteClaimCommand>()), Times.Once);
        }

        [Fact]
        public async Task RemovePhoneNumberInvokesRefreshSignInAsyncWithCorrectParametersWhenUserIsNotNullAndPhoneNumberWasSetSuccessfullyAndUsersProfileIsComplete()
        {
            //Arrange
            var userId = "userId";
            var phoneNumber = "phone";
            var user = UserWithCompleteProfile(userId, phoneNumber);

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.SetPhoneNumberAsync(It.IsAny<ApplicationUser>(),
                                                         It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManager.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            await controller.RemovePhoneNumber();

            //Assert
            signInManager.Verify(x => x.RefreshSignInAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [Fact]
        public async Task RemovePhoneNumberRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNotNullAndPhoneNumberWasSetSuccessfully()
        {
            //Arrange
            var userId = "userId";
            var phoneNumber = "phone";
            var user = UserWithCompleteProfile(userId, phoneNumber);

            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            userManager.Setup(x => x.SetPhoneNumberAsync(It.IsAny<ApplicationUser>(),
                                                         It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(user);

            var controller = new ManageController(userManager.Object, signInManager.Object, mediator.Object);
            controller.SetFakeUser(userId);

            //Act
            var result = await controller.RemovePhoneNumber() as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(ManageController.ManageMessageId.RemovePhoneSuccess, result.RouteValues["Message"]);
        }

        [Fact]
        public async Task RemovePhoneNumberRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNull()
        {
            //Arrange
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            var signInManager = SignInManagerMockHelper.CreateSignInManagerMock(userManager);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync((ApplicationUser)null);

            var controller = new ManageController(userManager.Object, signInManager.Object, mediator.Object);

            //Act
            var result = await controller.RemovePhoneNumber() as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(ManageController.ManageMessageId.Error, result.RouteValues["Message"]);
        }

        [Fact]
        public void RemovePhoneNumberHasHttpGetAttribute()
        {
            //Arrange
            var controller = new ManageController(null, null, null);

            //Act
            var attribute = controller.GetAttributesOn(x => x.RemovePhoneNumber())
                                      .OfType<HttpGetAttribute>()
                                      .SingleOrDefault();

            //Assert
            Assert.NotNull(attribute);
        }

        private static ApplicationUser UserWithCompleteProfile(string userId, string phoneNumber) =>
            new ApplicationUser
            {
                Id = userId,
                EmailConfirmed = true,
                FirstName = "first",
                LastName = "last",
                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = true
            };
    }
}
