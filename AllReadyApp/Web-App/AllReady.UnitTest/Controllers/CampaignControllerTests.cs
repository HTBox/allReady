using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Features.Campaigns;
using AllReady.Models;
using AllReady.Security;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels.Campaign;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using CampaignController = AllReady.Controllers.CampaignController;
using Shouldly;

namespace AllReady.UnitTest.Controllers
{
    public class CampaignControllerTests
    {
        [Fact]
        public async Task IndexSendsCampaignIndexQuery()
        {
            var mockMediator = new Mock<IMediator>();
            var authorizationServiceMock = new Mock<IUserAuthorizationService>();
            authorizationServiceMock.Setup(a => a.IsCampaignManager()).ReturnsAsync(false);

            var sut = new CampaignController(mockMediator.Object, authorizationServiceMock.Object, null);
            await sut.Index();

            mockMediator.Verify(m => m.SendAsync(It.IsAny<UnlockedCampaignsQuery>()), Times.Once);
        }

        [Fact]
        public async Task IndexReturnsAView()
        {
            var authorizationServiceMock = new Mock<IUserAuthorizationService>();
            authorizationServiceMock.Setup(a => a.IsCampaignManager()).ReturnsAsync(false);

            var sut = new CampaignController(Mock.Of<IMediator>(), authorizationServiceMock.Object, null);
            var result = await sut.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task DetailsSendsCampaignByCampaignIdQueryWithCorrectCampaignId()
        {
            const int campaignId = 1;
            var mockedMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockedMediator.Object, null, null);
            await sut.Details(campaignId);

            mockedMediator.Verify(m => m.SendAsync(It.Is<CampaignByCampaignIdQuery>(q => q.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundWhenCampaignIsNull()
        {
            var sut = new CampaignController(Mock.Of<IMediator>(), null, null);
            var result = await sut.Details(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundWhenCampaignIsLocked()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign { Locked = true });

            var sut = new CampaignController(mockedMediator.Object, null, null);
            var result = await sut.Details(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsSetsViewBagAbsoluteUrlToUrlEncodedUrlAction()
        {
            const string urlEncodedValue = "urlEncodedValue";

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign());

            var sut = new CampaignControllerForDetailsActionMethod(mockMediator.Object, urlEncodedValue)
            {
                Url = new Mock<IUrlHelper>().Object
            };
            sut.SetDefaultHttpContext();

            await sut.Details(It.IsAny<int>());

            Assert.Equal(urlEncodedValue, sut.ViewBag.AbsoluteUrl);
        }

        [Fact]
        public async Task DetailsCallsUrlActionWithTheCorrectUrlControllerContextValues()
        {
            const string requestScheme = "requestScheme";

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign());

            var mockUrlHelper = new Mock<IUrlHelper>();

            var sut = new CampaignControllerForDetailsActionMethod(mockMediator.Object)
            {
                Url = mockUrlHelper.Object
            };
            sut.SetFakeHttpRequestSchemeTo(requestScheme);

            await sut.Details(It.IsAny<int>());

            mockUrlHelper.Verify(mock => mock.Action(It.Is<UrlActionContext>(x =>
                x.Action == "Details" &&
                x.Controller == "Campaign" &&
                x.Values == null &&
                x.Protocol == requestScheme)));
        }

        [Fact]
        public async Task DetailsReturnsTheCorrectView()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign());

            var sut = new CampaignControllerForDetailsActionMethod(mockMediator.Object) { Url = new Mock<IUrlHelper>().Object };
            sut.SetDefaultHttpContext();

            var result = await sut.Details(It.IsAny<int>()) as ViewResult;

            Assert.Equal("Details", result.ViewName);
        }

        [Fact]
        public async Task DetailsReturnsTheCorrectModel()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign());

            var sut = new CampaignControllerForDetailsActionMethod(mockMediator.Object) { Url = new Mock<IUrlHelper>().Object };
            sut.SetDefaultHttpContext();

            var result = await sut.Details(It.IsAny<int>()) as ViewResult;

            Assert.IsType<CampaignViewModel>(result.ViewData.Model);
        }

