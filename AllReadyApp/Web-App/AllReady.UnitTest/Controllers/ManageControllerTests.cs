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

        //start at RemoveLogin
    }
}
