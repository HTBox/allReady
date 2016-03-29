using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class ManageControllerTests
    {
        [Fact]
        public void IndexGetAddsCorrectMessageToViewDataWhenMessageEqualsChangePasswordSuccess()
        {
        }

        [Fact]
        public void IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsSetPasswordSuccess()
        {
        }

        [Fact]
        public void IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsSetTwoFactorSuccess()
        {
        }

        [Fact]
        public void IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsError()
        {
        }

        [Fact]
        public void IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsAddPhoneSuccess()
        {
        }

        [Fact]
        public void IndexGetAddsCorrectMessageToViewDataWhenMessageIdEqualsRemovePhoneSuccess()
        {
        }

        [Fact]
        public void IndexGetInvokesGetUserWithCorrectUserId()
        {
        }

        [Fact]
        public void IndexGetReturnsCorrectView()
        {
        }

        [Fact]
        public void IndexGetHasHttpGetAttribute()
        {
        }

        [Fact]
        public void IndexGetReturnsCorrectViewModel()
        {
        }

        [Fact]
        public void IndexPostInvokesGetUserWithCorrectUserId()
        {
        }

        [Fact]
        public void IndexPostReturnsCorrectViewWhenModelStateIsInvalid()
        {
        }

        [Fact]
        public void IndexPostReturnsCorrectViewModelWhenModelStateIsInvalid()
        {
        }

        [Fact]
        public void IndexPostInvokesRemoveClaimsAsyncWithCorrectParametersWhenUsersTimeZoneDoesNotEqualModelsTimeZone()
        {
        }

        [Fact]
        public void IndexPostInvokesAddClaimAsyncWithCorrectParametersWhenUsersTimeZoneDoesNotEqualModelsTimeZone()
        {
        }

        //TODO: come back to finsih these stubs... there is a lot going on in Index Post

        [Fact]
        public void IndexPostHasHttpPostAttrbiute()
        {
        }

        [Fact]
        public void IndexPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact]
        public void UpdateUserProfileCompletenessSendsRemoveUserProfileIncompleteClaimCommandWithCorrectUserIdWhenUsersProfileIsComplete()
        {
        }

        [Fact]
        public void UpdateUserProfileCompletenessInvokesRefreshSignInAsyncWithCorrectUserWhenUsersProfileIsComplete()
        {
        }

        [Fact]
        public void ResendEmailConfirmationInvokesFindByIdAsyncWithCorrectUserId()
        {
        }

        [Fact]
        public void ResendEmailConfirmationInvokesGenerateEmailConfirmationTokenAsyncWithCorrectUser()
        {
        }

        [Fact]
        public void ResendEmailConfirmationInvokesUrlActionWithCorrectParameters()
        {
        }

        [Fact]
        public void ResendEmailConfirmationInvokesSendEmailAsyncWithCorrectParameters()
        {
        }

        [Fact]
        public void ResendEmailConfirmationRedirectsToCorrectAction()
        {
        }

        [Fact]
        public void ResendEmailConfirmationHasHttpPostAttribute()
        {
        }

        [Fact]
        public void ResendEmailConfirmationHasHttpValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact]
        public void EmailConfirmationSentReturnsAView()
        {
        }

        [Fact]
        public void RemoveLoginInvokesGetUserWithCorrectUserId()
        {
        }

        [Fact]
        public void RemoveLoginInvokesRemoveLoginAsyncWithCorrectParametersWhenUserIsNotNull()
        {
        }

        [Fact]
        public void RemoveLoginInvokesSignInAsyncWithCorrectParametersWhenUserIsNotNullAndRemoveLoginSucceeds()
        {
        }

        [Fact]
        public void RemoveLoginRedirectsToCorrectActionWithCorrectRouteValues()
        {
        }

        [Fact]
        public void RemoveLoginHasHttpPostAttribute()
        {
        }

        [Fact]
        public void RemoveLoginHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact]
        public void AddPhoneNumberGetReturnsAView()
        {
        }

        [Fact]
        public void AddPhoneNumberPostReturnsTheSameViewAndModelWhenModelStateIsInvalid()
        {
        }

        [Fact]
        public void AddPhoneNumberPostInvokesGetUserWithCorrectUserIdWhenModelStateIsValid()
        {
        }

        [Fact]
        public void AddPhoneNumberPostInvokesGenerateChangePhoneNumberTokenAsyncWithCorrectParametersWhenModelStateIsValid()
        {
        }

        [Fact]
        public void AddPhoneNumberPostInvokesSendSmsAsyncWithCorrectParametersWhenModelStateIsValid()
        {
        }

        [Fact]
        public void AddPhonNumberPostRedirectsToCorrectActionWithCorrectRouteValuesWhenModelStateIsValid()
        {
        }

        [Fact]
        public void AddPhoneNumberHasHttpPostAttribute()
        {
        }

        [Fact]
        public void AddPhoneNumberHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact]
        public void ResendPhoneNumberConfirmationInvokesGetUserWithCorrectUserId()
        {
        }

        [Fact]
        public void ResendPhoneNumberConfirmationInvokesGenerateChangePhoneNumberTokenAsyncWithCorrectParameters()
        {
        }

        [Fact]
        public void ResendPhoneNumberInvokesSendSmsAsyncWithCorrectParameters()
        {
        }

        [Fact]
        public void ResendPhoneNumberRedirectsToCorrectActionWithCorrectRouteValues()
        {
        }

        [Fact]
        public void ResendPhoneNumberHasHttpPostAttribute()
        {
        }

        [Fact]
        public void ResendPhoneNumberHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact]
        public void EnableTwoFactorAuthenticationInvokesGetUserWithCorrectUserId()
        {
        }

        [Fact]
        public void EnableTwoFactorAuthenticationInvokesSetTwoFactorEnabledAsyncWhenUserIsNotNull()
        {
        }

        [Fact]
        public void EnableTwoFactorAuthenticationInvokesSignInAsyncWhenUserIsNotNull()
        {
        }

        [Fact]
        public void EnableTwoFactorAuthenticationRedirectsToCorrectAction()
        {
        }

        [Fact]
        public void EnbaleTwoFactorAuthenticationHasHttpPostAttribute()
        {
        }

        [Fact]
        public void EnableTwoFactorAuthenticationHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact]
        public void DisableTwoFactorAuthenticationInvokesGetUserWithCorrectUserId()
        {
        }

        [Fact]
        public void DisableTwoFactorAuthenticationInvokesSetTwoFactorEnabledAsyncWithCorrectParametersWhenUserIsNotNull()
        {
        }

        [Fact]
        public void DisableTwoFactorAuthenticationInvokesSignInAsyncWhenUserIsNotNull()
        {
        }

        [Fact]
        public void DisableTwoFactorAuthenticationRedirectsToCorrectAction()
        {
        }

        [Fact]
        public void DisableTwoFactorAuthenticationHasHttpPostAttribute()
        {
        }

        [Fact]
        public void DisableTwoFactorAuthenticationHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact]
        public void VerifyPhoneNumberGetReturnsErrorViewWhenPhoneNumberIsNull()
        {
        }

        [Fact]
        public void VerifyPhoneNumberGetReturnsReturnsTheCorrectViewandViewModelWhenPhoneNumberIsNotNull()
        {
        }

        [Fact]
        public void VerifyPhoneNumberGetHasHttpGetAttribute()
        {
        }

        [Fact]
        public void VerifyPhoneNumberPostReturnsTheSameViewAndModelWhenModelStateIsInvalid()
        {
        }

        [Fact]
        public void VerifyPhoneNumberPostInvokesGetUserWithCorrectUserId()
        {
        }

        [Fact]
        public void VerifyPhoneNumberPostInvokesChangePhoneNumberAsyncWithCorrectParametersWhenUserIsNotNull()
        {
        }

        [Fact]
        public void VerifyPhoneNumberPostSendsRemoveUserProfileIncompleteClaimCommandWhenUserIsNotNullAndPhoneNumberChangeWasSuccessfullAndUserProfileIsComplete()
        {
        }

        [Fact]
        public void VerifyPhoneNumberPostInvokesRefreshSignInAsyncWithCorrectParametersWhenUserIsNotNullAndPhoneNumberChangeWasSuccessfullAndUserProfileIsComplete()
        {
        }

        [Fact]
        public void VerifyPhoneNumberPostInvokesSignInAsyncWithCorrectPaarmetersWhenUserIsNotNullAndPhoneNumberChangeWasSuccessfull()
        {
        }

        [Fact]
        public void VerifyPhoneNumberPostRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNotNullAndPhoneNumberChangeWasSuccessfull()
        {
        }

        [Fact]
        public void VerifyPhoneNumberPostAddsCorrectErrorMessageToModelStateWhenUserIsNull()
        {
        }

        [Fact]
        public void VerifyPhoneNumberPostReturnsCorrectViewModelWhenUserIsNull()
        {
        }

        [Fact]
        public void VerifyPhoneNumberPostHasHttpPostAttribute()
        {
        }

        [Fact]
        public void VerifyPhoneNumberPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact]
        public void RemovePhoneNumberInvokesGetUserWithCorrectUserId()
        {
        }

        [Fact]
        public void RemovePhoneNumberInvokesSetPhoneNumberAsyncWithCorrectParametersWhenUserIsNotNull()
        {
        }

        [Fact]
        public void RemovePhoneNumberInvokesSignInAsyncWithCorrectParametersWhenUserIsNotNullAndPhoneNumberWasSetSuccessfully()
        {
        }

        [Fact]
        public void RemovePhoneNumberSendsRemoveUserProfileIncompleteClaimCommandWithCorrectDataWhenUserIsNotNullAndPhoneNumberWasSetSuccessfullyAndUsersProfileIsComplete()
        {
        }

        [Fact]
        public void RemovePhoneNumberInvokesRefreshSignInAsyncWithCorrectParametersWhenUserIsNotNullAndPhoneNumberWasSetSuccessfullyAndUsersProfileIsComplete()
        {
        }

        [Fact]
        public void RemovePhoneNumberRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNotNullAndPhoneNumberWasSetSuccessfully()
        {
        }

        [Fact]
        public void RemovePhoneNumberRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNull()
        {
        }

        [Fact]
        public void RemovePhoneNumberHasHttpGetAttribute()
        {
        }

        [Fact]
        public void ChangePasswordGetReturnsAView()
        {
        }

        [Fact]
        public void ChangePasswordGetHasHttpGetAttribute()
        {
        }

        [Fact]
        public void ChangePasswordPostReturnsSameViewAndModelWhenModelStateIsInvalid()
        {
        }

        [Fact]
        public void ChangePasswordPostInvokesGetUserWithCorrectUserId()
        {
        }

        [Fact]
        public void ChangePasswordPostInvokesChangePasswordAsyncWithCorrectParametersWhenUserIsNotNull()
        {
        }

        [Fact]
        public void ChangePasswordPostInvokesSignInAsyncWithCorrectParametersWhenUserIsNotNullAndPasswordWasChangedSuccessfully()
        {
        }

        [Fact]
        public void ChangePasswordPostRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNotNullAndPasswordWasChangedSuccessfully()
        {
        }

        [Fact]
        public void ChangePasswordPostAddsIdentityResultErrorsToModelStateErrorsWhenUserIsNotNullAndPasswordWasNotChangedSuccessfully()
        {
        }

        [Fact]
        public void ChangePasswordPostReturnsCorrectViewModelWhenUserIsNotNullAndPasswordWasNotChangedSuccessfully()
        {
        }

        [Fact]
        public void ChangePasswordPostRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNull()
        {
        }

        [Fact]
        public void ChangePasswordHPostasHttpPostAttribute()
        {
        }

        [Fact]
        public void ChangePasswordPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact]
        public void ChangeEmailGetReturnsAView()
        {
        }

        [Fact]
        public void ChangeEmailGetHasHttpGetAttribute()
        {
        }

        [Fact]
        public void ChangeEmailPostReturnsSameViewAndViewModelWhenModelStateIsInvalid()
        {
        }

        [Fact]
        public void ChangeEmailPostInvokesGetUserWithCorrectUserId()
        {
        }

        [Fact]
        public void ChangeEmailPostInvokesCheckPasswordAsyncWithCorrectParametersWhenUserIsNotNull()
        {
        }

        [Fact]
        public void ChangeEmailPostAddsCorrectErrorMessageToModelStateWhenChangePasswordIsUnsuccessfull()
        {
        }

        [Fact]
        public void ChangeEmailPostReturnsCorrectVieModelWhenChangePasswordIsUnsuccessfull()
        {
        }

        [Fact]
        public void ChangeEmailPostInvokesFindByEmailAsyncWithCorrectParametersWhenChangePasswordIsSuccessfull()
        {
        }

        [Fact]
        public void ChangeEmailPostAddsCorrectErrorToModelStateWhenChangePasswordIsSuccessfullAndEmailCannotBeFound()
        {
        }

        [Fact]
        public void ChangeEmailPostReturnsCorrectViewModelWhenChangePasswordIsSuccessfullAndEmailCannotBeFound()
        {
        }

        [Fact]
        public void ChangeEmailPostInvokesUpdateAsyncWithCorrectParametersWhenUserIsNotNullAndChangePasswordIsSuccessfullAndUsersEmailIsFound()
        {
        }

        [Fact]
        public void ChangeEmailPostInvokesGenerateChangeEmailTokenAsyncWithCorrectParametersWhenUserIsNotNullAndChangePasswordIsSuccessfullAndUsersEmailIsFound()
        {
        }

        [Fact]
        public void ChangeEmailPostInvokesUrlActioncWithCorrectParametersWhenUserIsNotNullAndChangePasswordIsSuccessfullAndUsersEmailIsFound()
        {
        }

        [Fact]
        public void ChangeEmailPostInvokesSendEmailAsyncWithCorrectParametersWhenUserIsNotNullAndChangePasswordIsSuccessfullAndUsersEmailIsFound()
        {
        }

        [Fact]
        public void ChangeEmailPostRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsNotNullAndChangePasswordIsSuccessfullAndUsersEmailIsFound()
        {
        }

        [Fact]
        public void ChangeEmailPostRedirectsToTheCorrectActionWithTheCorrectRouteValuesWhenUserIsNull()
        {
        }
    }
}
