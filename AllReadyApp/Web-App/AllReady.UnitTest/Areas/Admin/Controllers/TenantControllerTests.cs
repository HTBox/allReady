using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class TenantControllerTests
    {
        private readonly TenantEditModel _stubViewModel;

        public TenantControllerTests()
        {
            _stubViewModel = new TenantEditModel
            {
                Id = 0,
                LogoUrl = "http://www.example.com/image.jpg",
                Name = "Test",
                PrimaryContactFirstName = "FirstName",
                PrimaryContactLastName = "LastName",
                PrimaryContactPhoneNumber = "0123456798",
                PrimaryContactEmail = "test@test.com",
                WebUrl = "http://www.example.com"
            };
        }

        [Fact]
        public void CreateNewTenantRedirectsToTenantList()
        {
            //arrange
            var sut = new TenantController(MockMediatorCreateTenant().Object);
            var expectedRouteValues = new {controller = "Tenant", action = "Index"};
            //act
            var result = sut.Create(_stubViewModel);
            //assert
            Assert.IsType<RedirectToRouteResult>(result);
            Assert.Equal("areaRoute", ((RedirectToRouteResult) result).RouteName);
            Assert.Equal("Tenant",((RedirectToRouteResult)result).RouteValues["controller"]); 
            Assert.Equal("Index",((RedirectToRouteResult)result).RouteValues["action"]); 
        }

        [Fact]
        public void CreateNewTenantPostReturnsBadRequestForNullTenant()
        {
            //arrange
            TenantEditModel viewmodel = null;
            var sut = new TenantController(MockMediatorCreateTenant().Object);
            //act
            var result = sut.Create(viewmodel);
            //assert 
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CreateNewTenantInvalidModelReturnsCreateView()
        {
            //arrange
            var sut = new TenantController(MockMediatorCreateTenant().Object);
            sut.ModelState.AddModelError("foo", "bar");
            //act
            var result = sut.Create(_stubViewModel);
            //assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", ((ViewResult) result).ViewName);
        }

        private static Mock<IMediator> MockMediatorCreateTenant()
        {
            var mockMediator = new Mock<IMediator>();
            return mockMediator;
        }
    }
}