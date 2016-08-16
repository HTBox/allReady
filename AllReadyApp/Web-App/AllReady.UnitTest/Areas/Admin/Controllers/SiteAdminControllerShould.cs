using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Features.Site;
using AllReady.Areas.Admin.Features.Users;
using AllReady.Areas.Admin.Models;
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
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class SiteAdminControllerShould
    {
        [Fact]
        public void IndexReturnsCorrectViewModel()
        {

            var mediator = new Mock<IMediator>();
            var users = new List<ApplicationUser>() {
                new ApplicationUser {
                    Id = It.IsAny<string>()
                },
                new ApplicationUser {
                    Id = It.IsAny<string>()
                }
            };
            mediator.Setup(x => x.Send(It.IsAny<AllUsersQuery>())).Returns(users);

            var controller = new SiteController(null, null, mediator.Object);
            var result = controller.Index();
            var model = ((ViewResult)result).ViewData.Model as SiteAdminViewModel;
            
            Assert.Equal(model.Users.Count(), users.Count());
            Assert.IsType<SiteAdminViewModel>(model);
        }

        [Fact]
        public async Task DeleteUserSendsUserQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            
            const string userId = "foo_id";
            mediator.Setup(x => x.SendAsync(It.Is<UserQuery>(q => q.UserId == userId))).ReturnsAsync(new EditUserViewModel());
            var controller = new SiteController(null, null, mediator.Object);

            await controller.DeleteUser(userId);
            mediator.Verify(m =>m.SendAsync(It.Is<UserQuery>(q =>q.UserId == userId)), Times.Once);
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

            Assert.Equal(result.ActionName, nameof(SiteController.Index));
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
        public void EditUserGetSendsUserByUserIdQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            string userId = It.IsAny<string>();
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId)))
                .Returns(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            controller.EditUser(userId);
            mediator.Verify(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public void EditUserGetReturnsCorrectViewModelWhenOrganizationIdIsNull()
        {
            {
                var mediator = new Mock<IMediator>();                

                string userId = It.IsAny<string>();
                mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId)))
                                .Returns(new ApplicationUser());
                var controller = new SiteController(null, null, mediator.Object);

                var result = controller.EditUser(userId);
                var model = ((ViewResult)result).ViewData.Model as EditUserViewModel;

                Assert.Equal(model.Organization, null);
                Assert.IsType<EditUserViewModel>(model);
            }
        }

        [Fact]
        public void EditUserGetReturnsCorrectValueForOrganiztionOnViewModelWhenOrganizationIdIsNotNull()
        {
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser();
            int orgId = 99;
            string orgName = "Test Org";
            var org = new Organization() { Id = orgId, Name = orgName };
            string userId = It.IsAny<string>();
            user.Claims.Add(new Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>()
            {
                ClaimType = AllReady.Security.ClaimTypes.Organization,
                ClaimValue = orgId.ToString()
            });
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId)))
                            .Returns(user);

            mediator.Setup(x => x.Send(It.Is<OrganizationByIdQuery>(q => q.OrganizationId == orgId)))
                            .Returns(org);

            var controller = new SiteController(null, null, mediator.Object);

            var result = controller.EditUser(userId);
            var model = ((ViewResult)result).ViewData.Model as EditUserViewModel;

            Assert.NotNull(model.Organization);
            Assert.IsType<EditUserViewModel>(model);
        }

        [Fact]
        public async Task EditUserPostReturnsSameViewAndViewModelWhenModelStateIsInvalid()
        {
            EditUserModel model = new EditUserModel()
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
            EditUserViewModel model = new EditUserViewModel()
            {
                UserId = "1234",
            };
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)))
                .Returns(new ApplicationUser());
            var controller = new SiteController(null, null, mediator.Object);

            await controller.EditUser(model);
            mediator.Verify(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)), Times.Once);
        }

        [Fact]
        public async Task EditUserPostSendsUpdateUserWithCorrectUserWhenUsersAssociatedSkillsAreNotNull()
        {
            var mediator = new Mock<IMediator>();
            EditUserViewModel model = new EditUserViewModel()
            {
                UserId = It.IsAny<string>(),
                AssociatedSkills = new List<UserSkill>() { new UserSkill() {Skill = It.IsAny<Skill>() } }
            };
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)))
                .Returns(new ApplicationUser());
            var controller = new SiteController(null, null, mediator.Object);

            await controller.EditUser(model);
            mediator.Verify(m => m.SendAsync(It.Is<UpdateUser>(q => q.User.AssociatedSkills[0].Skill == model.AssociatedSkills[0].Skill)), Times.Once);
        }

        [Fact]
        public async Task EditUserPostInvokesUpdateUserWithCorrectUserWhenUsersAssociatedSkillsAreNotNullAndThereIsAtLeastOneAssociatedSkillForTheUser()
        {
            var mediator = new Mock<IMediator>();
            EditUserViewModel model = new EditUserViewModel()
            {
                UserId = It.IsAny<string>(),
                AssociatedSkills = new List<UserSkill>() { new UserSkill() { SkillId = 1, Skill = new Skill() { Id = 1 } } }
            };
            var user = new ApplicationUser()
            {
                Id = model.UserId,
                AssociatedSkills = new List<UserSkill>() { new UserSkill() { SkillId = 2, Skill = new Skill() { Id = 2 } } }
            };
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)))
                .Returns(user);

            var controller = new SiteController(null, null, mediator.Object);

            await controller.EditUser(model);
            mediator.Verify(m => m.SendAsync(It.Is<UpdateUser>(q => q.User.AssociatedSkills[0] == model.AssociatedSkills[0])), Times.Once);
        }

        [Fact]
        public async Task EditUserPostInvokesAddClaimAsyncWhenModelsIsOrganizationAdminIsTrue()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            EditUserViewModel model = new EditUserViewModel()
            {
                IsOrganizationAdmin = true,
                UserId = It.IsAny<string>()
                
            };
            var user = new ApplicationUser()
            {
                Id = model.UserId,
                Email = "test@testy.com"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)))
                .Returns(user);
            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()))
                .Returns(() => Task.FromResult(IdentityResult.Success));

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            controller.SetDefaultHttpContext();
            controller.Url = GetMockUrlHelper("any");
            await controller.EditUser(model);

            userManager.Verify(x => x.AddClaimAsync(user, It.Is<Claim>(c => c.Value == "OrgAdmin")), Times.Once);
        }

        [Fact]
        public async Task EditUserPostInvokesUrlActionWithCorrectParametersWhenModelsIsOrganizationAdminIsTrueAndOrganizationAdminClaimWasAddedSuccessfully()
        {
            const string requestScheme = "requestScheme";
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            EditUserModel model = new EditUserModel()
            {
                IsOrganizationAdmin = true,
                UserId = It.IsAny<string>()

            };
            var user = new ApplicationUser()
            {
                Id = model.UserId,
                Email = "test@testy.com"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)))
                .Returns(user);
            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()))
                .Returns(() => Task.FromResult(IdentityResult.Success));

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            controller.SetFakeHttpRequestSchemeTo(requestScheme);
            var urlHelper = new Mock<IUrlHelper>();
            controller.Url = urlHelper.Object;
            await controller.EditUser(model);

            urlHelper.Verify(mock => mock.Action(It.Is<UrlActionContext>(uac =>
                uac.Action == "Login" &&
                uac.Controller == "Admin" &&
                uac.Protocol == requestScheme)),
                Times.Once);
        }

        [Fact]
        public async Task EditUserPostSendsSendAccountApprovalEmailWithCorrectDataWhenModelsIsOrganizationAdminIsTrueAndOrganizationAdminClaimWasAddedSuccessfully()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            EditUserViewModel model = new EditUserViewModel()
            {
                IsOrganizationAdmin = true,
                UserId = It.IsAny<string>()

            };
            var user = new ApplicationUser()
            {
                Id = model.UserId,
                Email = "test@testy.com"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)))
                .Returns(user);
            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()))
                .Returns(() => Task.FromResult(IdentityResult.Success));

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            controller.SetDefaultHttpContext();
            var expectedUrl = String.Format("Login/Admin?Email={0}", user.Email);
            controller.Url = GetMockUrlHelper(expectedUrl);
            await controller.EditUser(model);

            mediator.Verify(m => m.SendAsync(It.Is<SendAccountApprovalEmail>(q => q.Email == user.Email && q.CallbackUrl == expectedUrl ))
                            , Times.Once);
        }

        [Fact]
        public async Task EditUserPostReturnsRedirectResultWithCorrectUrlWhenModelsIsOrganizationAdminIsTrueAndOrganizationAdminClaimWasNotAddedSuccessfully()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            EditUserModel model = new EditUserModel()
            {
                IsOrganizationAdmin = true,
                UserId = It.IsAny<string>()

            };
            var user = new ApplicationUser()
            {
                Id = model.UserId,
                Email = "test@testy.com"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)))
                .Returns(user);
            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()))
                .Returns(() => Task.FromResult( IdentityResult.Failed(null)));

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = await controller.EditUser(model);

            Assert.IsType<RedirectResult>(result);
            Assert.Equal("Error", ((RedirectResult)result).Url);
        }

        [Fact]
        public async Task EditUserPostInvokesRemoveClaimAsyncWithCorrectParametersWhenUserIsAnOrganizationAdmin()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            EditUserModel model = new EditUserModel()
            {
                IsOrganizationAdmin = false,
                UserId = It.IsAny<string>()
            };
            var user = new ApplicationUser()
            {
                Id = model.UserId,
                Email = "test@testy.com"
            };
            user.Claims.Add(new Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>()
                                    {
                                        ClaimType = AllReady.Security.ClaimTypes.UserType,
                                        ClaimValue = Enum.GetName(typeof(UserType), UserType.OrgAdmin)
                                    });

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)))
                .Returns(user);
            userManager.Setup(x => x.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()))
                .Returns(() => Task.FromResult(IdentityResult.Success));

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = await controller.EditUser(model);

            userManager.Verify(x => x.RemoveClaimAsync(user, It.Is<Claim>(c => c.Value == "OrgAdmin")), Times.Once);
        }

        [Fact]
        public async Task EditUserPostReturnsRedirectResultWithCorrectUrlWhenUserIsAnOrganizationAdminAndRemovClaimAsyncDoesNotSucceed()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            EditUserModel model = new EditUserModel()
            {
                IsOrganizationAdmin = false,
                UserId = It.IsAny<string>()
            };
            var user = new ApplicationUser()
            {
                Id = model.UserId,
                Email = "test@testy.com"
            };
            user.Claims.Add(new Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>()
            {
                ClaimType = AllReady.Security.ClaimTypes.UserType,
                ClaimValue = Enum.GetName(typeof(UserType), UserType.OrgAdmin)
            });

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)))
                .Returns(user);
            userManager.Setup(x => x.RemoveClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()))
                .Returns(() => Task.FromResult(IdentityResult.Failed(null)));

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = await controller.EditUser(model);

            Assert.IsType<RedirectResult>(result);
            Assert.Equal("Error", ((RedirectResult)result).Url);
        }

        [Fact]
        public async Task EditUserPostRedirectsToCorrectAction()
        {
            var mediator = new Mock<IMediator>();

            string userId = It.IsAny<string>();
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId)))
                            .Returns(new ApplicationUser());
            var controller = new SiteController(null, null, mediator.Object);

            var result = await controller.EditUser(new EditUserModel());

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
        public async Task ResetPasswordSendsUserByUserIdQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            string userId = It.IsAny<string>();
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Returns(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.ResetPassword(userId);
            mediator.Verify(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordAddsCorrectErrorMessageToViewBagWhenUserIsNull()
        {
            var mediator = new Mock<IMediator>();
            string userId = "1234";
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Returns<ApplicationUser>(null); 

            var controller = new SiteController(null, null, mediator.Object);
            var result = await controller.ResetPassword(userId);
            Assert.Equal("User not found.", controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public async Task ResetPasswordInvokesGeneratePasswordResetTokenAsyncWithCorrectUserWhenUserIsNotNull()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser()
            {
                Id = "1234",
                Email = "test@testy.com"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Returns(user);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            controller.SetDefaultHttpContext();
            controller.Url = GetMockUrlHelper("any");
            var result = await controller.ResetPassword(user.Id);

            userManager.Verify(u => u.GeneratePasswordResetTokenAsync(user));
        }

        [Fact]
        public async Task ResetPasswordInvokesUrlActionWithCorrectParametersWhenUserIsNotNull()
        {
            const string requestScheme = "requestScheme";
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser()
            {
                Id = "1234",
                Email = "test@testy.com"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Returns(user);
            string code = "passcode";
            userManager.Setup(u => u.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(code);
            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var urlHelper = new Mock<IUrlHelper>();
            controller.Url = urlHelper.Object;
            controller.SetFakeHttpRequestSchemeTo(requestScheme);
            var result = await controller.ResetPassword(user.Id);

            urlHelper.Verify(mock => mock.Action(It.Is<UrlActionContext>(uac =>
                uac.Action == "ResetPassword" &&
                uac.Controller == "Admin" &&
                uac.Protocol == requestScheme)),
                Times.Once);
        }

        [Fact]
        public async Task ResetPasswordSendsSendResetPasswordEmailWithCorrectDataWhenUserIsNotNull()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser()
            {
                Id = "1234",
                Email = "test@testy.com"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Returns(user);
            string code = "passcode";
            userManager.Setup(u => u.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(code);
            string url = String.Format("Admin/ResetPassword?userId={0}&code={1}", user.Id, code);
            var controller = new SiteController(userManager.Object, null, mediator.Object);
            controller.SetDefaultHttpContext();
            controller.Url = GetMockUrlHelper(url);
            var result = await controller.ResetPassword(user.Id);

            mediator.Verify(x => x.SendAsync(It.Is<AllReady.Areas.Admin.Features.Site.SendResetPasswordEmail>(e => e.Email == user.Email && e.CallbackUrl == url)));
        }

        [Fact]
        public async Task ResetPasswordAddsCorrectSuccessMessagetoViewBagWhenUserIsNotNull()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser()
            {
                Id = "1234",
                Email = "test@testy.com",
                UserName = "auser"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Returns(user);
            string code = "passcode";
            userManager.Setup(u => u.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(code);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            controller.SetDefaultHttpContext();
            controller.Url = GetMockUrlHelper("any");
            var result = await controller.ResetPassword(user.Id);
            Assert.Equal($"Sent password reset email for {user.UserName}.", controller.ViewBag.SuccessMessage);
        }

        [Fact]
        public async Task ResetPasswordReturnsAView()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser()
            {
                Id = "1234",
                Email = "test@testy.com",
                UserName = "auser"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Returns(user);
            string code = "passcode";
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
            mediator.Setup(x => x.Send(It.IsAny<UserByUserIdQuery>()))
                .Throws<Exception>();

            var controller = new SiteController(null, logger.Object, mediator.Object);
            var result = (ViewResult)await controller.ResetPassword("1234");

            logger.Verify(l => l.Log<object>(LogLevel.Error, 0,
                It.IsAny<Microsoft.Extensions.Logging.Internal.FormattedLogValues>(), null,
                It.IsAny<Func<object, Exception, string>>()));
        }

        [Fact]
        public async Task ResetPasswordAddsCorrectErrorMessagetoViewBagWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            mediator.Setup(x => x.Send(It.IsAny<UserByUserIdQuery>()))
                .Throws<Exception>();

            var controller = new SiteController(null, logger.Object, mediator.Object);
            string userId = "1234";
            var result = (ViewResult)await controller.ResetPassword(userId);

            Assert.Equal($"Failed to reset password for {userId}. Exception thrown.", controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public async Task ResetPasswordReturnsAViewWhenExcpetionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            mediator.Setup(x => x.Send(It.IsAny<UserByUserIdQuery>()))
                .Throws<Exception>();

            var controller = new SiteController(null, logger.Object, mediator.Object);
            string userId = "1234";
            var result = (ViewResult)await controller.ResetPassword(userId);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ResetPasswordHasHttpGetAttribute()
        {
            var controller = new SiteController(null, null,null);
            var attribute = controller.GetAttributesOn(x => x.ResetPassword(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task AssignSiteAdminSendsUserByUserIdQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            string userId = It.IsAny<string>();
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Returns(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.AssignSiteAdmin(userId);
            mediator.Verify(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task AssignApiRoleQueriesForCorrectId()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            string userId = Guid.NewGuid().ToString();
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Returns(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.AssignApiAccessRole(userId);
            mediator.Verify(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public void SearchForCorrectUserWhenManagingApiKeys()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            string userId = Guid.NewGuid().ToString();
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Returns(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            controller.ManageApiKeys(userId);
            mediator.Verify(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }


        [Fact]
        public async Task SearchForCorrectUserWhenGeneratingApiKeys()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();

            string userId = Guid.NewGuid().ToString();
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Returns(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.GenerateToken(userId);
            mediator.Verify(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task AssignSiteAdminInvokesAddClaimAsyncWithCorrrectParameters()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser()
            {
                Id = "1234"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Returns(user);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            await controller.AssignSiteAdmin(user.Id);

            userManager.Verify(x => x.AddClaimAsync(user, It.Is<Claim>(c => 
                                                        c.Value == UserType.SiteAdmin.ToName() && 
                                                        c.Type == AllReady.Security.ClaimTypes.UserType)), Times.Once);
        }

        [Fact]
        public async Task AssignSiteAdminRedirectsToCorrectAction()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser()
            {
                Id = "1234"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Returns(user);
            userManager.Setup(x => x.AddClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<Claim>()))
                .Returns(() => Task.FromResult(IdentityResult.Success));

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = (RedirectToActionResult) await controller.AssignSiteAdmin(user.Id);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task AssignSiteAdminInvokesLogErrorWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            var user = new ApplicationUser()
            {
                Id = "1234"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Throws<Exception>();

            var controller = new SiteController(null, logger.Object, mediator.Object);
            var result = await controller.AssignSiteAdmin(user.Id);

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
            var user = new ApplicationUser()
            {
                Id = "1234"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Throws<Exception>();

            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            var result = await controller.AssignSiteAdmin(user.Id);

            Assert.Equal($"Failed to assign site admin for {user.Id}. Exception thrown.", controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public async Task AssignSiteAdminReturnsAViewWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            var userManager = CreateApplicationUserMock();
            var user = new ApplicationUser()
            {
                Id = "1234"
            };

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
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
        public void AssignOrganizationAdminGetSendsUserByUserIdQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            string userId = "1234";
            var controller = new SiteController(null, null, mediator.Object);
            controller.AssignOrganizationAdmin(userId);
            mediator.Verify(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public void AssignOrganizationAdminGetRedirectsToCorrectActionWhenUserIsAnOrganizationAdmin()
        {
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser()
            {
                Id = "1234"
            };
            user.Claims.Add(new Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>()
            {
                ClaimType = AllReady.Security.ClaimTypes.UserType,
                ClaimValue = Enum.GetName(typeof(UserType), UserType.OrgAdmin)
            });

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Returns(user);

            var controller = new SiteController(null, null, mediator.Object);
            var result = (RedirectToActionResult) controller.AssignOrganizationAdmin(user.Id);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public void AssignOrganizationAdminGetRedirectsToCorrectActionWhenUserIsASiteAdmin()
        {
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser()
            {
                Id = "1234"
            };
            user.Claims.Add(new Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>()
            {
                ClaimType = AllReady.Security.ClaimTypes.UserType,
                ClaimValue = Enum.GetName(typeof(UserType), UserType.SiteAdmin)
            });

            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Returns(user);

            var controller = new SiteController(null, null, mediator.Object);
            var result = (RedirectToActionResult)controller.AssignOrganizationAdmin(user.Id);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public void AssignOrganizationAdminGetSendsAllOrganizationsQuery()
        {
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser()
            {
                Id = "1234"
            };
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Returns(user);

            var controller = new SiteController(null, null, mediator.Object);
            var result = controller.AssignOrganizationAdmin(user.Id);
            mediator.Verify(m => m.Send(It.IsAny<AllOrganizationsQuery>()), Times.Once);
        }

        [Fact]
        public void AssignOrganizationAdminGetAddsCorrectSelectListItemToOrganizationsOnViewBag()
        {
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser()
            {
                Id = "1234"
            };
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Returns(user);
            List<Organization> orgs = new List<Organization>();
            orgs.Add(new Organization() { Id = 2, Name = "Borg" });
            orgs.Add(new Organization() { Id = 1, Name = "Aorg" });
            mediator.Setup(x => x.Send(It.IsAny<AllOrganizationsQuery>())).Returns(orgs);
            var controller = new SiteController(null, null, mediator.Object);
            var result = controller.AssignOrganizationAdmin(user.Id);

            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> viewBagOrgs = 
                ((IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>)controller.ViewBag.Organizations).ToList();

            Assert.Equal(3, viewBagOrgs.Count);
            Assert.Equal("0", viewBagOrgs[0].Value); // Select One item added in controller
            Assert.Equal("1", viewBagOrgs[1].Value); // sorted items
            Assert.Equal("2", viewBagOrgs[2].Value);
        }

        [Fact]
        public void AssignOrganizationAdminGetReturnsCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            var user = new ApplicationUser()
            {
                Id = "1234"
            };
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == user.Id)))
                .Returns(user);
            List<Organization> orgs = new List<Organization>();
            orgs.Add(new Organization() { Id = 2, Name = "Borg" });
            orgs.Add(new Organization() { Id = 1, Name = "Aorg" });
            mediator.Setup(x => x.Send(It.IsAny<AllOrganizationsQuery>())).Returns(orgs);
            var controller = new SiteController(null, null, mediator.Object);
            var result = (ViewResult)controller.AssignOrganizationAdmin(user.Id);

            Assert.IsType<AssignOrganizationAdminModel>(result.Model);
        }

        [Fact]
        public void AssignOrganizationAdminGetHasHttpGetAttribute()
        {
            var controller = new SiteController(null, null,null);
            var attribute = controller.GetAttributesOn(x => x.AssignOrganizationAdmin(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostSendsUserByUserIdQueryWithCorrectUserId()
        {
            var mediator = new Mock<IMediator>();
            AssignOrganizationAdminModel model = new AssignOrganizationAdminModel()
            {
                UserId = "1234"
            };

            var controller = new SiteController(null, null, mediator.Object);
            await controller.AssignOrganizationAdmin(model);

            mediator.Verify(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId)), Times.Once);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostRedirectsToCorrectActionIsUserIsNull()
        {
            var mediator = new Mock<IMediator>();
            AssignOrganizationAdminModel model = new AssignOrganizationAdminModel()
            {
                UserId = "1234"
            };
            ApplicationUser nullUser = null;
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).Returns(nullUser);
            var controller = new SiteController(null, null, mediator.Object);
            var result = (RedirectToActionResult)await controller.AssignOrganizationAdmin(model);

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostAddsCorrectKeyAndErrorMessageToModelStateWhenOrganizationIdIsZero()
        {
            var mediator = new Mock<IMediator>();
            AssignOrganizationAdminModel model = new AssignOrganizationAdminModel()
            {
                UserId = "1234"
            };
            ApplicationUser user = new ApplicationUser() { UserName = "name" };
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).Returns(user);
            var controller = new SiteController(null, null, mediator.Object);
            var result = await controller.AssignOrganizationAdmin(model);

            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("OrganizationId"));
            Assert.True(controller.ModelState.GetErrorMessagesByKey("OrganizationId").FirstOrDefault(x => x.ErrorMessage.Equals("You must pick a valid organization.")) != null);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostSendsAllOrganizationsQueryWhenModelStateIsValid()
        {
            var mediator = new Mock<IMediator>();
            AssignOrganizationAdminModel model = new AssignOrganizationAdminModel()
            {
                UserId = "1234",
                OrganizationId = 5678
            };
            ApplicationUser user = new ApplicationUser() { UserName = "name" };
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).Returns(user);
            var controller = new SiteController(null, null, mediator.Object);
            var result = await controller.AssignOrganizationAdmin(model);

            mediator.Verify(x => x.Send(It.IsAny<AllOrganizationsQuery>()), Times.Once);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostInvokesAddClaimAsyncTwiceAddingTheCorrectClaimsWhenOrganizationIdOnModelMatchesAnyExistingOrganization()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            AssignOrganizationAdminModel model = new AssignOrganizationAdminModel()
            {
                UserId = "1234",
                OrganizationId = 5678
            };
            ApplicationUser user = new ApplicationUser() { UserName = "name" };
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).Returns(user);
            List<Organization> orgs = new List<Organization>();
            orgs.Add(new Organization() { Id = 5678, Name = "Borg" });
            mediator.Setup(x => x.Send(It.IsAny<AllOrganizationsQuery>())).Returns(orgs);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = await controller.AssignOrganizationAdmin(model);

            userManager.Verify(x => x.AddClaimAsync(user, It.Is<Claim>(c =>
                                            c.Value == UserType.OrgAdmin.ToName() &&
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
            AssignOrganizationAdminModel model = new AssignOrganizationAdminModel()
            {
                UserId = "1234",
                OrganizationId = 5678
            };
            ApplicationUser user = new ApplicationUser() { UserName = "name" };
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).Returns(user);
            List<Organization> orgs = new List<Organization>();
            orgs.Add(new Organization() { Id = 5678, Name = "Borg" });
            mediator.Setup(x => x.Send(It.IsAny<AllOrganizationsQuery>())).Returns(orgs);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = (RedirectToActionResult) await controller.AssignOrganizationAdmin(model);

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostAddsCorrectKeyAndErrorMessageToModelStateWhenOrganzationNotFound()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            AssignOrganizationAdminModel model = new AssignOrganizationAdminModel()
            {
                UserId = "1234",
                OrganizationId = 5678
            };
            ApplicationUser user = new ApplicationUser() { UserName = "name" };
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).Returns(user);
            List<Organization> orgs = new List<Organization>();
            orgs.Add(new Organization() { Id = 9123, Name = "Borg" });
            mediator.Setup(x => x.Send(It.IsAny<AllOrganizationsQuery>())).Returns(orgs);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = await controller.AssignOrganizationAdmin(model);

            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("OrganizationId"));
            Assert.True(controller.ModelState.GetErrorMessagesByKey("OrganizationId").FirstOrDefault(x => x.ErrorMessage.Equals("Invalid Organization. Please contact support.")) != null);
        }

        [Fact]
        public async Task AssignOrganizationAdminPostReturnsAView()
        {
            var mediator = new Mock<IMediator>();
            AssignOrganizationAdminModel model = new AssignOrganizationAdminModel()
            {
                UserId = "1234",
                OrganizationId = 0
            };
            ApplicationUser user = new ApplicationUser() { UserName = "name" };
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == model.UserId))).Returns(user);

            var controller = new SiteController(null, null, mediator.Object);
            var result = (IActionResult) await controller.AssignOrganizationAdmin(model);

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

            string userId = It.IsAny<string>();
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Returns(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.RevokeSiteAdmin(userId);
            mediator.Verify(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task RevokeSiteAdminInvokesRemoveClaimAsyncWithCorrectParameters()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            string userId = "1234";
            ApplicationUser user = new ApplicationUser() { UserName = "name", Id = userId };
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Returns(user);

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = await controller.RevokeSiteAdmin(userId);

            userManager.Verify(u => u.RemoveClaimAsync(user, It.Is<Claim>(c => 
                                                            c.Type == AllReady.Security.ClaimTypes.UserType 
                                                            && c.Value == UserType.SiteAdmin.ToName())), Times.Once);
        }

        [Fact]
        public async Task RevokeSiteAdminRedirectsToCorrectAction()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            mediator.Setup(m => m.Send(It.IsAny<UserByUserIdQuery>())).Returns(new ApplicationUser());

            var controller = new SiteController(userManager.Object, null, mediator.Object);
            var result = (RedirectToActionResult) await controller.RevokeSiteAdmin(It.IsAny<string>());

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task RevokeSiteAdminInvokesLogErrorWithCorrectParametersWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            string userId = "1234";
            Exception thrown = new Exception("Test");
            mediator.Setup(x => x.Send(It.IsAny<UserByUserIdQuery>()))
                .Throws(thrown);

            var controller = new SiteController(null, logger.Object, mediator.Object);
            var result = await controller.RevokeSiteAdmin (userId);

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
            string userId = "1234";
            Exception thrown = new Exception("Test");
            mediator.Setup(x => x.Send(It.IsAny<UserByUserIdQuery>()))
                .Throws(thrown);

            var controller = new SiteController(null, logger.Object, mediator.Object);
            var result = await controller.RevokeSiteAdmin(userId);
            string expectedMessage = $"Failed to revoke site admin for {userId}. Exception thrown.";

            Assert.Equal(expectedMessage, controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public async Task RevokeSiteAdminReturnsAViewWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<SiteController>>();
            string userId = "1234";
            Exception thrown = new Exception("Test");
            mediator.Setup(x => x.Send(It.IsAny<UserByUserIdQuery>()))
                .Throws(thrown);

            var controller = new SiteController(null, logger.Object, mediator.Object);
            var result = (IActionResult)await controller.RevokeSiteAdmin(userId);
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

            string userId = It.IsAny<string>();
            mediator.Setup(x => x.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Returns(new ApplicationUser());
            var controller = new SiteController(null, logger.Object, mediator.Object);

            await controller.RevokeOrganizationAdmin(userId);
            mediator.Verify(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task RevokeOrganizationAdminInvokesGetclaimsAsyncWithCorrectUser()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var logger = new Mock<ILogger<SiteController>>();
            string userId = "1234";
            ApplicationUser user = new ApplicationUser() { UserName = "name", Id = userId };
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Returns(user);

            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            var result = await controller.RevokeOrganizationAdmin(userId);

            userManager.Verify(u => u.GetClaimsAsync(user), Times.Once);
        }

        [Fact]
        public async Task RevokeOrganizationAdminInokesRemoveClaimAsyncTwiceWithCorrectParameters()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var logger = new Mock<ILogger<SiteController>>();
            string userId = "1234";
            ApplicationUser user = new ApplicationUser() { UserName = "name", Id = userId };
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Returns(user);

            string orgId = "4567";
            IList<Claim> claims = new List<Claim>();
            claims.Add(new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToName()));
            claims.Add(new Claim(AllReady.Security.ClaimTypes.Organization, orgId));
            userManager.Setup(u => u.GetClaimsAsync(user)).ReturnsAsync(claims);

            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            var result = await controller.RevokeOrganizationAdmin(userId);

            userManager.Verify(u => u.RemoveClaimAsync(user, It.Is<Claim>(c =>
                                                c.Type == AllReady.Security.ClaimTypes.UserType
                                                && c.Value == UserType.SiteAdmin.ToName())), Times.Once);
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
            string userId = "1234";
            ApplicationUser user = new ApplicationUser() { UserName = "name", Id = userId };
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Returns(user);

            string orgId = "4567";
            IList<Claim> claims = new List<Claim>();
            claims.Add(new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToName()));
            claims.Add(new Claim(AllReady.Security.ClaimTypes.Organization, orgId));
            userManager.Setup(u => u.GetClaimsAsync(user)).ReturnsAsync(claims);

            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            var result = (RedirectToActionResult) await controller.RevokeOrganizationAdmin(userId);

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task RevokeOrganizationAdminInvokesLogErrorWithCorrectParametersWhenExceptionIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var logger = new Mock<ILogger<SiteController>>();
            string userId = "1234";
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Throws<Exception>();
            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            var result = await controller.RevokeOrganizationAdmin(userId);

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
            string userId = "1234";
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Throws<Exception>();
            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            var result = await controller.RevokeOrganizationAdmin(userId);

            string expectedMessage = $"Failed to revoke organization admin for {userId}. Exception thrown.";

            Assert.Equal(controller.ViewBag.ErrorMessage, expectedMessage);
        }

        [Fact]
        public async Task RevokeOrganizationAdminReturnsAViewWhenErrorIsThrown()
        {
            var mediator = new Mock<IMediator>();
            var userManager = CreateApplicationUserMock();
            var logger = new Mock<ILogger<SiteController>>();
            string userId = "1234";
            mediator.Setup(m => m.Send(It.Is<UserByUserIdQuery>(q => q.UserId == userId))).Throws<Exception>();
            var controller = new SiteController(userManager.Object, logger.Object, mediator.Object);
            var result = (ViewResult)await controller.RevokeOrganizationAdmin(userId);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void RevokeOrganizationAdminHasHttpGetAttribute()
        {
            var controller = new SiteController(null, null,null);
            var attribute = controller.GetAttributesOn(x => x.RevokeOrganizationAdmin(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributes().OfType<AreaAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.RouteValue, "Admin");
        }

        [Fact]
        public void ControllerHasAuthorizeAtttributeWithTheCorrectPolicy()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Policy, "SiteAdmin");
        }

        private static Mock<UserManager<ApplicationUser>> CreateApplicationUserMock()
        {
            return new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null,null);
        }

        private static IUrlHelper GetMockUrlHelper(string returnValue)
        {
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(o => o.Action(It.IsAny<UrlActionContext>())).Returns(returnValue);
            return urlHelper.Object;
        }

    }
}