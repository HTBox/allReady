using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Extensions;
using AllReady.Services;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.UnitTest.Areas.Admin.Controllers.Builders;
using Xunit;
using ClaimTypes = AllReady.Security.ClaimTypes;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class CampaignAdminControllerTests
    {
        private CampaignController GetInstance(IMediator mediator = null,
            IImageService imageService = null)
        {            
            return CampaignAdminControllerBuilder.WithInstances(mediator ?? new Mock<IMediator>().Object,
                imageService ?? new Mock<IImageService>().Object).Build();
        }

        [Fact]
        public async Task IndexSendsIndexQueryWithCorrectData_WhenUserIsOrgAdmin()
        {
            const int organizationId = 99;
            var mockMediator = new Mock<IMediator>();

            var sut = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).Build();
            sut.MakeUserAnOrgAdmin(organizationId.ToString());
            await sut.Index();

            mockMediator.Verify(mock => mock.SendAsync(It.Is<IndexQuery>(q => q.OrganizationId == organizationId)));
        }

        [Fact]
        public async Task IndexSendsIndexQueryWithCorrectData_WhenUserIsNotOrgAdmin()
        {
            var mockMediator = new Mock<IMediator>();
        
            var sut = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).Build();
            sut.MakeUserNotAnOrgAdmin();
            await sut.Index();

            mockMediator.Verify(mock => mock.SendAsync(It.Is<IndexQuery>(q => q.OrganizationId == null)));
        }

        [Fact]
        public async Task DetailsSendsCampaignDetailQueryWithCorrectCampaignId()
        {
            const int campaignId = 100;
            var mockMediator = new Mock<IMediator>();

            var sut = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).Build();
            await sut.Details(campaignId);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<CampaignDetailQuery>(c => c.CampaignId == campaignId)));
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundResultWhenVieModelIsNull()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignDetailQuery>())).ReturnsAsync((CampaignDetailViewModel)null).Verifiable();

            CampaignController sut = GetInstance(mediator: mockMediator.Object);
            Assert.IsType<NotFoundResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsHttpForbiddenResultIfUserIsNotAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignDetailQuery>())).ReturnsAsync(new CampaignDetailViewModel()).Verifiable();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();

            Assert.IsType<ForbidResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewWhenViewModelIsNotNullAndUserIsAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignDetailQuery>())).ReturnsAsync(new CampaignDetailViewModel()).Verifiable();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(true, false, false, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();
            Assert.IsType<ViewResult>(await sut.Details(It.IsAny<int>()));
        }

        [Fact]
        public async Task DetailsReturnsCorrectViewModelWhenViewModelIsNotNullAndUserIsAuthorized()
        {
            const int campaignId = 100;
            const int organizationId = 99;
            var viewModel = new CampaignDetailViewModel { OrganizationId = organizationId, Id = campaignId };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.Is<CampaignDetailQuery>(c => c.CampaignId == campaignId))).ReturnsAsync(viewModel).Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(true, false, false, false));

            // user is org admin
            var sut = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).Build();

            var view = (ViewResult)(await sut.Details(campaignId));
            var resultViewModel = (CampaignDetailViewModel)view.ViewData.Model;
            Assert.Equal(resultViewModel.Id, campaignId);
            Assert.Equal(resultViewModel.OrganizationId, organizationId);
        }

        [Fact]
        public void CreateReturnsCorrectViewWithCorrectViewModel()
        {
            var sut = CampaignAdminControllerBuilder.AllNullParamsInstance().Build();
            sut.SetClaims(new List<Claim>());

            var view = (ViewResult)sut.Create();

            var viewModel = (CampaignSummaryViewModel)view.ViewData.Model;

            Assert.Equal("Edit", view.ViewName);
            Assert.NotNull(viewModel);
        }

        [Fact]
        public void CreateReturnsCorrectDataOnViewModelWhenTimeZoneIdClaimNotSet()
        {
            var dateTimeNow = DateTime.Now;

            var sut = CampaignAdminControllerBuilder.AllNullParamsInstance().WithToday(() => dateTimeNow).Build();
            sut.SetClaims(new List<Claim>());
            var view = (ViewResult)sut.Create();
            var viewModel = (CampaignSummaryViewModel)view.ViewData.Model;

            Assert.Equal(viewModel.StartDate, dateTimeNow);
            Assert.Equal(viewModel.EndDate, dateTimeNow.AddMonths(1));
            Assert.Equal(viewModel.TimeZoneId, "UTC");
        }
        [Fact]
        public void CreateReturnsCorrectDataOnViewModelWhenTimeZoneIdClaimSet()
        {
            var dateTimeNow = DateTime.Now;
            var TimeZoneId = "GMT Standard Time";

          var sut = CampaignAdminControllerBuilder.AllNullParamsInstance().WithToday(() => dateTimeNow).Build();
            sut.SetClaims(new List<Claim>{new Claim(ClaimTypes.TimeZoneId, TimeZoneId) });
            var view = (ViewResult)sut.Create();
            var viewModel = (CampaignSummaryViewModel)view.ViewData.Model;

            Assert.Equal(viewModel.StartDate, dateTimeNow);
            Assert.Equal(viewModel.EndDate, dateTimeNow.AddMonths(1));
            Assert.Equal(viewModel.TimeZoneId, TimeZoneId);
        }

        [Fact]
        public async Task EditGetSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            const int campaignId = 100;
            var mockMediator = new Mock<IMediator>();

            var sut = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).Build();

            await sut.Edit(campaignId);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<CampaignSummaryQuery>(c => c.CampaignId == campaignId)));
        }

        [Fact]
        public async Task EditGetReturnsHttpNotFoundResultWhenViewModelIsNull()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync((CampaignSummaryViewModel)null).Verifiable();

            var sut = GetInstance(mediator: mockMediator.Object);
            Assert.IsType<NotFoundResult>(await sut.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async Task EditGetReturnsForbidResultWhenUserIsNotAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel()).Verifiable();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, false));

            var mockImageService = new Mock<IImageService>();

            var sut = CampaignAdminControllerBuilder.WithInstances(mediator.Object, mockImageService.Object).Build();
            Assert.IsType<ForbidResult>(await sut.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async Task EditGetReturnsCorrectViewModelWhenUserIsAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel()).Verifiable();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(true, false, false, false));

            var mockImageService = new Mock<IImageService>();

            var sut = CampaignAdminControllerBuilder.WithInstances(mediator.Object, mockImageService.Object).Build();
            Assert.IsType<ViewResult>(await sut.Edit(It.IsAny<int>()));
        }

        [Fact]
        public async Task EditPostReturnsBadRequestWhenCampaignIsNull()
        {
            var sut = CampaignAdminControllerBuilder.AllNullParamsInstance().Build();

            var result = await sut.Edit(null, null);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task EditPostAddsCorrectKeyAndErrorMessageToModelStateWhenCampaignEndDateIsLessThanCampainStartDate_WhenCampaignIdIsZero()
        {
            var campaignSummaryModel = new CampaignSummaryViewModel { OrganizationId = 1, StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(-1) };

            var sut = CampaignAdminControllerBuilder.AllNullParamsInstance().Build();
            sut.MakeUserAnOrgAdmin(campaignSummaryModel.OrganizationId.ToString());

            await sut.Edit(campaignSummaryModel, null);
            var modelStateErrorCollection = sut.ModelState.GetErrorMessagesByKey(nameof(CampaignSummaryViewModel.EndDate));

            Assert.Equal("The end date must fall on or after the start date.", modelStateErrorCollection.Single().ErrorMessage);
        }

        [Fact]
        public async Task EditPostAddsCorrectKeyAndErrorMessageToModelStateWhenCampaignEndDateIsLessThanCampainStartDate_WhenCampaignIdIsNotZero()
        {
            const int campaignId = 33;
            var campaignSummaryModel = new CampaignSummaryViewModel { Id = campaignId, OrganizationId = 1, StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(-1) };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();

            await sut.Edit(campaignSummaryModel, null);
            var modelStateErrorCollection = sut.ModelState.GetErrorMessagesByKey(nameof(CampaignSummaryViewModel.EndDate));

            Assert.Equal("The end date must fall on or after the start date.", modelStateErrorCollection.Single().ErrorMessage);
        }

        [Fact]
        public async Task EditPostInsertsCampaign()
        {
            const int organizationId = 99;
            const int newCampaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EditCampaignCommand>())).ReturnsAsync(newCampaignId);

            var mockImageService = new Mock<IImageService>();
            var sut = CampaignAdminControllerBuilder.WithInstances(mockMediator.Object, mockImageService.Object).Build();
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var model = MassiveTrafficLightOutageModel;
            model.OrganizationId = organizationId;

            // verify the model is valid
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults);
            Assert.Empty(validationResults);

            var file = FormFile("image/jpeg");
            var view = (RedirectToActionResult)await sut.Edit(model, file);

            // verify the edit(add) is called
            mockMediator.Verify(mock => mock.SendAsync(It.Is<EditCampaignCommand>(c => c.Campaign.OrganizationId == organizationId)));

            // verify that the next route
            Assert.Equal("Admin", view.RouteValues["area"]);
            Assert.Equal(view.RouteValues["id"], newCampaignId);
        }

        [Fact]
        public async Task EditPostFileTooLarge()
        {
            const int organizationId = 99;
            const int newCampaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EditCampaignCommand>())).ReturnsAsync(newCampaignId);

            var imageSizeValidator = new Mock<IImageSizeValidator>();
            imageSizeValidator.SetupGet(x => x.FileSizeInBytes).Returns(1024);
            imageSizeValidator.Setup(x => x.BytesToMb()).Returns(1);

            var sut = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).WithImageSizeValidator(imageSizeValidator.Object).Build();
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var model = MassiveTrafficLightOutageModel;
            model.OrganizationId = organizationId;

            // verify the model is valid
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults);
            Assert.Empty(validationResults);

            var file = FormFile("image/jpeg");
            await sut.Edit(model, file);

            Assert.False(sut.ModelState.IsValid);
            Assert.Equal("File size must be less than 1.00MB!", sut.ModelState["ImageUrl"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public async Task EditPostReturnsForbidResultResultWhenUserIsNotAuthorized()
        {
            const int campaignId = 100;
            var viewModel = new CampaignSummaryViewModel { Id = campaignId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, false));
          
            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();

            var result = await sut.Edit(viewModel, null);
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task EditPostRedirectsToCorrectActionWithCorrectRouteValuesWhenModelStateIsValid()
        {
            const int organizationId = 99;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EditCampaignCommand>())).ReturnsAsync(campaignId);
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));
            var sut = CampaignAdminControllerBuilder.WithInstances(mockMediator.Object, new Mock<IImageService>().Object).Build();
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var result = (RedirectToActionResult)await sut.Edit(new CampaignSummaryViewModel { Name = "Foo", OrganizationId = organizationId, Id = campaignId }, null);

            Assert.Equal("Details", result.ActionName);
            Assert.Equal("Admin", result.RouteValues["area"]);
            Assert.Equal(result.RouteValues["id"], campaignId);
        }

        [Fact]
        public async Task EditPostAddsErrorToModelStateWhenInvalidImageFormatIsSupplied()
        {
            const int campaignId = 100;
            var viewModel = new CampaignSummaryViewModel { Name = "Foo", Id = campaignId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var mockImageService = new Mock<IImageService>();

            var file = FormFile("");

            var sut = CampaignAdminControllerBuilder.WithInstances(mediator.Object, mockImageService.Object).Build();
            await sut.Edit(viewModel, file);

            Assert.False(sut.ModelState.IsValid);
            Assert.Equal("You must upload a valid image file for the logo (.jpg, .png, .gif)", sut.ModelState["ImageUrl"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public async Task EditPostReturnsCorrectViewModelWhenInvalidImageFormatIsSupplied()
        {
            const int organizationId = 100;
            var mockMediator = new Mock<IMediator>();
            var mockImageService = new Mock<IImageService>();

            var sut = CampaignAdminControllerBuilder.WithInstances(mockMediator.Object, mockImageService.Object).Build();
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var file = FormFile("audio/mpeg3");
            var model = MassiveTrafficLightOutageModel;
            model.OrganizationId = organizationId;

            var view = await sut.Edit(model, file) as ViewResult;
            var viewModel = view.ViewData.Model as CampaignSummaryViewModel;
            Assert.True(ReferenceEquals(model, viewModel));
        }

        [Fact]
        public async Task EditPostInvokesUploadCampaignImageAsyncWithTheCorrectParameters_WhenFileUploadIsNotNull_AndFileIsAcceptableContentType()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));
            var mockImageService = new Mock<IImageService>();

            var sut = CampaignAdminControllerBuilder.WithInstances(mockMediator.Object, mockImageService.Object).Build();
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var file = FormFile("image/jpeg");

            await sut.Edit(new CampaignSummaryViewModel { Name = "Foo", OrganizationId = organizationId, Id = campaignId }, file);

            mockImageService.Verify(mock => mock.UploadCampaignImageAsync(
                It.Is<int>(i => i == organizationId),
                It.Is<int>(i => i == campaignId),
                It.Is<IFormFile>(i => i == file)), Times.Once);
        }

        [Fact]
        public async Task EditPostInvokesDeleteImageAsync_WhenCampaignHasAnImage()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));
            var mockImageService = new Mock<IImageService>();

            var file = FormFile("image/jpeg");
            mockImageService.Setup(x => x.UploadCampaignImageAsync(It.IsAny<int>(), It.IsAny<int>(), file)).ReturnsAsync("newImageUrl");

            var sut = CampaignAdminControllerBuilder.WithInstances(mockMediator.Object, mockImageService.Object).Build();
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var campaignSummaryViewModel = new CampaignSummaryViewModel
            {
                OrganizationId = organizationId,
                ImageUrl = "existingImageUrl",
                Id = campaignId,
                StartDate = new DateTimeOffset(new DateTime(2016, 2, 13)),
                EndDate = new DateTimeOffset(new DateTime(2016, 2, 14)),
            };

            await sut.Edit(campaignSummaryViewModel, file);
            mockImageService.Verify(mock => mock.DeleteImageAsync(It.Is<string>(x => x == "existingImageUrl")), Times.Once);
        }

        [Fact]
        public async Task EditPostDoesNotInvokeDeleteImageAsync__WhenCampaignDoesNotHaveAnImage()
        {
            const int organizationId = 1;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));
            var mockImageService = new Mock<IImageService>();

            var file = FormFile("image/jpeg");
            mockImageService.Setup(x => x.UploadCampaignImageAsync(It.IsAny<int>(), It.IsAny<int>(), file)).ReturnsAsync("newImageUrl");

            var sut = CampaignAdminControllerBuilder.WithInstances(mockMediator.Object, mockImageService.Object).Build();
            sut.MakeUserAnOrgAdmin(organizationId.ToString());

            var campaignSummaryViewModel = new CampaignSummaryViewModel
            {
                OrganizationId = organizationId,
                Id = campaignId,
                StartDate = new DateTimeOffset(new DateTime(2016, 2, 13)),
                EndDate = new DateTimeOffset(new DateTime(2016, 2, 14)),
            };

            await sut.Edit(campaignSummaryViewModel, file);
            mockImageService.Verify(mock => mock.DeleteImageAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void EditPostHasHttpPostAttribute()
        {
            var attr = (HttpPostAttribute)typeof(CampaignController).GetMethod(nameof(CampaignController.Edit), new Type[] { typeof(CampaignSummaryViewModel), typeof(IFormFile) }).GetCustomAttribute(typeof(HttpPostAttribute));
            Assert.NotNull(attr);
        }

        [Fact]
        public void EditPostHasValidateAntiForgeryTokenAttribute()
        {
            var attr = (ValidateAntiForgeryTokenAttribute)typeof(CampaignController).GetMethod(nameof(CampaignController.Edit), new Type[] { typeof(CampaignSummaryViewModel), typeof(IFormFile) }).GetCustomAttribute(typeof(ValidateAntiForgeryTokenAttribute));
            Assert.NotNull(attr);
        }

        [Fact]
        public async Task PublishSendsPublishQueryWithCorrectCampaignId()
        {
            const int organizationId = 99;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.Is<PublishViewModelQuery>(c => c.CampaignId == campaignId))).ReturnsAsync(new PublishViewModel { Id = campaignId, OrganizationId = organizationId });
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(true, false, false, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).Build();
            sut.MakeUserAnOrgAdmin(organizationId.ToString());
            await sut.Publish(campaignId);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<PublishViewModelQuery>(c => c.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task PublishReturnsHttpNotFoundResultWhenCampaignIsNotFound()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync((CampaignSummaryViewModel)null).Verifiable();

            var sut = GetInstance(mediator: mockMediator.Object);
            Assert.IsType<NotFoundResult>(await sut.Publish(It.IsAny<int>()));
        }

        [Fact]
        public async Task PublishReturnsHttpForbidResultWhenUserIsNotAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<PublishViewModelQuery>())).ReturnsAsync(new PublishViewModel());
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();

            Assert.IsType<ForbidResult>(await sut.Publish(It.IsAny<int>()));
        }

        [Fact]
        public async Task PublishReturnsCorrectViewWhenUserIsAuthorized()
        {
            const int campaignId = 100;
            var viewModel = new PublishViewModel { Id = campaignId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.Is<PublishViewModelQuery>(c => c.CampaignId == campaignId))).ReturnsAsync(viewModel);
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(true, false, false, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();

            Assert.IsType<ViewResult>(await sut.Publish(campaignId));
        }

        [Fact]
        public async Task PublishReturnsCorrectViewModelWhenUserIsAuthorized()
        {
            const int campaignId = 100;
            var viewModel = new PublishViewModel { Id = campaignId };

            var mediator = new Mock<IMediator>();
            mediator.Setup(mock => mock.SendAsync(It.Is<PublishViewModelQuery>(c => c.CampaignId == campaignId))).ReturnsAsync(viewModel);
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(true, false, false, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();

            var view = (ViewResult)await sut.Publish(campaignId);
            var resultViewModel = (PublishViewModel)view.ViewData.Model;

            Assert.Equal(resultViewModel.Id, campaignId);
        }

        [Fact]
        public async Task DeleteSendsDeleteQueryWithCorrectCampaignId()
        {
            const int organizationId = 99;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.Is<DeleteViewModelQuery>(c => c.CampaignId == campaignId))).ReturnsAsync(new DeleteViewModel { Id = campaignId, OrganizationId = organizationId });
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AllReady.Areas.Admin.Features.Organizations.AuthorizableOrganizationQuery>())).ReturnsAsync(new FakeAuthorizableOrganization(false, false, true, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).Build();
            await sut.Delete(campaignId);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<DeleteViewModelQuery>(c => c.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task DeleteReturnsHttpNotFoundResultWhenCampaignIsNotFound()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync((CampaignSummaryViewModel)null).Verifiable();

            var sut = GetInstance(mediator: mockMediator.Object);
            Assert.IsType<NotFoundResult>(await sut.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteReturnsForbidResultWhenUserIsNotAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteViewModelQuery>())).ReturnsAsync(new DeleteViewModel());
            mediator.Setup(x => x.SendAsync(It.IsAny<AllReady.Areas.Admin.Features.Organizations.AuthorizableOrganizationQuery>())).ReturnsAsync(new FakeAuthorizableOrganization(false, false, false, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();

            Assert.IsType<ForbidResult>(await sut.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteReturnsCorrectViewWhenUserIsOrgAdmin()
        {
            const int organizationId = 1;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteViewModelQuery>())).ReturnsAsync(new DeleteViewModel { OrganizationId = organizationId });
            mediator.Setup(x => x.SendAsync(It.IsAny<AllReady.Areas.Admin.Features.Organizations.AuthorizableOrganizationQuery>())).ReturnsAsync(new FakeAuthorizableOrganization(false, false, true, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();

            Assert.IsType<ViewResult>(await sut.Delete(It.IsAny<int>()));
        }

        [Fact]
        public async Task DeleteReturnsCorrectViewModelWhenUserIsOrgAdmin()
        {
            const int organizationId = 99;
            const int campaignId = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.Is<DeleteViewModelQuery>(c => c.CampaignId == campaignId))).ReturnsAsync(new DeleteViewModel { Id = campaignId, OrganizationId = organizationId });
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AllReady.Areas.Admin.Features.Organizations.AuthorizableOrganizationQuery>())).ReturnsAsync(new FakeAuthorizableOrganization(false, false, true, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).Build();

            var view = (ViewResult)await sut.Delete(campaignId);
            var viewModel = (DeleteViewModel)view.ViewData.Model;

            Assert.Equal(viewModel.Id, campaignId);
        }

        [Fact]
        public async Task DeleteCampaignImageReturnsJsonObjectWithStatusOfNotFound()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync((CampaignSummaryViewModel)null);
            var imageServiceMock = new Mock<IImageService>();
            var sut = CampaignAdminControllerBuilder.WithInstances(mediatorMock.Object, imageServiceMock.Object).Build();

            var result = await sut.DeleteCampaignImage(It.IsAny<int>());

            result.ShouldNotBeNull();
            result.ShouldBeOfType<JsonResult>();

            result.Value.GetType()
                .GetProperty("status")
                .GetValue(result.Value)
                .ShouldBe("NotFound");
        }

        [Fact]
        public async Task DeleteCampaignImageReturnsJsonObjectWithStatusOfUnauthorizedIfUserIsNotAuthorized()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel());
            mediatorMock.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, false));
            var imageServiceMock = new Mock<IImageService>();
            var sut = CampaignAdminControllerBuilder.WithInstances(mediatorMock.Object, imageServiceMock.Object).Build();

            var result = await sut.DeleteCampaignImage(It.IsAny<int>());

            result.Value.GetType()
                .GetProperty("status")
                .GetValue(result.Value)
                .ShouldBe("Unauthorized");
        }

        [Fact]
        public async Task DeleteCampaignSendsTheCorrectIdToCampaignSummaryQuery()
        {
            var mediatorMock = new Mock<IMediator>();
            var imageServiceMock = new Mock<IImageService>();
            var sut = CampaignAdminControllerBuilder.WithInstances(mediatorMock.Object, imageServiceMock.Object).Build();
            const int campaignId = 2;

            await sut.DeleteCampaignImage(campaignId);

            mediatorMock.Verify(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>()), Times.Once);
            mediatorMock.Verify(m => m.SendAsync(It.Is<CampaignSummaryQuery>(s => s.CampaignId == campaignId)));
        }

        [Fact]
        public async Task DeleteCampaignImageReturnsJsonObjectWithStatusOfDateInvalidIfCampaignEndDateIsLessThanStartDate()
        {
            var mediatorMock = new Mock<IMediator>();

            var campaignSummaryViewModel = new CampaignSummaryViewModel
            {
                OrganizationId = 1,
                StartDate = DateTimeOffset.Now.AddDays(10),
                EndDate = DateTimeOffset.Now
            };
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);
            mediatorMock.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var imageServiceMock = new Mock<IImageService>();

            var sut = CampaignAdminControllerBuilder.WithInstances(mediatorMock.Object, imageServiceMock.Object).Build();
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            var result = await sut.DeleteCampaignImage(It.IsAny<int>());

            result.Value.GetType()
                .GetProperty("status")
                .GetValue(result.Value)
                .ShouldBe("DateInvalid");

            result.Value.GetType()
                .GetProperty("message")
                .GetValue(result.Value)
                .ShouldBe("The end date must fall on or after the start date.");
        }

        [Fact]
        public async Task DeleteCampaignImageCallsDeleteImageAsyncWithCorrectData()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel
            {
                OrganizationId = 1,
                ImageUrl = "URL!"
            };

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);
            mediatorMock.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var imageServiceMock = new Mock<IImageService>();

            var sut = CampaignAdminControllerBuilder.WithInstances(mediatorMock.Object, imageServiceMock.Object).Build();
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            await sut.DeleteCampaignImage(It.IsAny<int>());

            imageServiceMock.Verify(i => i.DeleteImageAsync(It.Is<string>(f => f == "URL!")), Times.Once);
        }

        [Fact]
        public async Task DeleteCampaignImageCallsEditCampaignCommandWithCorrectData()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel
            {
                OrganizationId = 1,
                ImageUrl = "URL!"
            };

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);
            mediatorMock.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var imageServiceMock = new Mock<IImageService>();

            var sut = CampaignAdminControllerBuilder.WithInstances(mediatorMock.Object, imageServiceMock.Object).Build();
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            await sut.DeleteCampaignImage(It.IsAny<int>());

            mediatorMock.Verify(m => m.SendAsync(It.Is<EditCampaignCommand>(s => s.Campaign == campaignSummaryViewModel)), Times.Once);
        }

        [Fact]
        public async Task DeleteCampaignImageReturnsJsonObjectWithStatusOfSuccessIfImageDeletedSuccessfully()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel
            {
                OrganizationId = 1,
                ImageUrl = "URL!"
            };

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);
            mediatorMock.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var imageServiceMock = new Mock<IImageService>();

            var sut = CampaignAdminControllerBuilder.WithInstances(mediatorMock.Object, imageServiceMock.Object).Build();
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            var result = await sut.DeleteCampaignImage(It.IsAny<int>());

            result.Value.GetType()
                .GetProperty("status")
                .GetValue(result.Value)
                .ShouldBe("Success");
        }

        [Fact]
        public async Task DeleteCampaignImageReturnsJsonObjectWithStatusOfNothingToDeleteIfThereWasNoExistingImage()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel
            {
                OrganizationId = 1
            };

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);
            mediatorMock.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var imageServiceMock = new Mock<IImageService>();

            var sut = CampaignAdminControllerBuilder.WithInstances(mediatorMock.Object, imageServiceMock.Object).Build();
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            var result = await sut.DeleteCampaignImage(It.IsAny<int>());

            result.Value.GetType()
                .GetProperty("status")
                .GetValue(result.Value)
                .ShouldBe("NothingToDelete");
        }

        [Fact]
        public async Task DeleteConfirmedSendsDeleteCampaignCommandWithCorrectCampaignId()
        {
            int id = 1;
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel { Id = id, OrganizationId = 100 });
            mediator.Setup(x => x.SendAsync(It.IsAny<AllReady.Areas.Admin.Features.Organizations.AuthorizableOrganizationQuery>())).ReturnsAsync(new FakeAuthorizableOrganization(false, false, true, false, 1));
            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();
            await sut.DeleteConfirmed(id);

            mediator.Verify(mock => mock.SendAsync(It.Is<DeleteCampaignCommand>(i => i.CampaignId == id)), Times.Once);
        }

        [Fact]
        public async Task DetailConfirmedReturnsForbidResultWhenUserIsNotAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel { Id = 1, OrganizationId = 100 });
            mediator.Setup(x => x.SendAsync(It.IsAny<AllReady.Areas.Admin.Features.Organizations.AuthorizableOrganizationQuery>())).ReturnsAsync(new FakeAuthorizableOrganization(false, false, false, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();
            Assert.IsType<ForbidResult>(await sut.DeleteConfirmed(1));
        }

        [Fact]
        public async Task DetailConfirmedSendsDeleteCampaignCommandWithCorrectCampaignIdWhenUserIsAuthorized()
        {
            int id = 100;

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel { Id = id, OrganizationId = 100 });
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AllReady.Areas.Admin.Features.Organizations.AuthorizableOrganizationQuery>())).ReturnsAsync(new FakeAuthorizableOrganization(false, false, true, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).Build();
            await sut.DeleteConfirmed(id);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<DeleteCampaignCommand>(i => i.CampaignId == id)), Times.Once);
        }

        [Fact]
        public async Task DetailConfirmedRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsAuthorized()
        {
            int id = 100;

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(new CampaignSummaryViewModel { Id = id, OrganizationId = 100 });
            mediator.Setup(x => x.SendAsync(It.IsAny<AllReady.Areas.Admin.Features.Organizations.AuthorizableOrganizationQuery>())).ReturnsAsync(new FakeAuthorizableOrganization(false, false, true, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();

            var routeValues = new Dictionary<string, object> { ["area"] = "Admin" };

            var result = await sut.DeleteConfirmed(id) as RedirectToActionResult;
            Assert.Equal(nameof(CampaignController.Index), result.ActionName);
            Assert.Equal(result.RouteValues, routeValues);
        }

        [Fact]
        public void DeleteConfirmedHasHttpPostAttribute()
        {
            var sut = CampaignAdminControllerBuilder.AllNullParamsInstance().Build();
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<int>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void DeleteConfirmedHasActionNameAttributeWithCorrectName()
        {
            var sut = CampaignAdminControllerBuilder.AllNullParamsInstance().Build();
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<int>())).OfType<ActionNameAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("Delete", attribute.Name);
        }

        [Fact]
        public void DeleteConfirmedHasValidateAntiForgeryTokenAttribute()
        {
            var sut = CampaignAdminControllerBuilder.AllNullParamsInstance().Build();
            var attribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<int>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task PublishConfirmedSendsPublishCampaignCommandWithCorrectCampaignId()
        {
            var viewModel = new PublishViewModel { Id = 1 };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false, 1));
            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();
            await sut.PublishConfirmed(viewModel);

            mediator.Verify(mock => mock.SendAsync(It.Is<PublishCampaignCommand>(i => i.CampaignId == viewModel.Id)), Times.Once);
        }

        [Fact]
        public async Task PublishConfirmedReturnsForbiddResultWhenUserIsNotAuthorized()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();
            Assert.IsType<ForbidResult>(await sut.PublishConfirmed(new PublishViewModel { UserIsOrgAdmin = false }));
        }

        [Fact]
        public async Task PublishConfirmedSendsPublishCampaignCommandWithCorrectCampaignIdWhenUserIsAuthorized()
        {
            var viewModel = new PublishViewModel { Id = 100, UserIsOrgAdmin = true };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();

            await sut.PublishConfirmed(viewModel);

            mediator.Verify(mock => mock.SendAsync(It.Is<PublishCampaignCommand>(i => i.CampaignId == viewModel.Id)), Times.Once);
        }

        [Fact]
        public async Task PublishConfirmedRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsAuthorized()
        {
            var viewModel = new PublishViewModel { Id = 100, UserIsOrgAdmin = true };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var sut = CampaignAdminControllerBuilder.WithMediator(mediator.Object).Build();

            var routeValues = new Dictionary<string, object> { ["area"] = "Admin" };

            var result = await sut.PublishConfirmed(viewModel) as RedirectToActionResult;
            Assert.Equal(nameof(CampaignController.Index), result.ActionName);
            Assert.Equal(result.RouteValues, routeValues);
        }

        [Fact]
        public void PublishConfirmedHasHttpPostAttribute()
        {
            var sut = CampaignAdminControllerBuilder.AllNullParamsInstance().Build();
            var attribute = sut.GetAttributesOn(x => x.PublishConfirmed(It.IsAny<PublishViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void PublishConfirmedHasActionNameAttributeWithCorrectName()
        {
            var sut = CampaignAdminControllerBuilder.AllNullParamsInstance().Build();
            var attribute = sut.GetAttributesOn(x => x.PublishConfirmed(It.IsAny<PublishViewModel>())).OfType<ActionNameAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
            Assert.Equal("Publish", attribute.Name);
        }

        [Fact]
        public void PublishConfirmedHasValidateAntiForgeryTokenAttribute()
        {
            var sut = CampaignAdminControllerBuilder.AllNullParamsInstance().Build();
            var attribute = sut.GetAttributesOn(x => x.PublishConfirmed(It.IsAny<PublishViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public async Task LockUnlockReturnsForbidResultWhenUserIsNotOrgAdmin()
        {
            var sut = CampaignAdminControllerBuilder.AllNullParamsInstance().Build();
            sut.MakeUserAnOrgAdmin("1");
            Assert.IsType<ForbidResult>(await sut.LockUnlock(100));
        }

        [Fact]
        public async Task LockUnlockSendsLockUnlockCampaignCommandWithCorrectCampaignIdWhenUserIsSiteAdmin()
        {
            const int campaignId = 99;
            var mockMediator = new Mock<IMediator>();

            var sut = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).Build();
            sut.MakeUserASiteAdmin();

            await sut.LockUnlock(campaignId);

            mockMediator.Verify(mock => mock.SendAsync(It.Is<LockUnlockCampaignCommand>(q => q.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task LockUnlockRedirectsToCorrectActionWithCorrectRouteValuesWhenUserIsSiteAdmin()
        {
            const int campaignId = 100;
            var mockMediator = new Mock<IMediator>();

            var sut = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).Build();
            sut.MakeUserASiteAdmin();

            var view = (RedirectToActionResult)await sut.LockUnlock(campaignId);

            // verify the next route
            Assert.Equal(nameof(CampaignController.Details), view.ActionName);
            Assert.Equal("Admin", view.RouteValues["area"]);
            Assert.Equal(view.RouteValues["id"], campaignId);
        }

        [Fact]
        public void LockUnlockHasHttpPostAttribute()
        {
            var attr = (HttpPostAttribute)typeof(CampaignController).GetMethod(nameof(CampaignController.LockUnlock), new Type[] { typeof(int) }).GetCustomAttribute(typeof(HttpPostAttribute));
            Assert.NotNull(attr);
        }

        [Fact]
        public void LockUnlockdHasValidateAntiForgeryTokenAttribute()
        {
            var attr = (ValidateAntiForgeryTokenAttribute)typeof(CampaignController).GetMethod(nameof(CampaignController.LockUnlock), new Type[] { typeof(int) }).GetCustomAttribute(typeof(ValidateAntiForgeryTokenAttribute));
            Assert.NotNull(attr);
        }

        private static void MockMediatorCampaignDetailQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignDetailQuery>())).ReturnsAsync((CampaignDetailViewModel)null).Verifiable();

            controller = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).Build();

            return;
        }

        private static void MockMediatorCampaignSummaryQuery(out CampaignController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync((CampaignSummaryViewModel)null).Verifiable();

            controller = CampaignAdminControllerBuilder.WithMediator(mockMediator.Object).Build();
        }

        private static IFormFile FormFile(string fileType, long length = (1 * 1024 * 1024)+1)
        {
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(mock => mock.ContentType).Returns(fileType);
            mockFormFile.Setup(mock => mock.Length).Returns(length);
            return mockFormFile.Object;
        }

        private static LocationEditViewModel BogusAveModel => new LocationEditViewModel
        {
            Address1 = "25 Bogus Ave",
            City = "Agincourt",
            State = "Ontario",
            Country = "Canada",
            PostalCode = "M1T2T9"
        };

        private static CampaignSummaryViewModel MassiveTrafficLightOutageModel => new CampaignSummaryViewModel
        {
            Description = "Preparations to be ready to deal with a wide-area traffic outage.",
            EndDate = DateTime.Today.AddMonths(1),
            ExternalUrl = "http://agincourtaware.trafficlightoutage.com",
            ExternalUrlText = "Agincourt Aware: Traffic Light Outage",
            Featured = false,
            FileUpload = null,
            FullDescription = "<h1><strong>Massive Traffic Light Outage Plan</strong></h1>\r\n<p>The Massive Traffic Light Outage Plan (MTLOP) is the official plan to handle a major traffic light failure.</p>\r\n<p>In the event of a wide-area traffic light outage, an alternative method of controlling traffic flow will be necessary. The MTLOP calls for the recruitment and training of volunteers to be ready to direct traffic at designated intersections and to schedule and follow-up with volunteers in the event of an outage.</p>",
            Id = 0,
            ImageUrl = null,
            Location = BogusAveModel,
            Locked = false,
            Name = "Massive Traffic Light Outage Plan",
            PrimaryContactEmail = "mlong@agincourtawareness.com",
            PrimaryContactFirstName = "Miles",
            PrimaryContactLastName = "Long",
            PrimaryContactPhoneNumber = "416-555-0119",
            StartDate = DateTime.Today,
            TimeZoneId = "Eastern Standard Time",
        };
    }
}