        [Fact]
        public async Task LocationMapReturnsHttpNotFoundWhenCampaignIsNull()
        {
            var sut = new CampaignController(Mock.Of<IMediator>(), null, null);
            var result = await sut.LocationMap(It.IsAny<int>());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task LocationMapReturnsTheCorrectViewAndCorrectModelWhenCampaignIsNotNull()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign { ManagingOrganization = new Organization() });

            var sut = new CampaignController(mockedMediator.Object, null, null);
            var result = await sut.LocationMap(It.IsAny<int>()) as ViewResult;

            Assert.Equal("Map", result.ViewName);
            Assert.IsType<CampaignViewModel>(result.ViewData.Model);
        }

        [Fact]
        public async Task LocationSendsCampaignByCampaignIdQueryWithTheCorrectCampaignId()
        {
            const int campaignId = 1;
            var mockedMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockedMediator.Object, null, null);
            await sut.LocationMap(campaignId);

            mockedMediator.Verify(m => m.SendAsync(It.Is<CampaignByCampaignIdQuery>(q => q.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task GetReturnsTheCorrectViewModel()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<UnlockedCampaignsQuery>())).ReturnsAsync(new List<CampaignViewModel>());

            var sut = new CampaignController(mockedMediator.Object, null, null);
            var result = await sut.Get();

            Assert.IsType<List<CampaignViewModel>>(result);
        }

        [Fact]
        public async Task GetSendsCampaignGetQuery()
        {
            var mockedMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockedMediator.Object, null, null);
            await sut.Get();

