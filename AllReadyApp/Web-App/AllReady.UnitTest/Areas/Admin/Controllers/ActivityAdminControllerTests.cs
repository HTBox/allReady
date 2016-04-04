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

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class ActivityAdminControllerTests
    {
        [Fact]
        public async Task CreateReturnsEditView_When_ModelStateNotValid()
        {
            var sut = GetActivityController();
            sut.ModelState.AddModelError("test", "test");
            var activityModel = new ActivityDetailModel();
            var result = (ViewResult)await sut.Create(1, activityModel, null);

            Assert.Equal("Edit", result.ViewName);
        }

        [Fact]
        public async Task CreateReturnsEditView_When_ActivityEndDateBeforeStartDate()
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
        public async Task CreateReturnsEditView_When_ActivityStartDateBeforeCampaignStartDate()
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
        public async Task CreateReturnsEditView_When_ActivityEndDateAfterCampaignEndDate()
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
        //public async Task CreateReturnsEditView_When_ModelStateNotValid_And_ImageIsNotNull()
        //{
        //    var sut = GetActivityController();
        //    var activityModel = new ActivityDetailModel();
        //    IFormFile file = new FormFile(null, 0, 0);
        //    var result = (ViewResult)await sut.Create(1, activityModel, file);

        //    Assert.Equal("Edit", result.ViewName);
        //}

        private static ActivityController GetActivityController()
        {
            return GetActivityController(new DateTimeOffset(), new DateTimeOffset());
        }

        private static ActivityController GetActivityController(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var mediator = new Mock<IMediator>();
            var dataAccess = new Mock<IAllReadyDataAccess>();
            var imageService = new Mock<IImageService>();

            mediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryModel { StartDate = startDate, EndDate = endDate });

            var sut = new ActivityController(dataAccess.Object, imageService.Object, mediator.Object);
            sut.SetClaims(new List<Claim>
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString()),
                new Claim(AllReady.Security.ClaimTypes.Organization, "1")
            });

            return sut;
        }
    }
}