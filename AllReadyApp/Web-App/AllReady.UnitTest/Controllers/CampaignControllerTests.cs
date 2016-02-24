using System.ComponentModel.DataAnnotations;
using System.Linq;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using AllReady.ViewModels;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;
using CampaignController = AllReady.Controllers.CampaignController;

namespace AllReady.UnitTest.Controllers
{
    public class CampaignControllerTests
    {
        //TODO: REMOVE THIS BEFORE COMMITING
        [Fact]
        public void FooHasKeyAttribute()
        {
            var sut = new CampaignController(null);
            var keyAttribute = sut.GetAttributesOn(x => x.Foo).OfType<KeyAttribute>().SingleOrDefault();
            Assert.NotNull(keyAttribute);
        }

        //need a way to unit test System.Net.WebUtility.UrlEncode and Url.Action
        [Fact]
        public void DetailsTests()
        {
        }

        [Fact]
        public void LocationMapReturnsHttpNotFoundWhenCampaignIsNull()
        {
            var sut = new CampaignController(new Mock<IAllReadyDataAccess>().Object);
            var result = sut.LocationMap(It.IsAny<int>());
            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void LocationMapReturnsTheCorrectViewWhenCampaignIsNotNull()
        {
            const int campaignId = 1;
            var mockedDataAccess = new Mock<IAllReadyDataAccess>();
            mockedDataAccess.Setup(x => x.GetCampaign(campaignId)).Returns(new Campaign());

            var sut = new CampaignController(mockedDataAccess.Object);
            var result = (ViewResult)sut.LocationMap(campaignId);

            Assert.Equal("Map", result.ViewName);
        }

        [Fact]
        public void LocationMapReturnsTheCorrectModelWhenCampaignIsNotNull()
        {
            const int campaignId = 1;
            var mockedDataAccess = new Mock<IAllReadyDataAccess>();
            mockedDataAccess.Setup(x => x.GetCampaign(campaignId)).Returns(new Campaign());

            var sut = new CampaignController(mockedDataAccess.Object);
            var result = (ViewResult)sut.LocationMap(campaignId);

            Assert.IsType<CampaignViewModel>(result.ViewData.Model);
        }

        [Fact]
        public void LocationMapCallsGetCampaignWithTheCorrectCampaignId()
        {
            const int campaignId = 1;
            var mockedDataAccess = new Mock<IAllReadyDataAccess>();
            mockedDataAccess.Setup(x => x.GetCampaign(campaignId)).Returns(new Campaign());

            var sut = new CampaignController(mockedDataAccess.Object);
            sut.LocationMap(campaignId);

            mockedDataAccess.Verify(x => x.GetCampaign(campaignId), Times.Once);
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