            mockedMediator.Verify(m => m.SendAsync(It.IsAny<UnlockedCampaignsQuery>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdSendsCampaignByCampaignIdQueryWithCorrectCampaignId()
        {
            const int campaignId = 1;
            var mockedMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockedMediator.Object, null, null);
            await sut.Get(campaignId);

            mockedMediator.Verify(m => m.SendAsync(It.Is<CampaignByCampaignIdQuery>(q => q.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task GetByIdReturnsHttpNotFoundWhenCampaignIsNull()
        {
            var sut = new CampaignController(Mock.Of<IMediator>(), null, null);
            var result = await sut.Get(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByIdReturnsHttpNotFoundWhenCampaignIsLocked()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign { Locked = true });

            var sut = new CampaignController(mockedMediator.Object, null, null);
            var result = await sut.Get(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByIdReturnsJsonResult()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign { ManagingOrganization = new Organization() });

            var sut = new CampaignController(mockedMediator.Object, null, null);
            var result = await sut.Get(It.IsAny<int>());

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task GetByIdReturnsCorrectViewModel()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign { ManagingOrganization = new Organization() });

            var sut = new CampaignController(mockedMediator.Object, null, null);
            var result = await sut.Get(It.IsAny<int>()) as JsonResult;

            Assert.IsType<CampaignViewModel>(result.Value);
        }

        [Fact]
        public void ControllerHasARouteAtttributeWithTheCorrectRoute()
        {
            var sut = new CampaignController(null, null, null);
            var routeAttribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal("api/[controller]", routeAttribute.Template);
        }

        [Fact]
        public void IndexHasHttpGetAttribute()
        {
            var sut = new CampaignController(null, null, null);
            var httpGetAttribute = sut.GetAttributesOn(x => x.Index()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void IndexHasRouteAttributeWithCorrectRoute()
        {
            var sut = new CampaignController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.Index()).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal("~/[controller]", routeAttribute.Template);
        }

        [Fact]
        public async Task IndexReturnsUnlockedCampaignsWithTrueValueForIsCampaignManager()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<UnlockedCampaignsQuery>())).ReturnsAsync(new List<CampaignViewModel> { new CampaignViewModel { Id = 1 } });

            var userAuthorizationMock = new Mock<IUserAuthorizationService>();
            userAuthorizationMock.Setup(u => u.IsCampaignManager()).ReturnsAsync(true);
            var sut = new CampaignController(mockedMediator.Object, userAuthorizationMock.Object, null);

            var result = await sut.Index() as ViewResult;
            var model = result.ViewData.Model as List<CampaignViewModel>;

            model.Any(m => m.IsCampaignManager).ShouldBeTrue();
        }

        [Fact]
        public void DetailsHasHttpGetAttribute()
        {
            var sut = new CampaignController(null, null, null);
            var httpGetAttribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void DetailsHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new CampaignController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal("~/[controller]/{id}", routeAttribute.Template);
        }

        [Fact]
        public void LocationMapHasHttpGetAttribute()
        {
            var sut = new CampaignController(null, null, null);
            var httpGetAttribute = sut.GetAttributesOn(x => x.LocationMap(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void LocationMapHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new CampaignController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.LocationMap(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal("~/[controller]/map/{id}", routeAttribute.Template);
        }

        [Fact]
        public void GetHasHttpGetAttributes()
        {
            var sut = new CampaignController(null, null, null);
            var httpGetAttribute = sut.GetAttributesOn(x => x.Get()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void GetWithIdHasHttpGetAttributes()
        {
            var sut = new CampaignController(null, null, null);
            var httpGetAttribute = sut.GetAttributesOn(x => x.Get(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(httpGetAttribute);
            Assert.Equal("{id}", httpGetAttribute.Template);
        }

        [Fact]
        public void ManageCampaignHasHttpGetAttribute()
        {
            var sut = new CampaignController(null, null, null);
            var httpGetAttribute = sut.GetAttributesOn(x => x.ManageCampaign()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void ManageCampaignHasTheCorrectRoute()
        {
            var sut = new CampaignController(null, null, null);
            var routeAttribute = sut.GetAttributesOn(x => x.ManageCampaign()).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal("~/[controller]/ManageCampaign", routeAttribute.Template);
        }

        [Fact]
        public async Task ManageCampaign_ReturnsUnAuthorized_IfUserIsNotCampaignManager()
        {
            var authorizationServiceMock = new Mock<IUserAuthorizationService>();
            authorizationServiceMock.Setup(a => a.IsCampaignManager()).ReturnsAsync(false);

            var sut = new CampaignController(null, authorizationServiceMock.Object, UserManagerMockHelper.CreateUserManagerMock().Object);

            var result = await sut.ManageCampaign();

            result.ShouldBeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task ManageCampaign_ReturnsTheCorrectViewModel()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<AuthorizedCampaignsQuery>())).ReturnsAsync(new List<ManageCampaignViewModel>());
            var authorizationServiceMock = new Mock<IUserAuthorizationService>();
            authorizationServiceMock.Setup(a => a.IsCampaignManager()).ReturnsAsync(true);
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser());

            var sut = new CampaignController(mockedMediator.Object, authorizationServiceMock.Object, userManager.Object);

            var result = await sut.ManageCampaign() as ViewResult;

            result.ShouldNotBeNull();
            result.ViewData.Model.ShouldBeOfType<List<ManageCampaignViewModel>>();
        }

        [Fact]
        public async Task ManageCampaign_ReturnsTheCorrectView()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<AuthorizedCampaignsQuery>())).ReturnsAsync(new List<ManageCampaignViewModel>());
            var authorizationServiceMock = new Mock<IUserAuthorizationService>();
            authorizationServiceMock.Setup(a => a.IsCampaignManager()).ReturnsAsync(true);
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser());

            var sut = new CampaignController(mockedMediator.Object, authorizationServiceMock.Object, userManager.Object);

            var result = await sut.ManageCampaign() as ViewResult;

            result.ShouldNotBeNull();
            result.ViewName.ShouldBeNull();
        }

        public class CampaignControllerForDetailsActionMethod : CampaignController
        {
            private readonly string urlEncodedValue;

            public CampaignControllerForDetailsActionMethod(IMediator mediator, string urlEncodedValue = null) : base(mediator, null, null)
            {
                this.urlEncodedValue = urlEncodedValue;
            }

            protected override string UrlEncode(string value)
            {
                return urlEncodedValue;
            }
        }
    }
}