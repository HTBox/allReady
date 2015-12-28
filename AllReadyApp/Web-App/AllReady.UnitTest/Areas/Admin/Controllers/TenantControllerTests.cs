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
    public class OrganizationControllerTests
    {
        private readonly OrganizationEditModel _stubViewModel;

        public OrganizationControllerTests()
        {
            _stubViewModel = new OrganizationEditModel
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
        public void CreateNewOrganizationRedirectsToOrganizationList()
        {
            //arrange
            var sut = new OrganizationController(MockMediatorCreateOrganization().Object);
            var expectedRouteValues = new {controller = "Organization", action = "Index"};
            //act
            var result = sut.Create(_stubViewModel);
            //assert
            Assert.IsType<RedirectToRouteResult>(result);
            Assert.Equal("areaRoute", ((RedirectToRouteResult) result).RouteName);
            Assert.Equal("Organization", ((RedirectToRouteResult)result).RouteValues["controller"]); 
            Assert.Equal("Index",((RedirectToRouteResult)result).RouteValues["action"]); 
        }

        [Fact]
        public void CreateNewOrganizationPostReturnsBadRequestForNullOrganization()
        {
            //arrange
            OrganizationEditModel viewmodel = null;
            var sut = new OrganizationController(MockMediatorCreateOrganization().Object);
            //act
            var result = sut.Create(viewmodel);
            //assert 
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CreateNewOrganizationInvalidModelReturnsCreateView()
        {
            //arrange
            var sut = new OrganizationController(MockMediatorCreateOrganization().Object);
            sut.ModelState.AddModelError("foo", "bar");
            //act
            var result = sut.Create(_stubViewModel);
            //assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", ((ViewResult) result).ViewName);
        }

        private static Mock<IMediator> MockMediatorCreateOrganization()
        {
            var mockMediator = new Mock<IMediator>();
            return mockMediator;
        }
    }
}