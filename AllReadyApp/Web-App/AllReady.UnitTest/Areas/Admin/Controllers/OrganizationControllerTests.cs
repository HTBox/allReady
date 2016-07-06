using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using AllReady.Areas.Admin.Models.Validators;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class OrganizationControllerTests
    {
        private readonly OrganizationEditModel _organizationEditModel;
        private static Mock<IMediator> _mediator;
        private static OrganizationController _sut;
        private const int Id = 4565;
        private readonly Type intType = typeof(int);
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
        public async Task IndexShouldSendOrganizationListQuery()
        {
            var mediator = new Mock<IMediator>();
            var sut = new OrganizationController(mediator.Object, null);
            await sut.Index();

            mediator.Verify(x => x.SendAsync(It.IsAny<OrganizationListQuery>()), Times.Once);
        }

        [Fact]
        public async Task IndexShouldReturnAViewWithTheCorrectViewModel()
        {
            var mediator = new Mock<IMediator>();
            var organizationSummaryModel = new List<OrganizationSummaryModel>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationListQuery>())).ReturnsAsync(organizationSummaryModel);

            var sut = new OrganizationController(mediator.Object, null);
            var result = (ViewResult) await sut.Index();

            Assert.IsType<ViewResult>(result);
            Assert.Same(organizationSummaryModel, result.ViewData.Model);
        }

    #endregion

    #region DetailsTests

        [Fact]
        public async Task DetailsShouldSendOrganizationDetailQueryWithTheCorrectOrganizationId()
        {
            CreateSut();
            await _sut.Details(Id);
            _mediator.Verify(x => x.SendAsync(It.Is<OrganizationDetailQuery>(y => y.Id == Id)));
        }

        [Fact]
        public async Task DetailsShouldReturnNullWhenOrganizationIsNotFound()
        {
            CreateSut();

            _mediator.Setup(x => x.SendAsync(It.Is<OrganizationDetailQuery>(y => y.Id == Id))).ReturnsAsync(null);

            var result = await _sut.Details(Id);

      Assert.IsType<NotFoundResult>(result);
    }

        [Fact]
        public async Task DetailsShouldReturnTheCorrectViewAndViewModelWhenOrganizationIsNotNull()
        {
            CreateSut();

      var model = new OrganizationDetailModel();

            _mediator.Setup(x => x.SendAsync(It.Is<OrganizationDetailQuery>(y => y.Id == Id))).ReturnsAsync(model);

            var result = (ViewResult) await _sut.Details(Id);

            Assert.IsType<ViewResult>(result);
            Assert.Same(model, result.ViewData.Model);
        }

        #endregion

    #region CreateTests

        [Fact]
        public void CreateReturnsCorrectView()
        {
            var sut = new OrganizationController(null, null);
            var result = (ViewResult)sut.Create();
            Assert.Equal("Edit", result.ViewName);         
        }
        #endregion

    #region EditTests

        [Fact]
        public async Task EditGetSendsOrganizationEditQueryWithCorrectOrganizationId()
        {
            const int organizationId = 1;
            var mediator = new Mock<IMediator>();

            var sut = new OrganizationController(mediator.Object, null);
            await sut.Edit(organizationId);

            mediator.Verify(x => x.SendAsync(It.Is<OrganizationEditQuery>(y => y.Id == organizationId)), Times.Once);
        }

        [Fact]
        public async Task EditGetReturnsHttpNotFoundResult_WhenOrganizationIsNotFound()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationEditQuery>())).ReturnsAsync(null);

            var sut = new OrganizationController(mediator.Object, null);
            var result = await sut.Edit(It.IsAny<int>());

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public async Task EditGetReturnsCorrectViewAndViewModel_WhenMediatorReturnsOrganizationEditModel()
        {
            CreateSut();

      var model = new OrganizationEditModel();

            _mediator.Setup(x => x.SendAsync(It.Is<OrganizationEditQuery>(y => y.Id == Id))).ReturnsAsync(model);

            var result = (ViewResult) await _sut.Edit(Id);

      Assert.Equal("Edit", result.ViewName);
      Assert.Same(model, result.ViewData.Model);
    }

        [Fact]
        public async Task EditPostReturnsBadRequestResult_WhenModelIsNull()
        {
            var sut = new OrganizationController(null, null);
            var result = await sut.Edit(null);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact (Skip = "NotImplemented")]
        public void EditPostInvokesOrganizationValidatorsValidateMethodWithTheCorrectOrganizationEditModel_WhenModelIsNotNull()
        {
        }

        [Fact(Skip = "NotImplemented")]
        public void EditPostAddsTheCorrectErrorsToModelState_WhenOrganizationValidatorHasErrors_AndModelIsNotNull()
        {
        }

        [Fact]
        public async Task EditPostReturnsTheCorrectViewAndViewModelWhenModelStateIsInvalid()
        {
            var model = new OrganizationEditModel();

            var controller = new OrganizationController(new Mock<IMediator>().Object, SuccessValidator());
            controller.ModelState.AddModelError("foo", "bar");
            var result = (ViewResult)await controller.Edit(model);

            Assert.Equal("Edit", result.ViewName);
            Assert.Same(model, result.ViewData.Model);
        }

        [Fact]
        public async Task EditPostSendsOrganizationNameUniqueQueryWithCorrectParametersWhenModelStateIsValid()
        {
            var mediator = new Mock<IMediator>();
            var organizationEditModel = new OrganizationEditModel { Name = "name", Id = 1 };

            var controller = new OrganizationController(mediator.Object, SuccessValidator());
            await controller.Edit(organizationEditModel);

            mediator.Verify(x => x.SendAsync(It.Is<OrganizationNameUniqueQuery>(y => y.OrganizationName == organizationEditModel.Name && y.OrganizationId == organizationEditModel.Id)), 
                Times.Once());
        }

        [Fact]
        public async Task EditPostSendsOrganizationEditCommandWithCorrectOrganizationEditModelWhenModelStateIsValidAndOrganizationNameIsUnique()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(y => y.SendAsync(It.IsAny<OrganizationNameUniqueQuery>())).ReturnsAsync(true);

            var controller = new OrganizationController(mediator.Object, SuccessValidator());
            await controller.Edit(_organizationEditModel);

            mediator.Verify(x => x.SendAsync(It.Is<OrganizationEditCommand>(y => y.Organization == _organizationEditModel)));
        }

        [Fact]
        public async Task EditPostRedirectsToCorrectActionWithCorrectData_WhenModelStateIsValid_AndOrganizationNameIsUnique()
        {
            var model = new OrganizationEditModel();
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(y => y.SendAsync(It.IsAny<OrganizationNameUniqueQuery>())).ReturnsAsync(true);
            mockMediator.Setup(x => x.SendAsync(It.Is<OrganizationEditCommand>(y => y.Organization == model))).ReturnsAsync(Id);            

            var controller = new OrganizationController(mockMediator.Object, SuccessValidator());
            var result = (RedirectToActionResult) await controller.Edit(model);

      Assert.Equal("Details", result.ActionName);
      Assert.Equal("Admin", result.RouteValues["area"]);
      Assert.Equal(Id, result.RouteValues["id"]);
    }

        [Fact(Skip = "NotImplemented")]
        public void EditPostAddsErrorToModelState_WhenModelStateIsValid_AndOrganizationNameIsNotUnique()
        {
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

        #endregion

    #region DeleteTests

        [Fact]
        public void DeleteGetShouldHaveActionNameAttributeOfDelete()
        {
            MethodShouldHaveCorrectAttribute(DeleteConst, intType, typeof(ActionNameAttribute), DeleteConst);
        }

        [Fact]
        public async Task DeleteGetWhenIdIsNullItShouldReturnAHttpNotFoundResponseShouldBeReturned()
        {
            CreateSut();
            var result = await _sut.Delete(null);
            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteGetWhenMediatorReturnsNullAHttpNotFoundResponseShouldBeReturned()
        {
            CreateSut();            
            _mediator.Setup(x => x.SendAsync(It.Is<OrganizationDetailQuery>(y => y.Id == Id))).ReturnsAsync(null);

            var result = await _sut.Delete(Id);

      Assert.IsType<NotFoundResult>(result);
    }

        [Fact]
        public async Task DeleteGetWhenMediatorReturnsAnOrganizationAViewOfThatOrganizationShouldBeShown()
        {
            CreateSut();

      var organizationModel = new OrganizationDetailModel();

            _mediator.Setup(x => x.SendAsync(It.Is<OrganizationDetailQuery>(y => y.Id == Id))).ReturnsAsync(organizationModel);

            var result = (ViewResult) await _sut.Delete(Id);

      Assert.Same(organizationModel, result.ViewData.Model);
    }

        [Fact]
        public void DeletePostShouldHaveValidateAntiForgeryTokenAttribute()
        {
            MethodShouldHaveValidateAntiForgeryTokenAttribute(DeleteConfirmedConst, intType);
        }

        [Fact]
        public void DeletePostShouldHaveActionNameAttributeWithDeleteAsAConstructorArguement()
        {
            MethodShouldHaveCorrectAttribute(DeleteConfirmedConst, intType, typeof(ActionNameAttribute), DeleteConst);
        }

        [Fact]
        public void DeletePostShouldHaveHttpPostAttribute()
        {
            MethodShouldHaveHttpPostAttribute(DeleteConfirmedConst, intType);
        }

        [Fact]
        public async Task DeletePostShouldSendAMessageWithTheCorrectIdUsingTheMediator()
        {
            CreateSut();
            await _sut.DeleteConfirmed(Id);
            _mediator.Verify(x => x.SendAsync(It.Is<OrganizationDeleteCommand>(y => y.Id == Id)));
        }

        [Fact]
        public async Task DeletePostShouldRedirectToTheIndex()
        {
            CreateSut();
            var result = (RedirectToActionResult) await _sut.DeleteConfirmed(Id);
            Assert.Equal("Index", result.ActionName);
        }

    #endregion

        #region HelperClasses
        private static void CreateSut()
        {
            _mediator = new Mock<IMediator>();
            _sut = new OrganizationController(_mediator.Object, SuccessValidator());
        }

    private static IOrganizationEditModelValidator SuccessValidator()
    {
      var mock = new Mock<IOrganizationEditModelValidator>();
      mock.Setup(v => v.Validate(It.IsAny<OrganizationEditModel>())).Returns(new List<KeyValuePair<string, string>>());
      return mock.Object;
    }

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

        #region "Test Models"
        public static LocationEditModel BogusAve => new LocationEditModel { Address1 = "25 Bogus Ave", City = "Agincourt", State = "Ontario", Country = "Canada", PostalCode = "M1T2T9" };
        public static OrganizationEditModel AgincourtAware => new OrganizationEditModel { Name = "Agincourt Awareness", Location = BogusAve, WebUrl = "http://www.AgincourtAwareness.ca", LogoUrl = "http://www.AgincourtAwareness.ca/assets/LogoLarge.png" };
        #endregion
    }
}