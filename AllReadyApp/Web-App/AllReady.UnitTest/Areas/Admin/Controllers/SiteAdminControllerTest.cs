using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Features.Site;
using AllReady.Areas.Admin.Features.Users;
using AllReady.Extensions;
using AllReady.Features.Manage;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Site;
using AllReady.Constants;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class SiteAdminControllerTest
    {
        [Fact]
        public async Task IndexReturnsCorrectViewModel()
        {
            var users = new List<ApplicationUser> { new ApplicationUser { Id = It.IsAny<string>() }, new ApplicationUser { Id = It.IsAny<string>() } };
            var viewModel = new IndexViewModel { Users = users };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<IndexQuery>())).ReturnsAsync(viewModel);

            var controller = new SiteController(null, null, mediator.Object);
            var result = await controller.Index() as ViewResult;
            var model = result.ViewData.Model as IndexViewModel;

            Assert.Equal(model.Users.Count(), users.Count());
            Assert.IsType<IndexViewModel>(model);
        }

        [Fact]
        public async Task DeleteUserSendsUserQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();

            const string userId = "foo_id";
            mediator.Setup(x => x.SendAsync(It.Is<UserQuery>(q => q.UserId == userId))).ReturnsAsync(new EditUserViewModel());
            var controller = new SiteController(null, null, mediator.Object);

            await controller.DeleteUser(userId);
            mediator.Verify(m => m.SendAsync(It.Is<UserQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task DeleteUserReturnsTheCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            const string userId = "foo_id";
            mediator.Setup(x => x.SendAsync(It.IsAny<UserQuery>())).ReturnsAsync(new EditUserViewModel());
            var controller = new SiteController(null, null, mediator.Object);

            var result = await controller.DeleteUser(userId);
            var model = ((ViewResult)result).ViewData.Model as DeleteUserViewModel;

            Assert.Equal(model.UserId, userId);
            Assert.IsType<DeleteUserViewModel>(model);
        }

        [Fact]
        public void DeleteUserHasHttpGetAttribute()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributesOn(x => x.DeleteUser(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task ConfirmDeletUserInvokesFindByIdAsync()
        {
            const string userId = "userId";
            var userManager = CreateApplicationUserMock();

            var controller = new SiteController(userManager.Object, null, null);

            await controller.ConfirmDeleteUser(userId);
            userManager.Verify(x => x.FindByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task ConfirmDeletUserRedirectsToCorrectAction()
        {
            var applicationUser = CreateApplicationUserMock();

            var controller = new SiteController(applicationUser.Object, null, null);

            var result = await controller.ConfirmDeleteUser(It.IsAny<string>()) as RedirectToActionResult;

            Assert.Equal(nameof(SiteController.Index), result.ActionName);
        }

        [Fact]
        public void ConfirmDeletUserHasHttpPostAttribute()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributesOn(x => x.ConfirmDeleteUser(It.IsAny<string>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ConfirmDeletUserHasValidateAntiForgeryTokenAttribute()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributesOn(x => x.ConfirmDeleteUser(It.IsAny<string>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();

            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task EditUserGetSendsUserByUserIdQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            var userId = It.IsAny<string>();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId)))
                .ReturnsAsync(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.EditUser(userId);
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task EditUserGetReturnsCorrectViewModelWhenOrganizationIdIsNull()
        {
            {
                var mediator = new Mock<IMediator>();

                var userId = It.IsAny<string>();
                mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId)))
                    .ReturnsAsync(new ApplicationUser());
                var controller = new SiteController(null, null, mediator.Object);

                var result = controller.EditUser(userId);
                var model = ((ViewResult)await result).ViewData.Model as EditUserViewModel;

                Assert.Null(model.Organization);
                Assert.IsType<EditUserViewModel>(model);
            }
        }

        [Fact]
        public async Task EditUserGetReturnsCorrectValueForOrganiztionOnViewModelWhenOrganizationIdIsNotNull()
        {
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser();
            var orgId = 99;
            var orgName = "Test Org";
            var org = new Organization { Id = orgId, Name = orgName };
            var userId = It.IsAny<string>();

            user.Claims.Add(new Microsoft.AspNetCore.Identity.IdentityUserClaim<string>
            {
                ClaimType = AllReady.Security.ClaimTypes.Organization,
                ClaimValue = orgId.ToString()
            });

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId)))
               .ReturnsAsync(user);

            mediator.Setup(x => x.Send(It.Is<OrganizationByIdQuery>(q => q.OrganizationId == orgId)))
               .Returns(org);

            var controller = new SiteController(null, null, mediator.Object);

            var result = controller.EditUser(userId);
            var model = ((ViewResult)await result).ViewData.Model as EditUserViewModel;

            Assert.NotNull(model.Organization);
            Assert.IsType<EditUserViewModel>(model);
        }

        [Fact]
        public async Task EditUserPostReturnsSameViewAndViewModelWhenModelStateIsInvalid()
        {
            var model = new EditUserViewModel
            {
                UserId = "1234",
            };
            var controller = new SiteController(null, null, null);
            controller.ModelState.AddModelError("FakeKey", "FakeMessage");
            var result = (ViewResult)await controller.EditUser(model);
            Assert.Equal(result.Model, model);
        }

        [Fact]
        public async Task EditUserPostSendsUserByUserIdQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            var model = new EditUserViewModel
            {
                UserId = "1234",
            };
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)))
                .ReturnsAsync(new ApplicationUser());
            var controller = new SiteController(null, null, mediator.Object);

            await controller.EditUser(model);
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)), Times.Once);
        }

        [Fact]
        public async Task EditUserPostSendsUpdateUserWithCorrectUserWhenUsersAssociatedSkillsAreNotNull()
        {
            var mediator = new Mock<IMediator>();
            var model = new EditUserViewModel
            {
                UserId = It.IsAny<string>(),
                AssociatedSkills = new List<UserSkill> { new UserSkill { Skill = It.IsAny<Skill>() } }
            };
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)))
                .ReturnsAsync(new ApplicationUser());
            var controller = new SiteController(null, null, mediator.Object);

            await controller.EditUser(model);
            mediator.Verify(m => m.SendAsync(It.Is<UpdateUser>(q => q.User.AssociatedSkills[0].Skill == model.AssociatedSkills[0].Skill)), Times.Once);
        }

        [Fact]
        public async Task EditUserPostInvokesUpdateUserWithCorrectUserWhenUsersAssociatedSkillsAreNotNullAndThereIsAtLeastOneAssociatedSkillForTheUser()
        {
            var mediator = new Mock<IMediator>();
            var model = new EditUserViewModel
            {
                UserId = It.IsAny<string>(),
                AssociatedSkills = new List<UserSkill> { new UserSkill { SkillId = 1, Skill = new Skill { Id = 1 } } }
            };
            var user = new ApplicationUser
            {
                Id = model.UserId,
                AssociatedSkills = new List<UserSkill> { new UserSkill { SkillId = 2, Skill = new Skill { Id = 2 } } }
            };
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)))
                .ReturnsAsync(user);

            var controller = new SiteController(null, null, mediator.Object);

            await controller.EditUser(model);
            mediator.Verify(m => m.SendAsync(It.Is<UpdateUser>(q => q.User.AssociatedSkills[0] == model.AssociatedSkills[0])), Times.Once);
        }

        [Fact]
        public async Task EditUserPostInvokesAddClaimAsyncWithCorrectParameters_WhenModelsIsOrganizationAdminIsTrue()
        {
            var userManager = CreateApplicationUserMock();

            var model = new EditUserViewModel
            {
                IsOrganizationAdmin = true,
                UserId = It.IsAny<string>()

            };

            var user = new ApplicationUser
            {
                Id = model.UserId,
                Email = "test@testy.com"
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).ReturnsAsync(user);
            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            controller.SetDefaultHttpContext();
            controller.Url = GetMockUrlHelper("any");
            await controller.EditUser(model);

            userManager.Verify(x => x.AddClaimAsync(user, It.Is<Claim>(c => c.Value == nameof(UserType.OrgAdmin))), Times.Once);
        }

        [Fact]
        public async Task EditUserPostInvokesUrlActionWithCorrectParametersWhenModelsIsOrganizationAdminIsTrueAndOrganizationAdminClaimWasAddedSuccessfully()
        {
            const string requestScheme = "requestScheme";
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var model = new EditUserViewModel
            {
                IsOrganizationAdmin = true,
                UserId = It.IsAny<string>()

            };
            var user = new ApplicationUser
            {
                Id = model.UserId,
                Email = "test@testy.com"
            };

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).ReturnsAsync(user);
            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            controller.SetFakeHttpRequestSchemeTo(requestScheme);
            var urlHelper = new Mock<IUrlHelper>();
            controller.Url = urlHelper.Object;
            await controller.EditUser(model);

            urlHelper.Verify(mock => mock.Action(It.Is<UrlActionContext>(uac =>
                uac.Action == "Login" &&
                uac.Controller == "Account" &&
                uac.Protocol == requestScheme)),
                Times.Once);
        }

        [Fact]
        public async Task EditUserPostSendsSendAccountApprovalEmailWithCorrectDataWhenModelsIsOrganizationAdminIsTrueAndOrganizationAdminClaimWasAddedSuccessfully()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var model = new EditUserViewModel
            {
                IsOrganizationAdmin = true,
                UserId = It.IsAny<string>()

            };
            var user = new ApplicationUser
            {
                Id = model.UserId,
                Email = "test@testy.com"
            };

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).ReturnsAsync(user);
            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            controller.SetDefaultHttpContext();
            var expectedUrl = $"Login/Admin?Email={user.Email}";
            controller.Url = GetMockUrlHelper(expectedUrl);
            await controller.EditUser(model);

            mediator.Verify(m => m.SendAsync(It.Is<SendAccountApprovalEmailCommand>(q => q.Email == user.Email && q.CallbackUrl == expectedUrl)), Times.Once);
        }

        [Fact]
        public async Task EditUserPostReturnsRedirectResultWithCorrectUrlWhenModelsIsOrganizationAdminIsTrueAndOrganizationAdminClaimWasNotAddedSuccessfully()
        {
            var model = new EditUserViewModel
            {
                IsOrganizationAdmin = true,
                UserId = It.IsAny<string>()

            };
            var user = new ApplicationUser
            {
                Id = model.UserId,
                Email = "test@testy.com"
            };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)))
                .ReturnsAsync(user);

            var userManager = CreateApplicationUserMock();
            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Failed(null));

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = await controller.EditUser(model);

            Assert.IsType<RedirectResult>(result);
            Assert.Equal("Error", ((RedirectResult)result).Url);
        }

        [Fact]
        public async Task EditUserPostInvokesRemoveClaimAsyncWithCorrectParameters_WhenUserIsAnOrganizationAdmin()
        {
            var model = new EditUserViewModel { IsOrganizationAdmin = false, UserId = It.IsAny<string>() };

            var user = new ApplicationUser { Id = model.UserId, Email = "test@testy.com" };
            user.MakeOrgAdmin();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).ReturnsAsync(user);

            var userManager = CreateApplicationUserMock();
            userManager.Setup(x => x.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            await controller.EditUser(model);

            userManager.Verify(x => x.RemoveClaimAsync(user, It.Is<Claim>(c => c.Value == nameof(UserType.OrgAdmin))), Times.Once);
        }

        [Fact]
        public async Task EditUserPostReturnsRedirectResultWithCorrectUrlWhenUserIsAnOrganizationAdminAndRemovClaimAsyncDoesNotSucceed()
        {
            var model = new EditUserViewModel { IsOrganizationAdmin = false, UserId = It.IsAny<string>() };

            var user = new ApplicationUser { Id = model.UserId, Email = "test@testy.com" };
            user.MakeOrgAdmin();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).ReturnsAsync(user);

            var userManager = CreateApplicationUserMock();
            userManager.Setup(x => x.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Failed(null));

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = await controller.EditUser(model);

            Assert.IsType<RedirectResult>(result);
            Assert.Equal("Error", ((RedirectResult)result).Url);
        }

        [Fact]
        public async Task EditUserPostRedirectsToCorrectAction()
        {
            var mediator = new Mock<IMediator>();

            var userId = It.IsAny<string>();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId)))
                .ReturnsAsync(new ApplicationUser());
            var controller = new SiteController(null, null, mediator.Object);

            var result = await controller.EditUser(new EditUserViewModel());

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Fact]
        public void EditUserPostHasHttpPostAttribute()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributesOn(x => x.EditUser(It.IsAny<EditUserViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void EditUserPostHasValidateAntiForgeryTokenAttribute()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributesOn(x => x.EditUser(It.IsAny<EditUserViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task UnlockUserWithUnknownUserIdRedirectsToCorrectAction()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = (RedirectToActionResult)await controller.UnlockUser("DontKnowThisId");
            Assert.Equal("Index", result.ActionName);
            userManager.Verify(u => u.SetLockoutEndDateAsync(It.IsAny<ApplicationUser>(), It.IsAny<DateTimeOffset>()), Times.Never);
        }

        [Fact]
        public async Task UnlockUserSendsUserQueryWithCorrectUserId()
        {
            var dateTimeNow = new DateTime(2018, 1, 1);
            var tomorrow = new DateTime(2018, 1, 2);

            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser { Id = "foo_id", LockoutEnd = tomorrow };
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id))).ReturnsAsync(user);
            var userManager = CreateApplicationUserMock();

            var controller = new SiteController(userManager.Object, null, mediator.Object) { DateTimeNow = () => dateTimeNow };
            await controller.UnlockUser(user.Id);

            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)), Times.Once);
        }

        [Fact]
        public async Task UnlockUserInvokesSetLockoutEndDateAsyncWithCorrectUserAndDateWhenCorrectLockoutEndAndRedirectsToCorrectAction()
        {
            var dateTimeNow = new DateTime(2018, 1, 1);
            var yesterday = new DateTime(2017, 12, 31);
            var tomorrow = new DateTime(2018, 1, 2);

            var mediator = new Mock<IMediator>();
            // TODO => Mock out datetime properly
            var user = new ApplicationUser { Id = "foo_id", LockoutEnd = tomorrow };
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id))).ReturnsAsync(user);
            var userManager = CreateApplicationUserMock();

            var controller = new SiteController(userManager.Object, null, mediator.Object) { DateTimeNow = () => dateTimeNow };
            var result = (RedirectToActionResult)await controller.UnlockUser(user.Id);

            Assert.Equal("Index", result.ActionName);
            userManager.Verify(u => u.SetLockoutEndDateAsync(user, yesterday));
        }

        [Fact]
        public async Task UnlockUserDoesNotInvokeSetLockoutEndDateAsyncWhenWrongLockoutEnd()
        {
            var dateTimeNow = new DateTime(2018, 1, 1);
            var yesterday = new DateTime(2017, 12, 31);

            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser { Id = "foo_id", LockoutEnd = yesterday };
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id))).ReturnsAsync(user);
            var userManager = CreateApplicationUserMock();

            var controller = new SiteController(userManager.Object, null, mediator.Object) { DateTimeNow = () => dateTimeNow };
            await controller.UnlockUser(user.Id);

            userManager.Verify(u => u.SetLockoutEndDateAsync(user, yesterday), Times.Never);
        }

        [Fact]
        public async Task ResetPasswordSendsUserByUserIdQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            var userId = It.IsAny<string>();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).ReturnsAsync(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.ResetPassword(userId);
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordAddsCorrectErrorMessageToViewBagWhenUserIsNull()
        {
            var mediator = new Mock<IMediator>();
            var userId = "1234";
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).ReturnsAsync((ApplicationUser)null);

            var controller = new SiteController(null, null, mediator.Object);
            await controller.ResetPassword(userId);
            Assert.Equal("User not found.", controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public async Task ResetPasswordInvokesGeneratePasswordResetTokenAsyncWithCorrectUserWhenUserIsNotNull()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser
            {
                Id = "1234",
                Email = "test@testy.com"
            };

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .ReturnsAsync(user);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            controller.SetDefaultHttpContext();
            controller.Url = GetMockUrlHelper("any");
            await controller.ResetPassword(user.Id);

            userManager.Verify(u => u.GeneratePasswordResetTokenAsync(user));
        }

        [Fact]
        public async Task ResetPasswordInvokesUrlActionWithCorrectParametersWhenUserIsNotNull()
        {
            const string requestScheme = "requestScheme";
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser
            {
                Id = "1234",
                Email = "test@testy.com"
            };

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .ReturnsAsync(user);
            var code = "passcode";
            userManager.Setup(u => u.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(code);
            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var urlHelper = new Mock<IUrlHelper>();
            controller.Url = urlHelper.Object;
            controller.SetFakeHttpRequestSchemeTo(requestScheme);
            await controller.ResetPassword(user.Id);

            urlHelper.Verify(mock => mock.Action(It.Is<UrlActionContext>(uac =>
                uac.Action == "ResetPassword" &&
                uac.Controller == "Account" &&
                uac.Protocol == requestScheme)),
                Times.Once);
        }

        [Fact]
        public async Task ResetPasswordSendsSendResetPasswordEmailWithCorrectDataWhenUserIsNotNull()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser
            {
                Id = "1234",
                Email = "test@testy.com"
            };

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .ReturnsAsync(user);
            var code = "passcode";
            userManager.Setup(u => u.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(code);
            var url = $"Admin/ResetPassword?userId={user.Id}&code={code}";
            var controller = new SiteController(userManager.Object, null, mediator.Object);
            controller.SetDefaultHttpContext();
            controller.Url = GetMockUrlHelper(url);
            await controller.ResetPassword(user.Id);

            mediator.Verify(x => x.SendAsync(It.Is<AllReady.Areas.Admin.Features.Site.SendResetPasswordEmailCommand>(e => e.Email == user.Email && e.CallbackUrl == url)));
        }

        [Fact]
        public async Task ResetPasswordAddsCorrectSuccessMessagetoViewBagWhenUserIsNotNull()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser
            {
                Id = "1234",
                Email = "test@testy.com",
                UserName = "auser"
            };

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .ReturnsAsync(user);
            var code = "passcode";
            userManager.Setup(u => u.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(code);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            controller.SetDefaultHttpContext();
            controller.Url = GetMockUrlHelper("any");
            await controller.ResetPassword(user.Id);
            Assert.Equal($"Sent password reset email for {user.UserName}.", controller.ViewBag.SuccessMessage);
        }

        [Fact]
        public async Task ResetPasswordReturnsAView()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser
            {
                Id = "1234",
                Email = "test@testy.com",
                UserName = "auser"
            };

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .ReturnsAsync(user);
            var code = "passcode";
            userManager.Setup(u => u.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(code);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            controller.SetDefaultHttpContext();
            controller.Url = GetMockUrlHelper("any");
            var result = (ViewResult)await controller.ResetPassword(user.Id);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ResetPasswordInvokesLogErrorWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>()))
                .Throws<Exception>();

            var controller = new SiteController(null, logger.Object, mediator.Object);
            await controller.ResetPassword("1234");

            logger.Verify(l => l.Log<object>(LogLevel.Error, 0,
                It.IsAny<Microsoft.Extensions.Logging.Internal.FormattedLogValues>(), null,
                It.IsAny<Func<object, Exception, string>>()));
        }

        [Fact]
        public async Task ResetPasswordAddsCorrectErrorMessagetoViewBagWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>()))
                .Throws<Exception>();

            var controller = new SiteController(null, logger.Object, mediator.Object);
            var userId = "1234";
            await controller.ResetPassword(userId);

            Assert.Equal($"Failed to reset password for {userId}. Exception thrown.", controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public async Task ResetPasswordReturnsAViewWhenExcpetionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>()))
                .Throws<Exception>();

            var controller = new SiteController(null, logger.Object, mediator.Object);
            var userId = "1234";
            var result = (ViewResult)await controller.ResetPassword(userId);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ResetPasswordHasHttpGetAttribute()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributesOn(x => x.ResetPassword(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task AssignSiteAdminSendsUserByUserIdQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            var userId = It.IsAny<string>();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).ReturnsAsync(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.AssignSiteAdmin(userId);
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task AssignApiRoleQueriesForCorrectId()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            var userId = Guid.NewGuid().ToString();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).ReturnsAsync(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.AssignApiAccessRole(userId);
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task SearchForCorrectUserWhenManagingApiKeys()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            var userId = Guid.NewGuid().ToString();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).ReturnsAsync(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.ManageApiKeys(userId);
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task SearchForCorrectUserWhenGeneratingApiKeys()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            var userId = Guid.NewGuid().ToString();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).ReturnsAsync(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.GenerateToken(userId);
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task AssignSiteAdminInvokesAddClaimAsyncWithCorrrectParameters()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser { Id = "1234" };

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .ReturnsAsync(user);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            await controller.AssignSiteAdmin(user.Id);

            userManager.Verify(x => x.AddClaimAsync(user, It.Is<Claim>(c =>
                c.Value == nameof(UserType.SiteAdmin) &&
                c.Type == AllReady.Security.ClaimTypes.UserType)), Times.Once);
        }

        [Fact]
        public async Task AssignSiteAdminRedirectsToCorrectAction()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser
            {
                Id = "1234"
            };

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id))).ReturnsAsync(user);
            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = (RedirectToActionResult)await controller.AssignSiteAdmin(user.Id);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task AssignSiteAdminInvokesLogErrorWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            var user = new ApplicationUser
            {
                Id = "1234"
            };

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Throws<Exception>();

            var controller = new SiteController(null, logger.Object, mediator.Object);
            await controller.AssignSiteAdmin(user.Id);

            logger.Verify(l => l.Log<object>(LogLevel.Error, 0,
                It.IsAny<Microsoft.Extensions.Logging.Internal.FormattedLogValues>(), null,
                It.IsAny<Func<object, Exception, string>>()));
        }

        [Fact]
        public async Task AssignSiteAdminAddsCorrectErrorMessageToViewBagWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser
            {
                Id = "1234"
            };

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Throws<Exception>();

            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            await controller.AssignSiteAdmin(user.Id);

            Assert.Equal($"Failed to assign site admin for {user.Id}. Exception thrown.", controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public async Task AssignSiteAdminReturnsAViewWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser
            {
                Id = "1234"
            };

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Throws<Exception>();

            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            var result = await controller.AssignSiteAdmin(user.Id);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AssignSiteAdminHasHttpGetAttribute()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributesOn(x => x.AssignSiteAdmin(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task AssignOrganizationAdminGetSendsUserByUserIdQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            var userId = "1234";
            var controller = new SiteController(null, null, mediator.Object);
            await controller.AssignOrganizationAdmin(userId);
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task AssignOrganizationAdminGetRedirectsToCorrectActionWhenUserIsAnOrganizationAdmin()
        {
            var user = new ApplicationUser { Id = "1234" };
            user.MakeOrgAdmin();

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .ReturnsAsync(user);

            var controller = new SiteController(null, null, mediator.Object);
            var result = (RedirectToActionResult)await controller.AssignOrganizationAdmin(user.Id);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task AssignOrganizationAdminGetRedirectsToCorrectActionWhenUserIsASiteAdmin()
        {
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser
            {
                Id = "1234"
            };
            user.Claims.Add(new IdentityUserClaim<string>
            {
                ClaimType = AllReady.Security.ClaimTypes.UserType,
                ClaimValue = nameof(UserType.SiteAdmin)
            });

            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .ReturnsAsync(user);

            var controller = new SiteController(null, null, mediator.Object);
            var result = (RedirectToActionResult)await controller.AssignOrganizationAdmin(user.Id);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task AssignOrganizationAdminGetSendsAllOrganizationsQuery()
        {
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser
            {
                Id = "1234"
            };
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .ReturnsAsync(user);

            var controller = new SiteController(null, null, mediator.Object);
            await controller.AssignOrganizationAdmin(user.Id);
            mediator.Verify(m => m.SendAsync(It.IsAny<AllOrganizationsQuery>()), Times.Once);
        }

        [Fact]
        public async Task AssignOrganizationAdminGetAddsCorrectSelectListItemToOrganizationsOnViewBag()
        {
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser
            {
                Id = "1234"
            };
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .ReturnsAsync(user);
            var orgs = new List<Organization>
            {
                new Organization { Id = 2, Name = "Borg" },
                new Organization { Id = 1, Name = "Aorg" }
            };
            mediator.Setup(x => x.SendAsync(It.IsAny<AllOrganizationsQuery>())).ReturnsAsync(orgs);
            var controller = new SiteController(null, null, mediator.Object);
            await controller.AssignOrganizationAdmin(user.Id);

            var viewBagOrgs = ((IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>)controller.ViewBag.Organizations).ToList();

            Assert.Equal(3, viewBagOrgs.Count);
            Assert.Equal("0", viewBagOrgs[0].Value); // Select One item added in controller
            Assert.Equal("1", viewBagOrgs[1].Value); // sorted items
            Assert.Equal("2", viewBagOrgs[2].Value);
        }

        [Fact]
        public async Task AssignOrganizationAdminGetReturnsCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser
            {
                Id = "1234"
            };
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .ReturnsAsync(user);
            var orgs = new List<Organization>
            {
                new Organization { Id = 2, Name = "Borg" },
                new Organization { Id = 1, Name = "Aorg" }
            };
            mediator.Setup(x => x.SendAsync(It.IsAny<AllOrganizationsQuery>())).ReturnsAsync(orgs);
            var controller = new SiteController(null, null, mediator.Object);
            var result = (ViewResult)await controller.AssignOrganizationAdmin(user.Id);

            Assert.IsType<AssignOrganizationAdminViewModel>(result.Model);
        }

        [Fact]
        public void AssignOrganizationAdminGetHasHttpGetAttribute()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributesOn(x => x.AssignOrganizationAdmin(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostSendsUserByUserIdQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            var model = new AssignOrganizationAdminViewModel
            {
                UserId = "1234"
            };

            var controller = new SiteController(null, null, mediator.Object);
            await controller.AssignOrganizationAdmin(model);

            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)), Times.Once);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostRedirectsToCorrectActionIsUserIsNull()
        {
            var mediator = new Mock<IMediator>();
            var model = new AssignOrganizationAdminViewModel
            {
                UserId = "1234"
            };
            ApplicationUser nullUser = null;
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).ReturnsAsync(nullUser);
            var controller = new SiteController(null, null, mediator.Object);
            var result = (RedirectToActionResult)await controller.AssignOrganizationAdmin(model);

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostAddsCorrectKeyAndErrorMessageToModelStateWhenOrganizationIdIsZero()
        {
            var mediator = new Mock<IMediator>();
            var model = new AssignOrganizationAdminViewModel
            {
                UserId = "1234"
            };
            var user = new ApplicationUser { UserName = "name" };
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).ReturnsAsync(user);
            var controller = new SiteController(null, null, mediator.Object);
            await controller.AssignOrganizationAdmin(model);

            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("OrganizationId"));
            Assert.True(controller.ModelState.GetErrorMessagesByKey("OrganizationId").FirstOrDefault(x => x.ErrorMessage.Equals("You must pick a valid organization.")) != null);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostSendsAllOrganizationsQueryWhenModelStateIsValid()
        {
            var mediator = new Mock<IMediator>();
            var model = new AssignOrganizationAdminViewModel
            {
                UserId = "1234",
                OrganizationId = 5678
            };
            var user = new ApplicationUser { UserName = "name" };
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).ReturnsAsync(user);
            var controller = new SiteController(null, null, mediator.Object);
            await controller.AssignOrganizationAdmin(model);

            mediator.Verify(x => x.SendAsync(It.IsAny<AllOrganizationsQuery>()), Times.Once);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostInvokesAddClaimAsyncTwiceAddingTheCorrectClaimsWhenOrganizationIdOnModelMatchesAnyExistingOrganization()
        {
            var model = new AssignOrganizationAdminViewModel
            {
                UserId = "1234",
                OrganizationId = 5678
            };

            var orgs = new List<Organization> { new Organization { Id = 5678, Name = "Borg" } };

            var user = new ApplicationUser { UserName = "name" };

            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).ReturnsAsync(user);
            mediator.Setup(x => x.SendAsync(It.IsAny<AllOrganizationsQuery>())).ReturnsAsync(orgs);

            var userManager = CreateApplicationUserMock();

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            await controller.AssignOrganizationAdmin(model);

            userManager.Verify(x => x.AddClaimAsync(user, It.Is<Claim>(c =>
                c.Value == nameof(UserType.OrgAdmin) &&
                c.Type == AllReady.Security.ClaimTypes.UserType)), Times.Once);
            userManager.Verify(x => x.AddClaimAsync(user, It.Is<Claim>(c =>
                c.Value == model.OrganizationId.ToString() &&
                c.Type == AllReady.Security.ClaimTypes.Organization)), Times.Once);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostRedirectsToCorrectActionWhenOrganizationIdOnModelMatchesAnyExistingOrganization()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var model = new AssignOrganizationAdminViewModel
            {
                UserId = "1234",
                OrganizationId = 5678
            };
            var user = new ApplicationUser { UserName = "name" };
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).ReturnsAsync(user);
            var orgs = new List<Organization> { new Organization { Id = 5678, Name = "Borg" } };
            mediator.Setup(x => x.SendAsync(It.IsAny<AllOrganizationsQuery>())).ReturnsAsync(orgs);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = (RedirectToActionResult)await controller.AssignOrganizationAdmin(model);

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostAddsCorrectKeyAndErrorMessageToModelStateWhenOrganzationNotFound()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var model = new AssignOrganizationAdminViewModel
            {
                UserId = "1234",
                OrganizationId = 5678
            };
            var user = new ApplicationUser { UserName = "name" };
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).ReturnsAsync(user);
            var orgs = new List<Organization> { new Organization { Id = 9123, Name = "Borg" } };
            mediator.Setup(x => x.SendAsync(It.IsAny<AllOrganizationsQuery>())).ReturnsAsync(orgs);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            await controller.AssignOrganizationAdmin(model);

            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("OrganizationId"));
            Assert.True(controller.ModelState.GetErrorMessagesByKey("OrganizationId").FirstOrDefault(x => x.ErrorMessage.Equals("Invalid Organization. Please contact support.")) != null);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostReturnsAView()
        {
            var mediator = new Mock<IMediator>();
            var model = new AssignOrganizationAdminViewModel
            {
                UserId = "1234",
                OrganizationId = 0
            };
            var user = new ApplicationUser { UserName = "name" };
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).ReturnsAsync(user);

            var controller = new SiteController(null, null, mediator.Object);
            var result = await controller.AssignOrganizationAdmin(model);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AssignOrganizationAdminPostHasHttpPostAttribute()
        {
            var mediator = new Mock<IMediator>();
            var controller = new SiteController(null, null, mediator.Object);

            var model = new AssignOrganizationAdminViewModel { UserId = It.IsAny<string>(), OrganizationId = It.IsAny<int>() };

            var attribute = controller.GetAttributesOn(x => x.AssignOrganizationAdmin(model)).OfType<HttpPostAttribute>().SingleOrDefault();

            Assert.NotNull(attribute);
        }

        [Fact]
        public void AssignOrganizationAdminPostHasValidateAntiForgeryTokenAttribute()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributesOn(x => x.AssignOrganizationAdmin(It.IsAny<AssignOrganizationAdminViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task RevokeSiteAdminSendsUserByUserIdQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            var userId = It.IsAny<string>();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).ReturnsAsync(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.RevokeSiteAdmin(userId);
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task RevokeSiteAdminInvokesRemoveClaimAsyncWithCorrectParameters()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var userId = "1234";
            var user = new ApplicationUser { UserName = "name", Id = userId };
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).ReturnsAsync(user);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            await controller.RevokeSiteAdmin(userId);

            userManager.Verify(u => u.RemoveClaimAsync(user, It.Is<Claim>(c =>
                c.Type == AllReady.Security.ClaimTypes.UserType
                && c.Value == nameof(UserType.SiteAdmin))), Times.Once);
        }

        [Fact]
        public async Task RevokeSiteAdminRedirectsToCorrectAction()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            mediator.Setup(m => m.SendAsync(It.IsAny<UserByUserIdQuery>())).ReturnsAsync(new ApplicationUser());

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = (RedirectToActionResult)await controller.RevokeSiteAdmin(It.IsAny<string>());

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task RevokeSiteAdminInvokesLogErrorWithCorrectParametersWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            var userId = "1234";
            var thrown = new Exception("Test");
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>()))
                .Throws(thrown);

            var controller = new SiteController(null, logger.Object, mediator.Object);
            await controller.RevokeSiteAdmin(userId);

            string expectedMessage = $"Failed to revoke site admin for {userId}";

            logger.Verify(l => l.Log(LogLevel.Error, 0,
                It.Is<Microsoft.Extensions.Logging.Internal.FormattedLogValues>(x => x.ToString().Equals(expectedMessage)),
                null, It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task RevokeSiteAdminAddsCorrectErrorMessageToViewBagWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            var userId = "1234";
            var thrown = new Exception("Test");
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>()))
                .Throws(thrown);

            var controller = new SiteController(null, logger.Object, mediator.Object);
            await controller.RevokeSiteAdmin(userId);
            string expectedMessage = $"Failed to revoke site admin for {userId}. Exception thrown.";

            Assert.Equal(expectedMessage, controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public async Task RevokeSiteAdminReturnsAViewWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            var userId = "1234";
            var thrown = new Exception("Test");
            mediator.Setup(x => x.SendAsync(It.IsAny<UserByUserIdQuery>()))
                .Throws(thrown);

            var controller = new SiteController(null, logger.Object, mediator.Object);
            var result = await controller.RevokeSiteAdmin(userId);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void RevokeSiteAdminHasHttpGetAttribute()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributesOn(x => x.RevokeSiteAdmin(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task RevokeOrganizationAdminSendsUserByUserIdQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            var userId = It.IsAny<string>();
            mediator.Setup(x => x.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).ReturnsAsync(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.RevokeOrganizationAdmin(userId);
            mediator.Verify(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task RevokeOrganizationAdminInvokesGetclaimsAsyncWithCorrectUser()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var logger = new Mock<ILogger<SiteController>>();
            var userId = "1234";
            var user = new ApplicationUser { UserName = "name", Id = userId };
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).ReturnsAsync(user);

            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            await controller.RevokeOrganizationAdmin(userId);

            userManager.Verify(u => u.GetClaimsAsync(user), Times.Once);
        }

        [Fact]
        public async Task RevokeOrganizationAdminInokesRemoveClaimAsyncTwiceWithCorrectParameters()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var logger = new Mock<ILogger<SiteController>>();
            var userId = "1234";
            var user = new ApplicationUser { UserName = "name", Id = userId };
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).ReturnsAsync(user);

            var orgId = "4567";
            IList<Claim> claims = new List<Claim>();
            claims.Add(new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.SiteAdmin)));
            claims.Add(new Claim(AllReady.Security.ClaimTypes.Organization, orgId));
            userManager.Setup(u => u.GetClaimsAsync(user)).ReturnsAsync(claims);

            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            await controller.RevokeOrganizationAdmin(userId);

            userManager.Verify(u => u.RemoveClaimAsync(user, It.Is<Claim>(c =>
                c.Type == AllReady.Security.ClaimTypes.UserType
                && c.Value == nameof(UserType.SiteAdmin))), Times.Once);
            userManager.Verify(u => u.RemoveClaimAsync(user, It.Is<Claim>(c =>
                c.Type == AllReady.Security.ClaimTypes.Organization
                && c.Value == orgId)), Times.Once);
        }

        [Fact]
        public async Task RevokeOrganizationAdminRedirectsToCorrectAction()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var logger = new Mock<ILogger<SiteController>>();
            var userId = "1234";
            var user = new ApplicationUser { UserName = "name", Id = userId };
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).ReturnsAsync(user);

            var orgId = "4567";
            IList<Claim> claims = new List<Claim>();
            claims.Add(new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.SiteAdmin)));
            claims.Add(new Claim(AllReady.Security.ClaimTypes.Organization, orgId));
            userManager.Setup(u => u.GetClaimsAsync(user)).ReturnsAsync(claims);

            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            var result = (RedirectToActionResult)await controller.RevokeOrganizationAdmin(userId);

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task RevokeOrganizationAdminInvokesLogErrorWithCorrectParametersWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var logger = new Mock<ILogger<SiteController>>();
            var userId = "1234";
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Throws<Exception>();
            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            await controller.RevokeOrganizationAdmin(userId);

            string expectedMessage = $"Failed to revoke organization admin for {userId}";

            logger.Verify(l => l.Log(LogLevel.Error, 0,
                It.Is<Microsoft.Extensions.Logging.Internal.FormattedLogValues>(x => x.ToString().Equals(expectedMessage)),
                null, It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task RevokeOrganizationAdminAddsCorrectErrorMessageToViewBagWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var logger = new Mock<ILogger<SiteController>>();
            var userId = "1234";
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Throws<Exception>();
            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            await controller.RevokeOrganizationAdmin(userId);

            string expectedMessage = $"Failed to revoke organization admin for {userId}. Exception thrown.";

            Assert.Equal(controller.ViewBag.ErrorMessage, expectedMessage);
        }

        [Fact]
        public async Task RevokeOrganizationAdminReturnsAViewWhenErrorIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var logger = new Mock<ILogger<SiteController>>();
            var userId = "1234";
            mediator.Setup(m => m.SendAsync(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Throws<Exception>();
            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            var result = (ViewResult)await controller.RevokeOrganizationAdmin(userId);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void RevokeOrganizationAdminHasHttpGetAttribute()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributesOn(x => x.RevokeOrganizationAdmin(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributes().OfType<AreaAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(AreaNames.Admin, attribute.RouteValue);
        }

        [Fact]
        public void ControllerHasAuthorizeAtttributeWithTheCorrectPolicy()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(nameof(UserType.SiteAdmin), attribute.Policy);
        }

        private static Mock<UserManager<ApplicationUser>> CreateApplicationUserMock()
        {
            return new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        }

        private static IUrlHelper GetMockUrlHelper(string returnValue)
        {
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(o => o.Action(It.IsAny<UrlActionContext>())).Returns(returnValue);
            return urlHelper.Object;
        }
    }
}
