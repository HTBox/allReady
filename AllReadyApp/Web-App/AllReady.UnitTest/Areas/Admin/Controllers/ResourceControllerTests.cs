using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Features.Resource;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Areas.Admin.ViewModels.Resource;
using AllReady.Constants;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Shouldly;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class ResourceControllerTests
    {
        [Fact]
        public async Task CreateGetReturnsUnauthorized_WhenCampaignIsNull()
        {
            var mockMediator = new Mock<IMediator>();

            var sut = new ResourceController(mockMediator.Object);

            var result = await sut.Create(It.IsAny<int>()) as UnauthorizedResult;

            result.ShouldNotBeNull();
            result.ShouldBeOfType<UnauthorizedResult>();
            result.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateGetReturnsUnauthorized_WhenUserIsNotOrganizationAdmin()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);

            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserNotAnOrgAdmin();

            var result = await sut.Create(It.IsAny<int>()) as UnauthorizedResult; 

            result.ShouldNotBeNull();
            result.ShouldBeOfType<UnauthorizedResult>();
            result.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateGetReturnsTheCorrectViewModel()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);
            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            var result = await sut.Create(campaignSummaryViewModel.Id) as ViewResult;

            result.Model.ShouldNotBeNull();
            result.Model.ShouldBeOfType<ResourceEditViewModel>();
            var model = result.Model as ResourceEditViewModel;
            model.CampaignId.ShouldBe(campaignSummaryViewModel.Id);
        }

        [Fact]
        public async Task CreateGetSendsCampaignSummaryViewModelQueryWithCorrectCampaignId()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);
            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            var result = await sut.Create(campaignSummaryViewModel.Id) as ViewResult;

            result.ShouldNotBeNull();
            mockMediator.Verify(m => m.SendAsync(It.Is<CampaignSummaryQuery>(c => c.CampaignId == campaignSummaryViewModel.Id)), Times.Once);
        }

        [Fact]
        public void CreateGetHasRouteAttributeWithCorrectRoute()
        {
            var sut = ResourceControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.Create(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal("Admin/Resource/Create/{campaignId}", routeAttribute.Template);
        }

        [Fact]
        public async Task CreatePostReturnsUnauthorized_WhenCampaignIsNull()
        {
            var mockMediator = new Mock<IMediator>();

            var sut = new ResourceController(mockMediator.Object);

            var result = await sut.Create(new ResourceEditViewModel()) as UnauthorizedResult;

            result.ShouldNotBeNull();
            result.ShouldBeOfType<UnauthorizedResult>();
            result.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreatePostReturnsUnauthorized_WhenUserIsNotOrganizationAdmin()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);

            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserNotAnOrgAdmin();

            var result = await sut.Create(new ResourceEditViewModel()) as UnauthorizedResult;

            result.ShouldNotBeNull();
            result.ShouldBeOfType<UnauthorizedResult>();
            result.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreatePostReturnsView_IfModelStateIsNotValid()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);

            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());
            sut.ModelState.AddModelError("Name", "Filling name is mandatory!");
            var result = await sut.Create(new ResourceEditViewModel()) as ViewResult;

            result.ShouldNotBeNull();
            result.ViewName.ShouldBe("Edit");
            result.Model.ShouldBeOfType<ResourceEditViewModel>();
        }

        [Fact]
        public async Task CreatePostSendsEditResourceCommandWithCorrectViewModel()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);

            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());
            var resourceEditViewModel = new ResourceEditViewModel();
            var result = await sut.Create(resourceEditViewModel) as RedirectToActionResult;

            result.ShouldNotBeNull();
            mockMediator.Verify(m => m.SendAsync(It.Is<CreateOrEditResourceCommand>(e => e.Resource == resourceEditViewModel)), Times.Once);
        }

        [Fact]
        public async Task CreatePostRedirectToActionWithTheCorrectCampaignId_AfterRsourceCreated()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);

            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());
            var resourceEditViewModel = new ResourceEditViewModel();
            var result = await sut.Create(resourceEditViewModel) as RedirectToActionResult;

            result.ShouldNotBeNull();
            result.ShouldBeOfType<RedirectToActionResult>();
            result.ActionName.ShouldBe(nameof(CampaignController.Details));
            result.ControllerName.ShouldBe(nameof(Campaign));
            result.RouteValues["area"].ShouldBe(AreaNames.Admin);
            result.RouteValues["id"].ShouldBe(resourceEditViewModel.CampaignId);
        }

        [Fact]
        public void CreatePostHasRouteAttributeWithCorrectRoute()
        {
            var sut = ResourceControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.Create(It.IsAny<ResourceEditViewModel>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal("Admin/Resource/Create/{campaignId}", routeAttribute.Template);
        }

        [Fact]
        public async Task DetailsSendsResourceDetailQuery_WithCorrectResourceId()
        {
            var mockMediator = new Mock<IMediator>();
            var resourceDetailViewModel = new ResourceDetailViewModel { Id = 2 };
            mockMediator.Setup(m => m.SendAsync(It.IsAny<ResourceDetailQuery>())).ReturnsAsync(It.IsAny<ResourceDetailViewModel>());
            var sut = new ResourceController(mockMediator.Object);

            var result = await sut.Details(resourceDetailViewModel.Id);

            result.ShouldNotBeNull();
            mockMediator.Verify(m => m.SendAsync(It.Is<ResourceDetailQuery>(r => r.ResourceId == resourceDetailViewModel.Id)));
        }

        [Fact]
        public async Task DetailsReturnsTheCorrectView()
        {
            var mockMediator = new Mock<IMediator>();
            var resourceDetailViewModel = new ResourceDetailViewModel { Id = 2 };
            mockMediator.Setup(m => m.SendAsync(It.IsAny<ResourceDetailQuery>())).ReturnsAsync(It.IsAny<ResourceDetailViewModel>());
            var sut = new ResourceController(mockMediator.Object);

            var result = await sut.Details(resourceDetailViewModel.Id) as ViewResult;

            result.ShouldNotBeNull();
            result.ViewName.ShouldBeNull();
        }

        [Fact]
        public async Task DetailsReturnsTheCorrectViewModel()
        {
            var mockMediator = new Mock<IMediator>();
            var resourceDetailViewModel = new ResourceDetailViewModel { Id = 2 };
            mockMediator.Setup(m => m.SendAsync(It.IsAny<ResourceDetailQuery>())).ReturnsAsync(resourceDetailViewModel);
            var sut = new ResourceController(mockMediator.Object);

            var result = await sut.Details(resourceDetailViewModel.Id) as ViewResult;

            result.ShouldNotBeNull();
            result.Model.ShouldBeOfType<ResourceDetailViewModel>();
            result.Model.ShouldBeSameAs(resourceDetailViewModel);
        }

        [Fact]
        public void DetailsHasRouteAttributeWithCorrectRoute()
        {
            var sut = ResourceControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal("Admin/Resource/Details/{resourceId}", routeAttribute.Template);
        }

        [Fact]
        public async Task DeleteGetReturnsUnauthorized_WhenCampaignIsNull()
        {
            var mockMediator = new Mock<IMediator>();

            mockMediator.Setup(m => m.SendAsync(It.IsAny<DeleteResourceQuery>())).ReturnsAsync(new ResourceDeleteViewModel());

            var sut = new ResourceController(mockMediator.Object);

            var result = await sut.Delete(It.IsAny<int>()) as UnauthorizedResult;

            result.ShouldNotBeNull();
            result.ShouldBeOfType<UnauthorizedResult>();
            result.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteGetReturnsUnauthorized_WhenUserIsNotOrganizationAdmin()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<DeleteResourceQuery>())).ReturnsAsync(new ResourceDeleteViewModel());
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);

            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserNotAnOrgAdmin();

            var result = await sut.Delete(It.IsAny<int>()) as UnauthorizedResult;

            result.ShouldNotBeNull();
            result.ShouldBeOfType<UnauthorizedResult>();
            result.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteReturnsTheCorrectViewModel()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };
            var deleteResourceQuery = new ResourceDeleteViewModel { Id = 1, CampaignId = 4 };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<DeleteResourceQuery>())).ReturnsAsync(deleteResourceQuery);
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);

            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            var result = await sut.Delete(It.IsAny<int>()) as ViewResult;

            result.ShouldNotBeNull();
            result.ShouldBeOfType<ViewResult>();
            result.Model.ShouldBeOfType<ResourceDeleteViewModel>();
            result.Model.ShouldBeSameAs(deleteResourceQuery);
        }

        [Fact]
        public async Task DeleteReturnsTheCorrectView()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };
            var deleteResourceQuery = new ResourceDeleteViewModel { Id = 1, CampaignId = 4 };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<DeleteResourceQuery>())).ReturnsAsync(deleteResourceQuery);
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);

            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            var result = await sut.Delete(It.IsAny<int>()) as ViewResult;

            result.ShouldNotBeNull();
            result.ViewName.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteGetSendsCampaignSummaryViewModelQueryWithCorrectCampaignId()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };
            var deleteResourceQuery = new ResourceDeleteViewModel { Id = 1, CampaignId = 4 };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<DeleteResourceQuery>())).ReturnsAsync(deleteResourceQuery);
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);
            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            var result = await sut.Delete(campaignSummaryViewModel.Id) as ViewResult;

            result.ShouldNotBeNull();
            mockMediator.Verify(m => m.SendAsync(It.Is<CampaignSummaryQuery>(c => c.CampaignId == campaignSummaryViewModel.Id)), Times.Once);
        }

        [Fact]
        public async Task DeleteGetSendsDeleteResourceQueryWithCorrectResourceId()
        {
            var deleteResourceQuery = new ResourceDeleteViewModel { Id = 1, CampaignId = 4 };
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<DeleteResourceQuery>())).ReturnsAsync(deleteResourceQuery);
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);
            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            var result = await sut.Delete(deleteResourceQuery.Id) as ViewResult;

            result.ShouldNotBeNull();
            mockMediator.Verify(m => m.SendAsync(It.Is<DeleteResourceQuery>(c => c.ResourceId == deleteResourceQuery.Id)), Times.Once);
        }

        [Fact]
        public async Task DeleteGetReturnsTheCorrectView()
        {
            var deleteResourceQuery = new ResourceDeleteViewModel { Id = 1, CampaignId = 4 };
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<DeleteResourceQuery>())).ReturnsAsync(deleteResourceQuery);
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);
            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            var result = await sut.Delete(deleteResourceQuery.Id) as ViewResult;

            result.ShouldNotBeNull();
            result.ViewName.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteConfirmedReturnsUnauthorized_IfUserIsNotOrgAdmin()
        {
            var deleteResourceQuery = new ResourceDeleteViewModel { Id = 1, CampaignId = 4, UserIsOrgAdmin = false };
            var mockMediator = new Mock<IMediator>();

            var sut = new ResourceController(mockMediator.Object);
            var result = await sut.DeleteConfirmed(deleteResourceQuery) as UnauthorizedResult;

            result.ShouldNotBeNull();
            result.ShouldBeOfType<UnauthorizedResult>();
            result.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteConfirmdedSendsDeleteResourceCommand_WithCorrectResourceId()
        {
            var deleteResourceQuery = new ResourceDeleteViewModel { Id = 1, CampaignId = 4, UserIsOrgAdmin = true };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<DeleteResourceQuery>())).ReturnsAsync(It.IsAny<ResourceDeleteViewModel>());

            var sut = new ResourceController(mockMediator.Object);
            var result = await sut.DeleteConfirmed(deleteResourceQuery) as RedirectToActionResult;

            result.ShouldNotBeNull();
            mockMediator.Verify(m => m.SendAsync(It.Is<DeleteResourceCommand>(d => d.ResourceId == deleteResourceQuery.Id)));
        }

        [Fact]
        public async Task DeleteConfirmdedRedirectToCorrectAction_AfterDeleteHasBeenDone()
        {
            var deleteResourceQuery = new ResourceDeleteViewModel { Id = 1, CampaignId = 4, UserIsOrgAdmin = true };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<DeleteResourceQuery>())).ReturnsAsync(It.IsAny<ResourceDeleteViewModel>());

            var sut = new ResourceController(mockMediator.Object);
            var result = await sut.DeleteConfirmed(deleteResourceQuery) as RedirectToActionResult;

            result.ShouldNotBeNull();
            result.ActionName.ShouldBe(nameof(CampaignController.Details));
            result.ControllerName.ShouldBe("Campaign");
            result.RouteValues["area"].ShouldBe(AreaNames.Admin);
            result.RouteValues["id"].ShouldBe(deleteResourceQuery.CampaignId);
        }

        [Fact]
        public void DeleteConfirmdHasActionNameAttributeOfDelete()
        {
            var sut = ResourceControllerWithNoInjectedDependencies();
            var actionNameAttribute = sut.GetAttributesOn(x => x.DeleteConfirmed(It.IsAny<ResourceDeleteViewModel>())).OfType<ActionNameAttribute>().SingleOrDefault();
            Assert.NotNull(actionNameAttribute);
            Assert.Equal("Delete", actionNameAttribute.Name);
        }

        private static ResourceController ResourceControllerWithNoInjectedDependencies()
        {
            return new ResourceController(null);
        }

        [Fact]
        public async Task EditGetReturnsUnauthorized_WhenCampaignIsNull()
        {
            var mockMediator = new Mock<IMediator>();

            mockMediator.Setup(m => m.SendAsync(It.IsAny<ResourceDetailQuery>())).ReturnsAsync(new ResourceDetailViewModel());

            var sut = new ResourceController(mockMediator.Object);

            var result = await sut.Edit(It.IsAny<int>()) as UnauthorizedResult;

            result.ShouldNotBeNull();
            result.ShouldBeOfType<UnauthorizedResult>();
            result.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task EditGetReturnsUnauthorized_WhenUserIsNotOrganizationAdmin()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<ResourceDetailQuery>())).ReturnsAsync(new ResourceDetailViewModel());
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);

            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserNotAnOrgAdmin();

            var result = await sut.Edit(It.IsAny<int>()) as UnauthorizedResult;

            result.ShouldNotBeNull();
            result.ShouldBeOfType<UnauthorizedResult>();
            result.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task EditGetReturnsTheCorrectViewModel()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };
            var resourceDetailViewModel = new ResourceDetailViewModel { Id = 5 };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<ResourceDetailQuery>())).ReturnsAsync(resourceDetailViewModel);
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);

            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            var result = await sut.Edit(It.IsAny<int>()) as ViewResult;

            result.ShouldNotBeNull();
            result.Model.ShouldBeOfType<ResourceEditViewModel>();
            result.Model.ShouldBeOfType<ResourceEditViewModel>().CampaignId.ShouldBe(campaignSummaryViewModel.Id);
            result.Model.ShouldBeOfType<ResourceEditViewModel>().Id.ShouldBe(resourceDetailViewModel.Id);
        }

        [Fact]
        public async Task EditGetReturnsTheCorrectView()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };
            var resourceDetailViewModel = new ResourceDetailViewModel { Id = 5 };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<ResourceDetailQuery>())).ReturnsAsync(resourceDetailViewModel);
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);

            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());

            var result = await sut.Edit(It.IsAny<int>()) as ViewResult;

            result.ShouldNotBeNull();
            result.ViewName.ShouldBe(nameof(ResourceController.Edit));
        }


        [Fact]
        public void EditGetHasRouteAttributeWithCorrectRoute()
        {
            var sut = ResourceControllerWithNoInjectedDependencies();
            var routeAttribute = sut.GetAttributesOn(x => x.Edit(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal("Admin/Resource/Edit/{id}", routeAttribute.Template);
        }

        [Fact]
        public async Task EditPostReturns_WhenModelStateIsNotValid()
        {
            var mockMediator = new Mock<IMediator>();
            var sut = new ResourceController(mockMediator.Object);
            sut.ModelState.AddModelError("Name", "Name is required.");

            var result = await sut.Edit(new ResourceEditViewModel()) as ViewResult;

            result.ShouldNotBeNull();
            result.Model.ShouldBeOfType<ResourceEditViewModel>();
            result.ViewName.ShouldBeNull();

        }

        [Fact]
        public async Task EditPostReturnsUnauthorized_WhenCampaignIsNull()
        {
            var mockMediator = new Mock<IMediator>();

            var sut = new ResourceController(mockMediator.Object);

            var result = await sut.Edit(new ResourceEditViewModel()) as UnauthorizedResult;

            result.ShouldNotBeNull();
            result.ShouldBeOfType<UnauthorizedResult>();
            result.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task EditPostReturnsUnauthorized_WhenUserIsNotOrganizationAdmin()
        {
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 4, OrganizationId = 1, Name = "TestCampaign" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);

            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserNotAnOrgAdmin();

            var result = await sut.Edit(new ResourceEditViewModel()) as UnauthorizedResult;

            result.ShouldNotBeNull();
            result.ShouldBeOfType<UnauthorizedResult>();
            result.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task EditPostSendsEditResourceCommandWithCorrectResource()
        {
            var mockMediator = new Mock<IMediator>();
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 2, OrganizationId = 5, Name = "OrgName" };
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CreateOrEditResourceCommand>())).ReturnsAsync(It.IsAny<int>());
            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());
            var resourceEditViewModel = new ResourceEditViewModel();

            var result = await sut.Edit(resourceEditViewModel) as RedirectToActionResult;

            result.ShouldNotBeNull();
            mockMediator.Verify(m => m.SendAsync(It.Is<CreateOrEditResourceCommand>(e => e.Resource == resourceEditViewModel)), Times.Once);
        }

        [Fact]
        public async Task EditPostSendsCampaignSummaryQueryWithCorrectCampaignId()
        {
            var mockMediator = new Mock<IMediator>();
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 2, OrganizationId = 5, Name = "OrgName" };
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CreateOrEditResourceCommand>())).ReturnsAsync(2);
            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());
            var resourceEditViewModel = new ResourceEditViewModel { Id = 1, CampaignId = 2 };

            var result = await sut.Edit(resourceEditViewModel) as RedirectToActionResult;

            result.ShouldNotBeNull();
            mockMediator.Verify(m => m.SendAsync(It.Is<CampaignSummaryQuery>(e => e.CampaignId == resourceEditViewModel.CampaignId)), Times.Once);
        }

        [Fact]
        public async Task EditPostRedirectToAction_WithCorrect_Contoller_ActionAndResourceId()
        {
            const int resourceId = 2;

            var mockMediator = new Mock<IMediator>();
            var campaignSummaryViewModel = new CampaignSummaryViewModel { Id = 2, OrganizationId = 5, Name = "OrgName" };
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CampaignSummaryQuery>())).ReturnsAsync(campaignSummaryViewModel);
            mockMediator.Setup(m => m.SendAsync(It.IsAny<CreateOrEditResourceCommand>())).ReturnsAsync(resourceId);
            var sut = new ResourceController(mockMediator.Object);
            sut.MakeUserAnOrgAdmin(campaignSummaryViewModel.OrganizationId.ToString());
            var resourceEditViewModel = new ResourceEditViewModel { Id = 1, CampaignId = 2 };

            var result = await sut.Edit(resourceEditViewModel) as RedirectToActionResult;

            result.ShouldNotBeNull();
            result.ActionName.ShouldBe(nameof(ResourceController.Details));
            result.ControllerName.ShouldBe(nameof(Resource));
            result.RouteValues["resourceId"].ShouldBe(resourceId);
        }
    }
}
