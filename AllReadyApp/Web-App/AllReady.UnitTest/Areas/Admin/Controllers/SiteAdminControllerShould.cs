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

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteUserSendsUserQueryWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteUserReturnsTheCorrectViewModel()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteUserHasHttpGetAttribute()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public async Task ConfirmDeletUserSendsDeleteUserCommandAsync()
        {
            var mediator = new Mock<IMediator>();
            var controller = new SiteController(null, null, null, mediator.Object);
            const string userId = "foo_id";

            await controller.ConfirmDeleteUser(userId);
            mediator.Verify(b => b.SendAsync(It.Is<DeleteUserCommand>(u => u.UserId == userId)));
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ConfirmDeletUserRedirectsToCorrectAction()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void ConfirmDeletUserHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ConfirmDeletUserHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditUserGetInvokesGetUserWithCorrectUserId()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditUserGetReturnsCorrectViewModelWhenOrganizationIdIsNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditUserGetReturnsCorrectViewModelWhenOrganizationIdIsNotNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditUserPostReturnsSameViewAndViewModelWhenModelStateIsInvalid()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditUserPostInvokesGetUserWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditUserPostInvokesUpdateUserWithCorrectUserWhenUsersAssociatedSkillsAreNotNull()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditUserPostInvokesUpdateUserWithCorrectUserWhenUsersAssociatedSkillsAreNotNullAndThereIsAtLeastOneAssociatedSkillForTheUser()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditUserPostInvokesAddClaimAsyncWhenModelsIsOrganizationAdminIsTrue()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditUserPostInvokesUrlActionWithCorrectParametersWhenModelsIsOrganizationAdminIsTrueAndOrganizationAdminClaimWasAddedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditUserPostSendsSendAccountApprovalEmailWithCorrectDataWhenModelsIsOrganizationAdminIsTrueAndOrganizationAdminClaimWasAddedSuccessfully()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
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

        [Fact(Skip = "NotImplemented")]
        public void EditUserPostHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditUserPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task ResetPasswordInvokesGetUserWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
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

        [Fact(Skip = "NotImplemented")]
        public void ResetPasswordHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignSiteAdminInvokesGetUserWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
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

        [Fact(Skip = "NotImplemented")]
        public void AssignSiteAdminHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminGetInvokesGetUserWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminGetRedirectsToCorrectActionWhenUserIsAnOrganizationAdmin()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminGetRedirectsToCorrectActionWhenUserIsASiteAdmin()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminGetAddsCorrectSelectListItemToOrganizationsOnViewBag()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminGetReturnsCorrectViewModel()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignOrganizationAdminGetHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task AssignOrganizationAdminPostInvokesGetUserWithCorrectUserId()
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
        public async Task AssignOrganizationAdminPostInvokesOrganizationsWhenModelStateIsValid()
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

        [Fact(Skip = "NotImplemented")]
        public void AssignOrganizationAdminPostHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignOrganizationAdminPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RevokeSiteAdminInvokesGetUserWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
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

        [Fact(Skip = "NotImplemented")]
        public void RevokeSiteAdminHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task RevokeOrganizationAdminInvokesGetUserWithCorrectUserId()
        {
            //delete this line when starting work on this unit test
            await TaskFromResultZero;
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

        [Fact(Skip = "NotImplemented")]
        public void RevokeOrganizationAdminHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void ControllerHasAuthorizeAtttributeWithTheCorrectPolicy()
        {
        }
    }
}