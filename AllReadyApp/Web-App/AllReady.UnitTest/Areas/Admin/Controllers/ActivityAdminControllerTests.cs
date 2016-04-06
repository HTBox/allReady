using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Models;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Services;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;
using System.Linq;
using Microsoft.AspNet.Authorization;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class ActivityAdminControllerTests
    {
        //delete this line when all unit tests using it have been completed
        private static readonly Task<int> TaskFromResultZero = Task.FromResult(0);

        [Fact(Skip = "NotImplemented")]
        public async Task DetailsSendsActivityDetailQueryAsyncWithCorrectActivityId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DetailsReturnsHttpNotFoundResultWhenActivityIsNull()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DetailsReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DetailsReturnsHttpUnauthorizedResultWhenActivityIsNotNullAndUserIsNotAnOrgAdmin()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DetailsReturnsCorrectViewModelWhenActivityIsNotNullAndUserIsOrgAdmin()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact]
        public void DetailsHasHttpGetAttribute()
        {
            var sut = new ActivityController(null, null);
            var attribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DetailsHasRouteAttributeWithCorrectRoute()
        {
            var sut = new ActivityController(null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Admin/Activity/Details/{id}");
        }

        [Fact(Skip = "NotImplemented")]
        public async Task CreateGetSendsCampaignSummaryQueryAsyncWithCorrectCampaignId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task CreateGetReturnsHttpUnauthorizedResultWhenCampaignIsNull()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task CreateGetReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task CreateGetReturnsCorrectViewAndViewModel()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void CreateGetHasRouteAttributeWithCorrectRoute()
        {
        }

        [Fact]
        public async Task CreatePostReturnsEditView_When_ModelStateNotValid()
        {
            var sut = GetActivityController();
            sut.ModelState.AddModelError("test", "test");
            var activityModel = new ActivityDetailModel();
            var result = (ViewResult)await sut.Create(1, activityModel, null);

            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public async Task CreatePostReturnsEditView_When_ActivityEndDateBeforeStartDate()
        {
            var campaignStartDate = new DateTimeOffset(new DateTime(1900, 1, 1));
            var campaignEndDate = campaignStartDate.AddDays(4);
            var sut = GetActivityController(campaignStartDate, campaignEndDate);
            var activityModel = new ActivityDetailModel
            {
                EndDateTime = campaignStartDate.AddDays(1),
                StartDateTime = campaignStartDate.AddDays(2)
            };

            var result = (ViewResult)await sut.Create(1, activityModel, null);

            Assert.Equal("Edit", result.ViewName);
            var errors = sut.ModelState.GetErrorMessages();
            Assert.Equal(1, errors.Count);
            Assert.Equal("End date cannot be earlier than the start date", errors[0]);
        }

        [Fact]
        public async Task CreatePostReturnsEditView_When_ActivityStartDateBeforeCampaignStartDate()
        {
            var campaignStartDate = new DateTimeOffset(new DateTime(1900, 1, 1));
            var campaignEndDate = campaignStartDate.AddDays(4);
            var sut = GetActivityController(campaignStartDate, campaignEndDate);
            var activityModel = new ActivityDetailModel
            {
                EndDateTime = campaignStartDate,
                StartDateTime = campaignStartDate.AddDays(-1)
            };

            var result = (ViewResult)await sut.Create(1, activityModel, null);

            Assert.Equal("Edit", result.ViewName);
            var errors = sut.ModelState.GetErrorMessages();
            Assert.Equal(1, errors.Count);
            Assert.Equal("Start date cannot be earlier than the campaign start date " + campaignStartDate.ToString("d"), errors[0]);
        }
        [Fact]
        public async Task CreatePostReturnsEditView_When_ActivityEndDateAfterCampaignEndDate()
        {
            var campaignStartDate = new DateTimeOffset(new DateTime(1900, 1, 1));
            var campaignEndDate = campaignStartDate.AddDays(4);
            var sut = GetActivityController(campaignStartDate, campaignEndDate);
            var activityModel = new ActivityDetailModel
            {
                EndDateTime = campaignEndDate.AddDays(1),
                StartDateTime = campaignStartDate
            };

            var result = (ViewResult)await sut.Create(1, activityModel, null);

            Assert.Equal("Edit", result.ViewName);
            var errors = sut.ModelState.GetErrorMessages();
            Assert.Equal(1, errors.Count);
            Assert.Equal("End date cannot be later than the campaign end date " + campaignEndDate.ToString("d"), errors[0]);
        }

        /// <summary>
        /// The following unit test fails because of a bug (#3586) in MVC 6 that breaks TryValidateModel().
        /// The bug is fixed in RC2. TODO: uncomment the below test once MVC reference is updated to RC2.
        /// </summary>
        /// <returns></returns>
        //[Fact]
        //public async Task CreatePostReturnsEditView_When_ModelStateNotValid_And_ImageIsNotNull()
        //{
        //    var sut = GetActivityController();
        //    var activityModel = new ActivityDetailModel();
        //    IFormFile file = new FormFile(null, 0, 0);
        //    var result = (ViewResult)await sut.Create(1, activityModel, file);

        //    Assert.Equal("Edit", result.ViewName);
        //}

        [Fact(Skip = "NotImplemented")]
        public void CreatePostHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void CreatePostHasValidateAntiForgeryTokenAttrbiute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void CreatePostHasRouteAttrbiuteWithCorrectRoute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditGetSendsActivityDetailQueryAsyncWithCorrectActivityId()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditGetReturnsHttpNotFoundResultWhenActivityIsNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditGetReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditGetReturnsCorrectViewModel()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsBadRequestResultWhenActivityIsNull()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostSendsManagingOrganizationIdByActivityIdQueryWithCorrectActivityId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdminUser()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostSendsCampaignSummaryQueryAsyncWithTheCorrectCampaignId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostAddsValidationErrorsToModelStateErrorsWhenValidationFails()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostInvokesUploadActivityImageAsyncWithTheCorrectParametersWhenModelStateIsValidAndFileUploadIsNotNullAndFileUploadIsAnAcceptableImageContentType()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostAddsCorrectKeyAndValueToModelStateErrorsWhenModelStateIsValidAndFileUploadIsNotNullAndFileUploadIsNotAnAcceptableImageContentType()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsCorrectViewModelWhenModelStateIsValidAndFileUploadIsNotNullAndFileUploadIsNotAnAcceptableImageContentType()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostSendsEditActivityCommandAsyncWithCorrectActivityWhenModelStateIsValid()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostRedirectsToCorrectActionAndControllerWithCorrectRouteValuesWhenModelStateIsValid()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task EditPostReturnsCorrectViewModelWhenModelStateIsNotValid()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void EditPostHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditPostHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteSendsActivityDetailQueryAsyncWithCorrectActivityId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteReturnsHttpNotFoundResultWhenActivityIsNull()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteReturnsCorrectViewModel()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteHasActionNameAttributeWithCorrectName()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteConfirmedSendsActivityDetailQueryAsyncWithCorrectActivityId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteConfirmedReturnsHttpNotFoundResultWhenActivityIsNull()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteConfirmedReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteConfirmedSendsDeleteActivityCommandAsyncWithCorrectActivityId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task DeleteConfirmedRedirectToCorrectActionAndControllerWithCorrectRouteValues()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteConfirmedHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteConfirmedHasActionNameAttributeWithCorrectName()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void DeleteConfirmedHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignSendsActivityByActivityIdQueryWithCorrectActivityId()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignReturnsHttpNotFoundResultWhenActivityIsNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignReturnsCorrectViewModel()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void AssignHasHttpGetAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersReturnsBadRequestObjectResultWhenModelStateIsInvalid()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersSendsActivityDetailQueryAsyncWithCorrectActivityId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersReturnsHttpNotFoundResultWhenActivityIsNull()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersReturnsHttpUnauthorizedResultWhenUserIsNotOrgAdmin()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersSendsMessageActivityVolunteersCommandAsyncWithCorrectData()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task MessageAllVolunteersReturnsHttpOkResult()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void MessageAllVolunteersHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void MessageAllVolunteersHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public async Task PostActivityFileSendsActivityByActivityIdQueryWithCorrectActivityId()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task PostActivityFileSendsUpdateActivityAsyncWithCorrectData()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public async Task PostActivityFileRedirectsToCorrectRoute()
        {
            // delete this line when starting work on this unit test
            await TaskFromResultZero;
        }

        [Fact(Skip = "NotImplemented")]
        public void PostActivityFileHasHttpPostAttribute()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void PostActivityFileHasValidateAntiForgeryTokenAttribute()
        {
        }

        [Fact]
        public void ControllerHasAreaAtttributeWithTheCorrectAreaName()
        {
            var sut = new ActivityController(null, null);
            var attribute = sut.GetAttributes().OfType<AreaAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.RouteValue, "Admin");
        }

        [Fact]
        public void ControllerHasAreaAuthorizeAttributeWithCorrectPolicy()
        {
            var sut = new ActivityController(null, null);
            var attribute = sut.GetAttributes().OfType<AuthorizeAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal(attribute.Policy, "OrgAdmin");
        }

        private static ActivityController GetActivityController()
        {
            return GetActivityController(new DateTimeOffset(), new DateTimeOffset());
        }

        private static ActivityController GetActivityController(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var mediator = new Mock<IMediator>();
            var imageService = new Mock<IImageService>();

            mediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryModel { StartDate = startDate, EndDate = endDate });

            var sut = new ActivityController(imageService.Object, mediator.Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            return sut;
        }
    }
}