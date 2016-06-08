using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class OrganizationControllerTests : InMemoryContextTest
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
        public void CreatePostShouldInsertOrganization()
        {
            // Arrange
            CreateSut();
            OrganizationEditModel model = AgincourtAware;
            IRequest<int> command = new OrganizationEditCommand() { Organization = model };
            _mediator.Setup(y => y.Send(It.IsAny<OrganizationNameUniqueQuery>())).Returns(() =>
            {
                return true;
            });
            _mediator.Setup(x => x.Send(It.IsAny<OrganizationEditCommand>())).Returns(() => {
                IRequestHandler<OrganizationEditCommand, int> handler = new OrganizationEditCommandHandler(Context);
                return handler.Handle((OrganizationEditCommand)command);
            });

            // Act
            _sut.Edit(model);

            // Assert
            Assert.Single(Context.Organizations.Where(t => t.Name == model.Name));

        }

        [Fact]
        public void CreateNewOrganizationWithExistingOrganizationNameReturnsEditView()
        {
            CreateSut();
            var model = new OrganizationEditModel();
            model.Name = "test";
            model.Id = 0;
            _mediator.Setup(x => x.Send(It.Is<OrganizationEditCommand>(y => y.Organization == model))).Returns(Id);
            var result = (ViewResult)_sut.Create();
            Assert.Equal("Edit", result.ViewName);            
        }
        #endregion

        #region EditTests

        [Fact]
        public void EditOrganizationShouldReturnBadResultWhenModelIsNull()
        {
            CreateSut();

            var result = _sut.Edit(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void EditOrganizationMediatorShouldBeCalledWithAppropriateDataWhenModelStateIsValid()
        {
            var mockMediator = new Mock<IMediator>();
            var controller = new OrganizationController(mockMediator.Object);

            var mockContext = MockActionContextWithUser(SiteAdmin());
            controller.ActionContext = mockContext.Object;
            mockMediator.Setup(y => y.Send(It.IsAny<OrganizationNameUniqueQuery>())).Returns(() =>{return true;});
            controller.Edit(_organizationEditModel);

            mockMediator.Verify(x => x.Send(It.Is<OrganizationEditCommand>(y => y.Organization == _organizationEditModel)));
        }

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
            var controller = new OrganizationController(new Mock<IMediator>().Object);

            var mockContext = MockActionContextWithUser(SiteAdmin());
            controller.ActionContext = mockContext.Object;

            controller.ModelState.AddModelError("foo", "bar");

            var model = new OrganizationEditModel();

            var result = (ViewResult)controller.Edit(model);

            Assert.Equal("Edit", result.ViewName);
            Assert.Same(model, result.ViewData.Model);
        }

        [Fact]
        public void EditPostShouldRedirectToDetailsWithTheIdFromTheMediatorIfModelStateIsValid()
        {
            var mockMediator = new Mock<IMediator>();

            var controller = new OrganizationController(mockMediator.Object);

            var mockContext = MockActionContextWithUser(SiteAdmin());
            controller.ActionContext = mockContext.Object;

            var model = new OrganizationEditModel();

            mockMediator.Setup(y => y.Send(It.IsAny<OrganizationNameUniqueQuery>())).Returns(() => { return true; });
            mockMediator.Setup(x => x.Send(It.Is<OrganizationEditCommand>(y => y.Organization == model))).Returns(Id);

            var result = (RedirectToActionResult)controller.Edit(model);

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

        private static ClaimsPrincipal SiteAdmin()
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(AllReady.Security.ClaimTypes.UserType, UserType.SiteAdmin.ToString())
                    }));
        }

        private static ClaimsPrincipal OrgAdminWithMissingOrgId()
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(AllReady.Security.ClaimTypes.UserType, UserType.OrgAdmin.ToString())
                    }));
        }

        private static Mock<ActionContext> MockActionContextWithUser(ClaimsPrincipal principle)
        {
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User)
                .Returns(() => principle);
            var mockContext = new Mock<ActionContext>();

            mockContext.Object.HttpContext = mockHttpContext.Object;
            return mockContext;
        }

        #endregion
        #region "Test Models"
        public static LocationEditModel BogusAve { get { return new LocationEditModel() { Address1 = "25 Bogus Ave", City = "Agincourt", State = "Ontario", Country = "Canada", PostalCode = "M1T2T9" }; } }
        public static OrganizationEditModel AgincourtAware { get { return new OrganizationEditModel() { Name = "Agincourt Awareness", Location = BogusAve, WebUrl = "http://www.AgincourtAwareness.ca", LogoUrl = "http://www.AgincourtAwareness.ca/assets/LogoLarge.png" }; } }
        #endregion
    }
}