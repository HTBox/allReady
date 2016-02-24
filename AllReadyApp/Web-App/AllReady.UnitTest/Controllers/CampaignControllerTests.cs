using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Mvc;
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

        [Fact]
        public void IndexHasHttpGetAttribute()
        {
            var sut = new CampaignController(null);
            var httpGetAttribute = sut.GetAttributesOn(x => x.Index()).OfType<HttpGetAttribute>().SingleOrDefault();
            Assert.NotNull(httpGetAttribute);
            //var attribute = sut.GetMethodAttributesOn(x => x.Index()).OfType<HttpGetAttribute>().SingleOrDefault();
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
        public void ControllerHasARouteAtttributeWithTheCorrectRoute()
        {
            var sut = new CampaignController(null);

            var hasAttribute = sut.HasAttribute(typeof(RouteAttribute));
            Assert.True(hasAttribute);

            var hasAttributeWithValue = sut.HasAttributeWithValue(typeof (RouteAttribute), "api/[controller]");
            Assert.True(hasAttributeWithValue);
        }
    }
}
