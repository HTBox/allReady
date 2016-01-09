using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        #region ControllerAttributeTests

        [Fact]
        public void TheControllerShouldHaveAnAreaAttributeOfAdmin()
        {
            CreateSut();

            var type = _sut.GetType();

            var areaAttribute = type.CustomAttributes.First(x => x.AttributeType == typeof(AreaAttribute));

            var constructorArg = areaAttribute.ConstructorArguments.First().Value as string;
                        
            Assert.Equal("Admin", constructorArg);
        }

        [Fact]
        public void TheControllerShouldHaveAnAuthorizeAttributeOfSiteAdmin()
        {
            CreateSut();

            var type = _sut.GetType();

            var areaAttribute = type.CustomAttributes.First(x => x.AttributeType == typeof(AuthorizeAttribute));

            var constructorArg = areaAttribute.ConstructorArguments.First().Value as string;

            Assert.Equal("SiteAdmin", constructorArg);
        }


        #endregion

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
        public void CreateGetShouldReturnAView()
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

            AddErrorToModelState();

            var result = _sut.Create(_organizationEditModel);

            Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", ((ViewResult) result).ViewName);
        }

        [Fact]
        public void CreateNewOrganizationPostShouldHaveValidateAntiForgeryTokenAttribute()
        {
            ShouldHaveValidateAntiForgeryTokenAttribute("Create", typeof(OrganizationEditModel));
        }

        [Fact]
        public void CreateNewOrganizationPostShouldHavePostAttribute()
        {
            ShouldHaveHttpPostAttribute("Create", typeof(OrganizationEditModel));
        }

        #endregion

        #region EditTests

        [Fact]
        public void EditGetShouldReturnHttpNotFoundWhenBusReturnsNullForId()
        {
            CreateSut();

            _bus.Setup(x => x.Send(It.Is<OrganizationEditQuery>(y => y.Id == Id))).Returns<OrganizationEditModel>(null);

            var result = _sut.Edit(Id);

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void EditGetShouldReturnEditViewWhenBusReturnsOrganizationEditModel()
        {
            CreateSut();

            var model = new OrganizationEditModel();

            _bus.Setup(x => x.Send(It.Is<OrganizationEditQuery>(y => y.Id == Id))).Returns(model);

            var result = (ViewResult)_sut.Edit(Id);

            Assert.Equal("Edit", result.ViewName);
            Assert.Same(model, result.ViewData.Model);
        }

        [Fact]
        public void EditPostShouldHaveValidateAntiForgeryTokenAttribute()
        {
            ShouldHaveValidateAntiForgeryTokenAttribute("Edit", typeof(OrganizationEditModel));
        }

        [Fact]
        public void EditPostShouldHaveHttpPostAttribute()
        {
            ShouldHaveHttpPostAttribute("Edit", typeof(OrganizationEditModel));
        }

        [Fact]
        public void EditPostShouldReturnTheEditViewWithTheModelPassedInIfTheModelStateIsInvalid()
        {
            CreateSut();
            AddErrorToModelState();

            var model = new OrganizationEditModel();

            var result = (ViewResult)_sut.Edit(model);

            Assert.Equal("Edit", result.ViewName);
            Assert.Same(model, result.ViewData.Model);
        }

        [Fact]
        public void EditPostShouldRedirectToDetailsWithTheIdFromTheBusIfModelStateIsValid()
        {
            CreateSut();

            var model = new OrganizationEditModel();

            _bus.Setup(x => x.Send(It.Is<OrganizationEditCommand>(y => y.Organization == model))).Returns(Id);

            var result = (RedirectToActionResult)_sut.Edit(model);

            Assert.Equal("Details", result.ActionName);
            Assert.Equal("Admin", result.RouteValues["area"]);
            Assert.Equal(Id, result.RouteValues["id"]);
        }

        #endregion

        private static void CreateSut()
        {
            _bus = new Mock<IMediator>();

            _sut = new OrganizationController(_bus.Object);
        }

        private static void ShouldHaveValidateAntiForgeryTokenAttribute(string methodName, Type paramTypePassedToMethod)
        {
            var method = GetMethodInfo(methodName, paramTypePassedToMethod);

            var httpPostAttribute = method.CustomAttributes.First(x => x.AttributeType == typeof(ValidateAntiForgeryTokenAttribute));

            Assert.IsType<CustomAttributeData>(httpPostAttribute);
        }

        private static void ShouldHaveHttpPostAttribute(string methodName, Type paramTypePassedToMethod)
        {
            var method = GetMethodInfo(methodName, paramTypePassedToMethod);

            var httpPostAttribute = method.CustomAttributes.First(x => x.AttributeType == typeof(HttpPostAttribute));

            Assert.IsType<CustomAttributeData>(httpPostAttribute);
        }


        private static MethodInfo GetMethodInfo(string methodName, Type paramTypePassedToMethod)
        {
            CreateSut();

            var typeOfController = _sut.GetType();

            var method = typeOfController.GetMethod(methodName, new Type[] { paramTypePassedToMethod });
            return method;
        }

        private static void AddErrorToModelState()
        {
            _sut.ModelState.AddModelError("foo", "bar");
        }
    }
}