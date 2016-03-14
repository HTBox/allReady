using System.Collections.Generic;
using System.Linq;
using AllReady.Features.Campaigns;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;
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
        public void DetailsSendsCampaignByCampaignIdQueryWithCorrectCampaignId()
        {
            const int campaignId = 1;

            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.Send(It.Is<CampaignByCampaignIdQuery>(q => q.CampaignId == campaignId)));

            var sut = new CampaignController(mockedMediator.Object);
            sut.Details(campaignId);

            mockedMediator.Verify(m => m.Send(It.Is<CampaignByCampaignIdQuery>(q => q.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public void DetailsReturnsHttpNotFoundWhenCampaignIsNull()
        {
            var sut = new CampaignController(Mock.Of<IMediator>());
            var result = sut.Details(It.IsAny<int>());

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void DetailsReturnsHttpNotFoundWhenCampaignIsLocked()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.Send(It.IsAny<CampaignByCampaignIdQuery>())).Returns(new Campaign { Locked = true });

            var sut = new CampaignController(mockedMediator.Object);
            var result = sut.Details(It.IsAny<int>());

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void DetailsSetsViewBagAbsoluteUrlToUrlEncodedUrlAction()
        {
            const string urlEncodedValue = "urlEncodedValue";

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.IsAny<CampaignByCampaignIdQuery>())).Returns(new Campaign());

            var sut = new CampaignControllerForDetailsActionMethod(mockMediator.Object, urlEncodedValue)
            {
                Url = new Mock<IUrlHelper>().Object
            };
            sut.SetDefaultHttpContext();

            sut.Details(It.IsAny<int>());

            Assert.Equal(urlEncodedValue, sut.ViewBag.AbsoluteUrl);
        }

        [Fact]
        public void DetailsCallsUrlActionWithTheCorrectUrlActionContextValues()
        {
            const string requestScheme = "requestScheme";

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.IsAny<CampaignByCampaignIdQuery>())).Returns(new Campaign());

            var mockUrlHelper = new Mock<IUrlHelper>();

            var sut = new CampaignControllerForDetailsActionMethod(mockMediator.Object)
            {
                Url = mockUrlHelper.Object
            };
            sut.SetFakeHttpContext().SetFakeHttpRequestSchemeTo(requestScheme);

            sut.Details(It.IsAny<int>());

            mockUrlHelper.Verify(mock => mock.Action(It.Is<UrlActionContext>(x =>
                x.Action == "Details" &&
                x.Controller == "Campaign" &&
                x.Values == null &&
                x.Protocol == requestScheme)));
        }

        [Fact]
        public void DetailsReturnsTheCorrectView()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.IsAny<CampaignByCampaignIdQuery>())).Returns(new Campaign());

            var sut = new CampaignControllerForDetailsActionMethod(mockMediator.Object) { Url = new Mock<IUrlHelper>().Object };
            sut.SetDefaultHttpContext();

            var result = (ViewResult)sut.Details(It.IsAny<int>());

            Assert.Equal("Details", result.ViewName);
        }

        [Fact]
        public void DetailsReturnsTheCorrectModel()
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.Send(It.IsAny<CampaignByCampaignIdQuery>())).Returns(new Campaign());

            var sut = new CampaignControllerForDetailsActionMethod(mockMediator.Object) { Url = new Mock<IUrlHelper>().Object };
            sut.SetDefaultHttpContext();

            var result = (ViewResult)sut.Details(It.IsAny<int>());

            Assert.IsType<CampaignViewModel>(result.ViewData.Model);
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

        [Fact]
        public void LocationMapReturnsHttpNotFoundWhenCampaignIsNull()
        {
            var sut = new CampaignController(Mock.Of<IMediator>());
            var result = sut.LocationMap(It.IsAny<int>());
            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void LocationMapReturnsTheCorrectViewAndCorrectModelWhenCampaignIsNotNull()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.Send(It.IsAny<CampaignByCampaignIdQuery>())).Returns(new Campaign() { ManagingOrganization = new Organization() });

            var sut = new CampaignController(mockedMediator.Object);
            var result = (ViewResult)sut.LocationMap(It.IsAny<int>());

            Assert.Equal("Map", result.ViewName);
            Assert.IsType<CampaignViewModel>(result.ViewData.Model);
        }

        [Fact]
        public void LocationSendsCampaignByCampaignIdQueryWithTheCorrectCampaignId()
        {
            const int campaignId = 1;
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.Send(It.Is<CampaignByCampaignIdQuery>(q => q.CampaignId == campaignId)));

            var sut = new CampaignController(mockedMediator.Object);
            sut.LocationMap(campaignId);

            mockedMediator.Verify(m => m.Send(It.Is<CampaignByCampaignIdQuery>(q => q.CampaignId == campaignId)), Times.Once);
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
        public void GetByIdSendsCampaignByCampaignIdQueryWithCorrectCampaignId()
        {
            const int campaignId = 1;
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.Send(It.Is<CampaignByCampaignIdQuery>(q => q.CampaignId == campaignId)));

            var sut = new CampaignController(mockedMediator.Object);
            sut.Get(campaignId);

            mockedMediator.Verify(m => m.Send(It.Is<CampaignByCampaignIdQuery>(q => q.CampaignId == campaignId)), Times.Once);
        }

        [Fact]
        public void GetByIdReturnsHttpNotFoundWhenCampaignIsNull()
        {
            var sut = new CampaignController(Mock.Of<IMediator>());
            var result = sut.Get(It.IsAny<int>());

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void GetByIdReturnsHttpNotFoundWhenCampaignIsLocked()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.Send(It.IsAny<CampaignByCampaignIdQuery>())).Returns(new Campaign { Locked = true });

            var sut = new CampaignController(mockedMediator.Object);
            var result = sut.Get(It.IsAny<int>());

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void GetByIdReturnsJsonResult()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.Send(It.IsAny<CampaignByCampaignIdQuery>())).Returns(new Campaign() { ManagingOrganization = new Organization() });

            var sut = new CampaignController(mockedMediator.Object);
            var result = sut.Get(It.IsAny<int>());

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public void GetByIdReturnsCorrectViewModel()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.Send(It.IsAny<CampaignByCampaignIdQuery>())).Returns(new Campaign() { ManagingOrganization = new Organization() });

            var sut = new CampaignController(mockedMediator.Object);
            var result = (JsonResult)sut.Get(It.IsAny<int>());

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
    }
}
