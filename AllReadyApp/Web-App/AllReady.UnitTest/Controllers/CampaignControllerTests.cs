using System.Linq;
using AllReady.Features.Campaigns;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
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

            mockMediator.Verify(m => m.Send(It.IsAny<CampaignIndexQuery>()), Times.Once);
        }

        [Fact]
        public void IndexReturnsAView()
        {
            var sut = new CampaignController(new Mock<IMediator>().Object);
            var result = sut.Index();

            Assert.IsType<ViewResult>(result);
        }

        //need a way to unit test System.Net.WebUtility.UrlEncode and Url.Action
        [Fact]
        public void DetailsTests()
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
            mockedMediator.Setup(m => m.Send(It.IsAny<CampaginByCampaignIdQuery>())).Returns(new Campaign());

            var sut = new CampaignController(mockedMediator.Object);
            var result = (ViewResult)sut.LocationMap(It.IsAny<int>());

            Assert.Equal("Map", result.ViewName);
            Assert.IsType<CampaignViewModel>(result.ViewData.Model);
        }

        [Fact]
        public void LocationSendsCampaginByCampaignIdQueryWithTheCorrectCampaignId()
        {
            const int campaignId = 1;
            var mockedMediator = new Mock<IMediator>();
            mockedMediator
                .Setup(m => m.Send(It.Is<CampaginByCampaignIdQuery>(q => q.CampaignId == campaignId)))
                .Returns(new Campaign { Id = campaignId });

            var sut = new CampaignController(mockedMediator.Object);
            sut.LocationMap(campaignId);

            mockedMediator.Verify(m => m.Send(It.Is<CampaginByCampaignIdQuery>(q => q.CampaignId == campaignId)), Times.Once);
        }

        //TODO: start here Get()

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
