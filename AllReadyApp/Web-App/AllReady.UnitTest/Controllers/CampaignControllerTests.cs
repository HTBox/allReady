using System.Collections.Generic;
using System.Linq;
using AllReady.Extensions;
using AllReady.Features.Campaigns;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNet.Mvc;
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
            var sut = new CampaignController(new Mock<IMediator>().Object);
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
            var sut = new CampaignController(new Mock<IMediator>().Object);
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

        //TODO: figure out how to test Url.Action and System.Net.WebUtility.UrlEncode
        [Fact]
        public void DetailsReturnsTheCorrectModel()
        {
        }

        //TODO: figure out how to test Url.Action and System.Net.WebUtility.UrlEncode
        [Fact]
        public void DetailsReturnsTheCorrectView()
        {
        }

        [Fact]
        public void LocationMapReturnsHttpNotFoundWhenCampaignIsNull()
        {
            var sut = new CampaignController(new Mock<IMediator>().Object);
            var result = sut.LocationMap(It.IsAny<int>());
            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void LocationMapReturnsTheCorrectViewAndCorrectModelWhenCampaignIsNotNull()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.Send(It.IsAny<CampaignByCampaignIdQuery>())).Returns(new Campaign());

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
            var sut = new CampaignController(new Mock<IMediator>().Object);
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
            mockedMediator.Setup(m => m.Send(It.IsAny<CampaignByCampaignIdQuery>())).Returns(new Campaign());

            var sut = new CampaignController(mockedMediator.Object);
            var result = sut.Get(It.IsAny<int>());

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public void GetByIdReturnsCorrectViewModel()
        {
            var mockedMediator = new Mock<IMediator>();
            mockedMediator.Setup(m => m.Send(It.IsAny<CampaignByCampaignIdQuery>())).Returns(new Campaign());

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
