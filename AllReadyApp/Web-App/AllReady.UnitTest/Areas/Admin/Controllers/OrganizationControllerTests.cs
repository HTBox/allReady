using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNet.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class OrganizationControllerTests
    {
        private readonly OrganizationEditModel _organizationEditModel;

        private static Mock<IMediator> _bus;

        private static OrganizationController _sut;

        private const int Id = 4565;

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

        #region IndexTests

        [Fact]
        public void IndexShouldReturnTheAViewWithTheListReturnedFromTheBus()
        {
            CreateSut();

            var organizationSummaryModel = new List<OrganizationSummaryModel> { new OrganizationSummaryModel() };

            _bus.Setup(x => x.Send(It.IsAny<OrganizationListQuery>())).Returns(organizationSummaryModel);

            var result = (ViewResult)_sut.Index();

            Assert.Same(organizationSummaryModel, result.ViewData.Model);
        }

        #endregion

        #region DetailsTests

        [Fact]
        public void DetailsShouldPassTheIdToTheBus()
        {
            CreateSut();

            _sut.Details(Id);

            _bus.Verify(x => x.Send(It.Is<OrganizationDetailQuery>(y => y.Id == Id)));
        }

        [Fact]
        public void WhenTheBusReturnsNullForThatIdHttpNotFoundShouldBeReturned()
        {
            CreateSut();

            _bus.Setup(x => x.Send(It.Is<OrganizationDetailQuery>(y => y.Id == Id))).Returns<OrganizationDetailModel>(null);

            var result = _sut.Details(Id);

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void WhenTheBusReturnsAOrganizationDetailModelForThatIdAViewShouldBeReturned()
        {
            CreateSut();

            var model = new OrganizationDetailModel();

            _bus.Setup(x => x.Send(It.Is<OrganizationDetailQuery>(y => y.Id == Id))).Returns(model);

            var result = (ViewResult)_sut.Details(Id);

            Assert.Same(model, result.ViewData.Model);
        }


        #endregion

        #region CreateTests

        [Fact]
        public void CreateWithoutParametersShouldReturnAView()
        {
            CreateSut();

            var result = _sut.Create();

            Assert.IsType<ViewResult>(result);
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
        public void CreateNewOrganizationMediatorShouldBeCalledWithAppropriateDataWhenModelStateIsValid()
        {
            CreateSut();

            _sut.Create(_organizationEditModel);

            _bus.Verify(x => x.Send(It.Is<OrganizationEditCommand>( y => y.Organization == _organizationEditModel)));
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

        #endregion
        
        private static void CreateSut()
        {
            _bus = new Mock<IMediator>();

            _sut = new OrganizationController(_bus.Object);
        }
    }
}