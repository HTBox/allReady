using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Campaigns;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels.Campaign;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using Xunit;
using CampaignController = AllReady.Controllers.CampaignController;

namespace AllReady.UnitTest.Controllers
{
    public class CampaignControllerTests
    {
        [Fact]
        public void IndexSendsCampaignIndexQuery()
        {
            var mockMediator = new Mock<IMediator>();
            var sut = new CampaignController(mockMediator.Object);
            sut.Index();

            mockMediator.Verify(m => m.Send(It.IsAny<UnlockedCampaignsQuery>()), Times.Once);
        }

        [Fact]
        public void IndexReturnsAView()
        {
            var sut = new CampaignController(Mock.Of<IMediator>());
            var result = sut.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task DetailsSendsCampaignByCampaignIdQueryWithCorrectCampaignId()
        {
            const int campaignId = 1;
            var mockedMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockedMediator.Object);
            await sut.Details(campaignId);

            mockedMediator.Verify(m => m.SendAsync(It.Is<CampaignByCampaignIdQuery>(q => q.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundWhenCampaignIsNull()
        {
            var sut = new CampaignController(Mock.Of<IMediator>());
            var result = await sut.Details(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsReturnsHttpNotFoundWhenCampaignIsLocked()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign { Locked = true });

            var sut = new CampaignController(mockedMediator.Object);
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
            var sut = new CampaignController(Mock.Of<IMediator>());
            var result = await sut.LocationMap(It.IsAny<int>());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task LocationMapReturnsTheCorrectViewAndCorrectModelWhenCampaignIsNotNull()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign { ManagingOrganization = new Organization() });

            var sut = new CampaignController(mockedMediator.Object);
            var result = await sut.LocationMap(It.IsAny<int>()) as ViewResult;

            Assert.Equal("Map", result.ViewName);
            Assert.IsType<CampaignViewModel>(result.ViewData.Model);
        }

        [Fact]
        public async Task LocationSendsCampaignByCampaignIdQueryWithTheCorrectCampaignId()
        {
            const int campaignId = 1;
            var mockedMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockedMediator.Object);
            await sut.LocationMap(campaignId);

            mockedMediator.Verify(m => m.SendAsync(It.Is<CampaignByCampaignIdQuery>(q => q.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public void GetReturnsTheCorrectViewModel()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.Send(It.IsAny<UnlockedCampaignsQuery>())).Returns(new List<CampaignViewModel>());

            var sut = new CampaignController(mockedMediator.Object);
            var result = sut.Get();

            Assert.IsType<List<CampaignViewModel>>(result);
        }

        [Fact]
        public void GetSendsCampaignGetQuery()
        {
            var mockedMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockedMediator.Object);
            sut.Get();

            mockedMediator.Verify(m => m.Send(It.IsAny<UnlockedCampaignsQuery>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdSendsCampaignByCampaignIdQueryWithCorrectCampaignId()
        {
            const int campaignId = 1;
            var mockedMediator = new Mock<IMediator>();

            var sut = new CampaignController(mockedMediator.Object);
            await sut.Get(campaignId);

            mockedMediator.Verify(m => m.SendAsync(It.Is<CampaignByCampaignIdQuery>(q => q.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public async Task GetByIdReturnsHttpNotFoundWhenCampaignIsNull()
        {
            var sut = new CampaignController(Mock.Of<IMediator>());
            var result = await sut.Get(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByIdReturnsHttpNotFoundWhenCampaignIsLocked()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign { Locked = true });

            var sut = new CampaignController(mockedMediator.Object);
            var result = await sut.Get(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByIdReturnsJsonResult()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign { ManagingOrganization = new Organization() });

            var sut = new CampaignController(mockedMediator.Object);
            var result = await sut.Get(It.IsAny<int>());

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task GetByIdReturnsCorrectViewModel()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign { ManagingOrganization = new Organization() });

            var sut = new CampaignController(mockedMediator.Object);
            var result = await sut.Get(It.IsAny<int>()) as JsonResult;

            Assert.IsType<CampaignViewModel>(result.Value);
        }

        [Fact]
        public void ControllerHasARouteAtttributeWithTheCorrectRoute()
        {
            var sut = new CampaignController(null);
            var routeAttribute = sut.GetAttributes().OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "api/[controller]");
        }

        [Fact]
        public void IndexHasHttpGetAttribute()
        {
            var sut = new CampaignController(null);
            var httpGetAttribute = sut.GetAttributesOn(x => x.Index()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void IndexHasRouteAttributeWithCorrectRoute()
        {
            var sut = new CampaignController(null);
            var routeAttribute = sut.GetAttributesOn(x => x.Index()).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "~/[controller]");
        }

        [Fact]
        public void DetailsHasHttpGetAttribute()
        {
            var sut = new CampaignController(null);
            var httpGetAttribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void DetailsHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new CampaignController(null);
            var routeAttribute = sut.GetAttributesOn(x => x.Details(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "~/[controller]/{id}");
        }

        [Fact]
        public void LocationMapHasHttpGetAttribute()
        {
            var sut = new CampaignController(null);
            var httpGetAttribute = sut.GetAttributesOn(x => x.LocationMap(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void LocationMapHasRouteAttributeWithCorrectTemplate()
        {
            var sut = new CampaignController(null);
            var routeAttribute = sut.GetAttributesOn(x => x.LocationMap(It.IsAny<int>())).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "~/[controller]/map/{id}");
        }

        [Fact]
        public void GetHasHttpGetAttributes()
        {
            var sut = new CampaignController(null);
            var httpGetAttribute = sut.GetAttributesOn(x => x.Get()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void GetWithIdHasHttpGetAttributes()
        {
            var sut = new CampaignController(null);
            var httpGetAttribute = sut.GetAttributesOn(x => x.Get(It.IsAny<int>())).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(httpGetAttribute);
            Assert.Equal(httpGetAttribute.Template, "{id}");
        }

        public class CampaignControllerForDetailsActionMethod : CampaignController
        {
            private readonly string urlEncodedValue;

            public CampaignControllerForDetailsActionMethod(IMediator mediator, string urlEncodedValue = null) : base(mediator)
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