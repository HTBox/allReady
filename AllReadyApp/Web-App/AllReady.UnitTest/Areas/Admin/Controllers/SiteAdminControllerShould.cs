using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Users;
using MediatR;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class SiteAdminControllerShould
    {
        //delete this line when all unit tests using it have been completed
        private static readonly Task<int> TaskFromResultZero = Task.FromResult(0);

        [Fact(Skip = "NotImplemented")]
        public void IndexReturnsCorrectViewModel()
        {
        }

        [Fact]
        public void DeleteUserSendsUserQueryWithCorrectUserId()
        {
        }

        [Fact]
        public void DeleteUserReturnsTheCorrectViewModel()
        {
        }

        [Fact]
        public void DeleteUserHasHttpGetAttribute()
        {
        }

        [Fact]
        public async Task ConfirmDeletUserSendsDeleteUserCommandAsync()
        {
            var mediator = new Mock<IMediator>();
            var controller = new SiteController(null, null, null, null, mediator.Object);
            const string userId = "foo_id";

            await controller.ConfirmDeleteUser(userId);
            mediator.Verify(b => b.SendAsync(It.Is<DeleteUserCommand>(u => u.UserId == userId)));
        }

        [Fact]
        public async Task ConfirmDeletUserRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void ConfirmDeletUserHasHttpPostAttribute()
        {
        }

        [Fact]
        public void ConfirmDeletUserHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact]
        public void EditUserGetInvokesGetUserWithCorrectUserId()
        {
        }

        [Fact]
        public void EditUserGetReturnsCorrectViewModelWhenOrganizationIdIsNull()
        {
        }

        [Fact]
        public void EditUserGetReturnsCorrectViewModelWhenOrganizationIdIsNotNull()
        {
        }

        [Fact]
        public void EditUserPostReturnsSameViewAndViewModelWhenModelStateIsInvalid()
        {
        }

        [Fact]
        public async Task EditUserPostInvokesGetUserWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task EditUserPostInvokesUpdateUserWithCorrectUserWhenUsersAssociatedSkillsAreNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task EditUserPostInvokesUpdateUserWithCorrectUserWhenUsersAssociatedSkillsAreNotNullAndThereIsAtLeastOneAssociatedSkillForTheUser()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task EditUserPostInvokesAddClaimAsyncWhenModelsIsOrganizationAdminIsTrue()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task EditUserPostInvokesUrlActionWithCorrectParametersWhenModelsIsOrganizationAdminIsTrueAndOrganizationAdminClaimWasAddedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task EditUserPostInvokesSendEmailAsyncWithCorrectParametersWhenModelsIsOrganizationAdminIsTrueAndOrganizationAdminClaimWasAddedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task EditUserPostReturnsRedirectResultWithCorrectUrlWhenModelsIsOrganizationAdminIsTrueAndOrganizationAdminClaimWasNotAddedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task EditUserPostInvokesRemoveClaimAsyncWithCorrectParametersWhenUserIsAnOrganizationAdmin()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task EditUserPostReturnsRedirectResultWithCorrectUrlWhenUserIsAnOrganizationAdminAndRemovClaimAsyncDoesNotSucceed()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task EditUserPostRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void EditUserPostHasHttpPostAttribute()
        {
        }

        [Fact]
        public void EditUserPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact]
        public async Task ResetPasswordInvokesGetUserWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task ResetPasswordAddsCorrectErrorMessageToViewBagWhenUserIsNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task ResetPasswordInvokesGeneratePasswordResetTokenAsyncWithCorrectUserWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task ResetPasswordInvokesUrlActionWithCorrectParametersWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task ResetPasswordInvokesSendEmailAsyncWithCorrectParametersWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task ResetPasswordAddsCorrectSuccessMessagetoViewBagWhenUserIsNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task ResetPasswordReturnsAView()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task ResetPasswordInvokesLogErrorWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task ResetPasswordAddsCorrectErrorMessagetoViewBagWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task ResetPasswordReturnsAViewWhenExcpetionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void ResetPasswordHasHttpGetAttribute()
        {
        }

        [Fact]
        public async Task AssignSiteAdminInvokesGetUserWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignSiteAdminInvokesAddClaimAsyncWithCorrrectParameters()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignSiteAdminRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignSiteAdminInvokesLogErrorWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignSiteAdminAddsCorrectErrorMessageToViewBagWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignSiteAdminReturnsAViewWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void AssignSiteAdminHasHttpGetAttribute()
        {
        }

        [Fact]
        public async Task AssignOrganizationAdminGetInvokesGetUserWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignOrganizationAdminGetRedirectsToCorrectActionWhenUserIsAnOrganizationAdmin()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignOrganizationAdminGetRedirectsToCorrectActionWhenUserIsASiteAdmin()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignOrganizationAdminGetAddsCorrectSelectListItemToOrganizationsOnViewBag()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignOrganizationAdminGetReturnsCorrectViewModel()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void AssignOrganizationAdminGetHasHttpGetAttribute()
        {
        }

        [Fact]
        public async Task AssignOrganizationAdminPostInvokesGetUserWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignOrganizationAdminPostRedirectsToCorrectActionIsUserIsNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignOrganizationAdminPostAddsCorrectKeyAndErrorMessageToModelStateWhenOrganizationIdIsZero()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignOrganizationAdminPostInvokesOrganizationsWhenModelStateIsValid()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignOrganizationAdminPostInvokesAddClaimAsyncTwiceAddingTheCorrectClaimsWhenOrganizationIdOnModelMatchesAnyExistingOrganization()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignOrganizationAdminPostRedirectsToCorrectActionWhenOrganizationIdOnModelMatchesAnyExistingOrganization()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignOrganizationAdminPostAddsCorrectKeyAndErrorMessageToModelStateWhenOrganzationNotFound()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task AssignOrganizationAdminPostReturnsAView()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void AssignOrganizationAdminPostHasHttpPostAttribute()
        {
        }

        [Fact]
        public void AssignOrganizationAdminPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact]
        public async Task RevokeSiteAdminInvokesGetUserWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task RevokeSiteAdminInvokesRemoveClaimAsyncWithCorrectParameters()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task RevokeSiteAdminRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task RevokeSiteAdminInvokesLogErrorWithCorrectParametersWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task RevokeSiteAdminAddsCorrectErrorMessageToViewBagWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task RevokeSiteAdminReturnsAViewWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void RevokeSiteAdminHasHttpGetAttribute()
        {
        }

        [Fact]
        public async Task RevokeOrganizationAdminInvokesGetUserWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task RevokeOrganizationAdminInvokesGetclaimsAsyncWithCorrectUser()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task RevokeOrganizationAdminInokesRemoveClaimAsyncTwiceWithCorrectParameters()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task RevokeOrganizationAdminRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task RevokeOrganizationAdminInvokesLogErrorWithCorrectParametersWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task RevokeOrganizationAdminAddsCorrectErrorMessageToViewBagWhenExceptionIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task RevokeOrganizationAdminReturnsAViewWhenErrorIsThrown()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void RevokeOrganizationAdminHasHttpGetAttribute()
        {
        }

        [Fact]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
        }

        [Fact]
        public void ControllerHasAuthorizeAtttributeWithTheCorrectPolicy()
        {
        }
    }
}