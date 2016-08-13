using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Features.Site;
using AllReady.Areas.Admin.Features.Users;
using AllReady.Features.Manage;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using MediatR;
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
        //delete this line when all unit tests using it have been completed
        private static readonly Task<int> TaskFromResultZero = Task.FromResult(0);

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

        [Fact(Skip = "NotImplemented")]
        public void EditUserPostReturnsSameViewAndViewModelWhenModelStateIsInvalid()
        {

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

        [Fact(Skip = "NotImplemented")]
        public async Task EditUserPostInvokesUrlActionWithCorrectParametersWhenModelsIsOrganizationAdminIsTrueAndOrganizationAdminClaimWasAddedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
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

        [Fact(Skip = "NotImplemented")]
        public async Task EditUserPostReturnsRedirectResultWithCorrectUrlWhenModelsIsOrganizationAdminIsTrueAndOrganizationAdminClaimWasNotAddedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditUserPostInvokesRemoveClaimAsyncWithCorrectParametersWhenUserIsAnOrganizationAdmin()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditUserPostReturnsRedirectResultWithCorrectUrlWhenUserIsAnOrganizationAdminAndRemovClaimAsyncDoesNotSucceed()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditUserPostRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
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

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordAddsCorrectErrorMessageToViewBagWhenUserIsNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordInvokesGeneratePasswordResetTokenAsyncWithCorrectUserWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordInvokesUrlActionWithCorrectParametersWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordSendsSendResetPasswordEmailWithCorrectDataWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordAddsCorrectSuccessMessagetoViewBagWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordReturnsAView()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordInvokesLogErrorWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordAddsCorrectErrorMessagetoViewBagWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordReturnsAViewWhenExcpetionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
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

        [Fact(Skip = "NotImplemented")]
        public async Task AssignSiteAdminInvokesAddClaimAsyncWithCorrrectParameters()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignSiteAdminRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignSiteAdminInvokesLogErrorWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignSiteAdminAddsCorrectErrorMessageToViewBagWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignSiteAdminReturnsAViewWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void AssignSiteAdminHasHttpGetAttribute()
        {
            var controller = new SiteController(null, null, null);
            var attribute = controller.GetAttributesOn(x => x.AssignSiteAdmin(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignOrganizationAdminGetSendsUserByUserIdQueryWithCorrectUserId()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignOrganizationAdminGetRedirectsToCorrectActionWhenUserIsAnOrganizationAdmin()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignOrganizationAdminGetRedirectsToCorrectActionWhenUserIsASiteAdmin()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignOrganizationAdminGetSendsAllOrganizationsQuery()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignOrganizationAdminGetAddsCorrectSelectListItemToOrganizationsOnViewBag()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignOrganizationAdminGetReturnsCorrectViewModel()
        {
        }

        [Fact]
        public void AssignOrganizationAdminGetHasHttpGetAttribute()
        {
            var controller = new SiteController(null, null,null);
            var attribute = controller.GetAttributesOn(x => x.AssignOrganizationAdmin(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminPostSendsUserByUserIdQueryWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminPostRedirectsToCorrectActionIsUserIsNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminPostAddsCorrectKeyAndErrorMessageToModelStateWhenOrganizationIdIsZero()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminPostSendsAllOrganizationsQueryWhenModelStateIsValid()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminPostInvokesAddClaimAsyncTwiceAddingTheCorrectClaimsWhenOrganizationIdOnModelMatchesAnyExistingOrganization()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminPostRedirectsToCorrectActionWhenOrganizationIdOnModelMatchesAnyExistingOrganization()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminPostAddsCorrectKeyAndErrorMessageToModelStateWhenOrganzationNotFound()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminPostReturnsAView()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
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

        [Fact(Skip = "NotImplemented")]
        public async Task RevokeSiteAdminInvokesRemoveClaimAsyncWithCorrectParameters()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RevokeSiteAdminRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RevokeSiteAdminInvokesLogErrorWithCorrectParametersWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RevokeSiteAdminAddsCorrectErrorMessageToViewBagWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RevokeSiteAdminReturnsAViewWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
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

        [Fact(Skip = "NotImplemented")]
        public async Task RevokeOrganizationAdminInvokesGetclaimsAsyncWithCorrectUser()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RevokeOrganizationAdminInokesRemoveClaimAsyncTwiceWithCorrectParameters()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RevokeOrganizationAdminRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RevokeOrganizationAdminInvokesLogErrorWithCorrectParametersWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RevokeOrganizationAdminAddsCorrectErrorMessageToViewBagWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RevokeOrganizationAdminReturnsAViewWhenErrorIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void RevokeOrganizationAdminHasHttpGetAttribute()
        {
            var controller = new SiteController(null, null,null);
            var attribute = controller.GetAttributesOn(x => x.RevokeOrganizationAdmin(It.IsAny<string>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact(Skip = "NotImplemented")]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ControllerHasAuthorizeAtttributeWithTheCorrectPolicy()
        {
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