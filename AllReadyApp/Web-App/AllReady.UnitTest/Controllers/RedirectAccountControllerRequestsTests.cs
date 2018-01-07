using AllReady.Areas.Admin.Controllers;
using AllReady.Constants;
using AllReady.Controllers;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class RedirectAccountControllerRequestsTests
    {
        [Fact]
        public void InvokeIsLocalUrlWithCorrectReturnUrl()
        {
            const string returnUrl = "ReturnUrl";
            var urlHelper = new Mock<IUrlHelper>();

            var sut = new RedirectAccountControllerRequests(urlHelper.Object);
            sut.RedirectToLocal(returnUrl, null);

            urlHelper.Verify(x => x.IsLocalUrl(returnUrl), Times.Once);
        }

        [Fact]
        public void RedirectToReturnUrl_WhenReturnUrlIsALocalUrl()
        {
            const string returnUrl = "ReturnUrl";

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(true);

            var sut = new RedirectAccountControllerRequests(urlHelper.Object);
            var result = sut.RedirectToLocal(returnUrl, null) as RedirectResult;

            Assert.Equal(result.Url, returnUrl);
        }

        [Fact]
        public void RedirectToCorrectActionResultWithCorrectRouteValues_WhenReturnUrlIsNotALocalUrl_AndUserIsSiteAdmin()
        {
            var applicationUser = new ApplicationUser();
            applicationUser.MakeSiteAdmin();

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(false);

            var routeValueDictionary = new RouteValueDictionary
            {
                ["area"] = AreaNames.Admin
            };

            var sut = new RedirectAccountControllerRequests(urlHelper.Object);
            var result = sut.RedirectToLocal(It.IsAny<string>(), applicationUser) as RedirectToActionResult;

            Assert.Equal(nameof(SiteController.Index), result.ActionName);
            Assert.Equal("Site", result.ControllerName);
            Assert.Equal(result.RouteValues, routeValueDictionary);
        }

        [Fact]
        public void RedirectToCorrectActionResultWithCorrectRouteValues_WhenReturnUrlIsNotALocalUrl_AndUserIsOrgAdmin()
        {
            var applicationUser = new ApplicationUser();
            applicationUser.MakeOrgAdmin();

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(false);

            var routeValueDictionary = new RouteValueDictionary
            {
                ["area"] = AreaNames.Admin
            };

            var sut = new RedirectAccountControllerRequests(urlHelper.Object);
            var result = sut.RedirectToLocal(It.IsAny<string>(), applicationUser) as RedirectToActionResult;

            Assert.Equal(nameof(AllReady.Areas.Admin.Controllers.CampaignController.Index), result.ActionName);
            Assert.Equal("Campaign", result.ControllerName);
            Assert.Equal(result.RouteValues, routeValueDictionary);
        }

        [Fact]
        public void RedirectToCorrectActionResultWithCorrectRouteValues_WhenReturnUrlIsNotALocalUrl_AndUserIsNotASiteOrAnOrgAdmin()
        {
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(false);

            var sut = new RedirectAccountControllerRequests(urlHelper.Object);
            var result = sut.RedirectToLocal(It.IsAny<string>(), new ApplicationUser()) as RedirectToPageResult;

            Assert.Equal(result.PageName, "/Index");
        }
    }
}
