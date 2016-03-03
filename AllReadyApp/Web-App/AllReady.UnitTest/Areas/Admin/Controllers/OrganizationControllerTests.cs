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

        private static Mock<IMediator> _mediator;

        private static OrganizationController _sut;

        private const int Id = 4565;

        private Type IntType = typeof(int);

        private const string DeleteConst = "Delete";

        private const string DeleteConfirmedConst = "DeleteConfirmed";

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
            ClassHasCorrectAttribute(typeof(AreaAttribute), "Admin");
        }

        [Fact]
        public void TheControllerShouldHaveAnAuthorizeAttributeOfSiteAdmin()
        {
            ClassHasCorrectAttribute(typeof(AuthorizeAttribute), "SiteAdmin");
        }

        #endregion

        #region IndexTests

        [Fact]
        public void IndexShouldReturnTheAViewWithTheListReturnedFromTheMediator()
        {
            CreateSut();

            var organizationSummaryModel = new List<OrganizationSummaryModel> { new OrganizationSummaryModel() };

            _mediator.Setup(x => x.Send(It.IsAny<OrganizationListQuery>())).Returns(organizationSummaryModel);

            var result = (ViewResult)_sut.Index();

            Assert.Same(organizationSummaryModel, result.ViewData.Model);
        }

        #endregion

        #region DetailsTests

        [Fact]
        public void DetailsShouldPassTheIdToTheMediator()
        {
            CreateSut();

            _sut.Details(Id);

            _mediator.Verify(x => x.Send(It.Is<OrganizationDetailQuery>(y => y.Id == Id)));
        }

        [Fact]
        public void WhenTheMediatorReturnsNullForThatIdHttpNotFoundShouldBeReturned()
        {
            CreateSut();

            _mediator.Setup(x => x.Send(It.Is<OrganizationDetailQuery>(y => y.Id == Id))).Returns<OrganizationDetailModel>(null);

            var result = _sut.Details(Id);

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void WhenTheMediatorReturnsAOrganizationDetailModelForThatIdAViewShouldBeReturned()
        {
            CreateSut();

            var model = new OrganizationDetailModel();

            _mediator.Setup(x => x.Send(It.Is<OrganizationDetailQuery>(y => y.Id == Id))).Returns(model);

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

            var result = (RedirectToRouteResult)_sut.Create(_organizationEditModel);

            Assert.Equal("areaRoute", result.RouteName);
            Assert.Equal("Organization", result.RouteValues["controller"]);
            Assert.Equal("Index", result.RouteValues["action"]);
        }

        [Fact]
        public void CreateNewOrganizationMediatorShouldBeCalledWithAppropriateDataWhenModelStateIsValid()
        {
            CreateSut();

            _sut.Create(_organizationEditModel);

            _mediator.Verify(x => x.Send(It.Is<OrganizationEditCommand>( y => y.Organization == _organizationEditModel)));
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
            MethodShouldHaveValidateAntiForgeryTokenAttribute("Create", typeof(OrganizationEditModel));
        }

        [Fact]
        public void CreateNewOrganizationPostShouldHavePostAttribute()
        {
            MethodShouldHaveHttpPostAttribute("Create", typeof(OrganizationEditModel));
        }

        #endregion

        #region EditTests

        [Fact]
        public void EditGetShouldReturnHttpNotFoundWhenMediatorReturnsNullForId()
        {
            CreateSut();

            _mediator.Setup(x => x.Send(It.Is<OrganizationEditQuery>(y => y.Id == Id))).Returns<OrganizationEditModel>(null);

            var result = _sut.Edit(Id);

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void EditGetShouldReturnEditViewWhenMediatorReturnsOrganizationEditModel()
        {
            CreateSut();

            var model = new OrganizationEditModel();

            _mediator.Setup(x => x.Send(It.Is<OrganizationEditQuery>(y => y.Id == Id))).Returns(model);

            var result = (ViewResult)_sut.Edit(Id);

            Assert.Equal("Edit", result.ViewName);
            Assert.Same(model, result.ViewData.Model);
        }

        [Fact]
        public void EditPostShouldHaveValidateAntiForgeryTokenAttribute()
        {
            MethodShouldHaveValidateAntiForgeryTokenAttribute("Edit", typeof(OrganizationEditModel));
        }

        [Fact]
        public void EditPostShouldHaveHttpPostAttribute()
        {
            MethodShouldHaveHttpPostAttribute("Edit", typeof(OrganizationEditModel));
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
        public void EditPostShouldRedirectToDetailsWithTheIdFromTheMediatorIfModelStateIsValid()
        {
            CreateSut();

            var model = new OrganizationEditModel();

            _mediator.Setup(x => x.Send(It.Is<OrganizationEditCommand>(y => y.Organization == model))).Returns(Id);

            var result = (RedirectToActionResult)_sut.Edit(model);

            Assert.Equal("Details", result.ActionName);
            Assert.Equal("Admin", result.RouteValues["area"]);
            Assert.Equal(Id, result.RouteValues["id"]);
        }

        #endregion

        #region DeleteTests

        [Fact]
        public void DeleteGetShouldHaveActionNameAttributeOfDelete()
        {
            MethodShouldHaveCorrectAttribute(DeleteConst, IntType, typeof(ActionNameAttribute), DeleteConst);
        }

        [Fact]
        public void DeleteGetWhenIdIsNullItShouldReturnAHttpNotFoundResponseShouldBeReturned()
        {
            CreateSut();
                        
            var result = _sut.Delete(null);

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void DeleteGetWhenMediatorReturnsNullAHttpNotFoundResponseShouldBeReturned()
        {
            CreateSut();
                        
            _mediator.Setup(x => x.Send(It.Is<OrganizationDetailQuery>(y => y.Id == Id))).Returns<OrganizationDetailModel>(null);

            var result = _sut.Delete(Id);

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void DeleteGetWhenMediatorReturnsAnOrganizationAViewOfThatOrganizationShouldBeShown()
        {
            CreateSut();

            var organizationModel = new OrganizationDetailModel();

            _mediator.Setup(x => x.Send(It.Is<OrganizationDetailQuery>(y => y.Id == Id))).Returns(organizationModel);

            var result = (ViewResult)_sut.Delete(Id);

            Assert.Same(organizationModel, result.ViewData.Model);
        }

        [Fact]
        public void DeletePostShouldHaveValidateAntiForgeryTokenAttribute()
        {
            MethodShouldHaveValidateAntiForgeryTokenAttribute(DeleteConfirmedConst, IntType);
        }

        [Fact]
        public void DeletePostShouldHaveActionNameAttributeWithDeleteAsAConstructorArguement()
        {
            MethodShouldHaveCorrectAttribute(DeleteConfirmedConst, IntType, typeof(ActionNameAttribute), DeleteConst);
        }

        [Fact]
        public void DeletePostShouldHaveHttpPostAttribute()
        {
            MethodShouldHaveHttpPostAttribute(DeleteConfirmedConst, IntType);
        }

        [Fact]
        public void DeletePostShouldSendAMessageWithTheCorrectIdUsingTheMediator()
        {
            CreateSut();

            _sut.DeleteConfirmed(Id);

            _mediator.Verify(x => x.Send(It.Is<OrganizationDeleteCommand>(y => y.Id == Id)));
        }

        [Fact]
        public void DeletePostShouldRedirectToTheIndex()
        {
            CreateSut();

            var result = (RedirectToActionResult)_sut.DeleteConfirmed(Id);

            Assert.Equal("Index", result.ActionName);
        }

        #endregion

        private static void CreateSut()
        {
            _mediator = new Mock<IMediator>();

            _sut = new OrganizationController(_mediator.Object);
        }
        
        private static void AddErrorToModelState()
        {
            _sut.ModelState.AddModelError("foo", "bar");
        }

        #region PotentialHelperClass

        private static MethodInfo GetMethodInfo(string methodName, Type paramTypePassedToMethod)
        {
            CreateSut();

            var typeOfController = _sut.GetType();

            var method = typeOfController.GetMethod(methodName, new Type[] { paramTypePassedToMethod });
            return method;
        }

        private static void ClassHasCorrectAttribute(Type attributeType, string valueOfConstructorArgument)
        {
            CreateSut();

            var sutType = _sut.GetType();

            var areaAttribute = sutType.CustomAttributes.First(x => x.AttributeType.Equals(attributeType));

            var constructorArg = areaAttribute.ConstructorArguments.First().Value as string;

            Assert.Equal(valueOfConstructorArgument, constructorArg);
        }

        private static void MethodShouldHaveCorrectAttribute(string methodName, Type paramTypePassedToMethod, Type attributeThatMethodShouldHave, string constructorArgument)
        {
            var attribute = ExtractCustomAttributeFromMethod(methodName, paramTypePassedToMethod, attributeThatMethodShouldHave);

            Assert.Equal(constructorArgument, attribute.ConstructorArguments[0].Value);
        }

        private static void MethodShouldHaveCorrectAttribute(string methodName, Type paramTypePassedToMethod, Type attributeThatMethodShouldHave)
        {
            var attribute = ExtractCustomAttributeFromMethod(methodName, paramTypePassedToMethod, attributeThatMethodShouldHave);

            Assert.IsType<CustomAttributeData>(attribute);
        }

        private static CustomAttributeData ExtractCustomAttributeFromMethod(string methodName, Type paramTypePassedToMethod, Type attributeThatMethodShouldHave)
        {
            var method = GetMethodInfo(methodName, paramTypePassedToMethod);

            var attribute = method.CustomAttributes.First(x => x.AttributeType.Equals(attributeThatMethodShouldHave));
            return attribute;
        }

        private static void MethodShouldHaveValidateAntiForgeryTokenAttribute(string methodName, Type paramTypePassedToMethod)
        {
            MethodShouldHaveCorrectAttribute(methodName, paramTypePassedToMethod, typeof(ValidateAntiForgeryTokenAttribute));
        }

        private static void MethodShouldHaveHttpPostAttribute(string methodName, Type paramTypePassedToMethod)
        {
            MethodShouldHaveCorrectAttribute(methodName, paramTypePassedToMethod, typeof(HttpPostAttribute));
        }

        #endregion
    }
}