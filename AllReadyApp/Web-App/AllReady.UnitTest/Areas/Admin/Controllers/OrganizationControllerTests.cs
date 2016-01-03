using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNet.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class OrganizationControllerTests
    {
        private readonly OrganizationEditModel _organizationEditModel;
        private static Mock<IMediator> _mockMediator;
        private static OrganizationController _sut;

        public OrganizationControllerTests()
        {
            _organizationEditModel = new OrganizationEditModel
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
            CreateSut();

            var expectedRouteValues = new { controller = "Organization", action = "Index" };

            var result = _sut.Create(_organizationEditModel);

            Assert.IsType<RedirectToRouteResult>(result);
            Assert.Equal("areaRoute", ((RedirectToRouteResult)result).RouteName);
            Assert.Equal("Organization", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.Equal("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        [Fact]
        public void BusShouldBeCalledWithAppropriateData()
        {
            CreateSut();

            _sut.Create(_organizationEditModel);

            _mockMediator.Verify(x => x.Send(It.Is<OrganizationEditCommand>( y => y.Organization == _organizationEditModel)));
        }

        [Fact]
        public void CreateNewOrganizationPostReturnsBadRequestForNullOrganization()
        {
            OrganizationEditModel viewmodel = null;
            CreateSut();

            var result = _sut.Create(viewmodel);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CreateNewOrganizationInvalidModelReturnsCreateView()
        {
            CreateSut();
            _sut.ModelState.AddModelError("foo", "bar");

            var result = _sut.Create(_organizationEditModel);

            Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", ((ViewResult) result).ViewName);
        }

        private static void CreateSut()
        {
            _mockMediator = new Mock<IMediator>();

            _sut = new OrganizationController(_mockMediator.Object);
        }
    }
}