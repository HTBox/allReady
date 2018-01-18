using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Organizations;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Xunit;
using Shouldly;
using AllReady.Models;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class OrganizationControllerTests
    {
        private readonly OrganizationEditViewModel _organizationEditModel;
        private static Mock<IMediator> _mediator;
        private static OrganizationController _sut;
        private const int Id = 4565;
        private readonly Type intType = typeof(int);
        private const string DeleteConst = "Delete";
        private const string DeleteConfirmedConst = "DeleteConfirmed";

        public OrganizationControllerTests()
        {
            _organizationEditModel = new OrganizationEditViewModel
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
            ClassHasCorrectAttribute(typeof(AreaAttribute), AreaNames.Admin);
        }

        [Fact]
        public void TheControllerShouldHaveAnAuthorizeAttributeOfSiteAdmin()
        {
            ClassHasCorrectAttribute(typeof(AuthorizeAttribute), nameof(UserType.SiteAdmin));
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
            var organizationSummaryModel = new List<OrganizationSummaryViewModel>();
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationListQuery>())).ReturnsAsync(organizationSummaryModel);

            var sut = new OrganizationController(mediator.Object, null);
            var result = (ViewResult)await sut.Index();

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

            _mediator.Setup(x => x.SendAsync(It.Is<OrganizationDetailQuery>(y => y.Id == Id))).ReturnsAsync((OrganizationDetailViewModel)null);

            var result = await _sut.Details(Id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsShouldReturnTheCorrectViewAndViewModelWhenOrganizationIsNotNull()
        {
            CreateSut();

            var model = new OrganizationDetailViewModel();

            _mediator.Setup(x => x.SendAsync(It.Is<OrganizationDetailQuery>(y => y.Id == Id))).ReturnsAsync(model);

            var result = (ViewResult)await _sut.Details(Id);

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
            mediator.Setup(x => x.SendAsync(It.IsAny<OrganizationEditQuery>())).ReturnsAsync((OrganizationEditViewModel)null);

            var sut = new OrganizationController(mediator.Object, null);
            var result = await sut.Edit(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditGetReturnsCorrectViewAndViewModel_WhenMediatorReturnsOrganizationEditModel()
        {
            CreateSut();

            var model = new OrganizationEditViewModel();

            _mediator.Setup(x => x.SendAsync(It.Is<OrganizationEditQuery>(y => y.Id == Id))).ReturnsAsync(model);

            var result = (ViewResult)await _sut.Edit(Id);

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

        [Fact]
        public async Task EditPostInvokesOrganizationValidatorsValidateMethodWithTheCorrectOrganizationEditModel_WhenModelIsNotNull()
        {
            //Arrange
            var organizationValidatorMock = new Mock<IOrganizationEditModelValidator>();
            var errors = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("FakeProperty", "FakeError") };
            var organizationEditModel = new OrganizationEditViewModel {Id = 1, Name = "Test1"};
            organizationValidatorMock.Setup(o => o.Validate(organizationEditModel)).Returns(errors);
            var sut = new OrganizationController(null, organizationValidatorMock.Object);

            //Act
            var result = await sut.Edit(organizationEditModel) as ViewResult;

            //Assert
            organizationValidatorMock.Verify(o => o.Validate(organizationEditModel), Times.Once);
        }

        [Fact]
        public async Task EditPostAddsTheCorrectErrorsToModelState_WhenOrganizationValidatorHasErrors_AndModelIsNotNull()
        {
            //Arrange
            var organizationValidatorMock = new Mock<IOrganizationEditModelValidator>();
            var errors = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("FakeProperty", "FakeError") };
            organizationValidatorMock.Setup(o => o.Validate(It.IsAny<OrganizationEditViewModel>())).Returns(errors);
            var sut = new OrganizationController(null, organizationValidatorMock.Object);

            //Act
            var result = await sut.Edit(_organizationEditModel) as ViewResult;

            //Assert
            result.ViewData.ModelState.ErrorCount.ShouldBe(1);
            result.ViewData.ModelState["FakeProperty"].ValidationState.ShouldBe(ModelValidationState.Invalid);
            result.ViewData.ModelState["FakeProperty"].Errors.Single().ErrorMessage.ShouldBe("FakeError");
        }

        [Fact]
        public async Task EditPostReturnsTheCorrectViewAndViewModelWhenModelStateIsInvalid()
        {
            var model = new OrganizationEditViewModel();

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
            var organizationEditModel = new OrganizationEditViewModel { Name = "name", Id = 1 };

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

            mediator.Verify(x => x.SendAsync(It.Is<EditOrganizationCommand>(y => y.Organization == _organizationEditModel)));
        }

        [Fact]
        public async Task EditPostRedirectsToCorrectActionWithCorrectData_WhenModelStateIsValid_AndOrganizationNameIsUnique()
        {
            var model = new OrganizationEditViewModel();
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(y => y.SendAsync(It.IsAny<OrganizationNameUniqueQuery>())).ReturnsAsync(true);
            mockMediator.Setup(x => x.SendAsync(It.Is<EditOrganizationCommand>(y => y.Organization == model))).ReturnsAsync(Id);

            var controller = new OrganizationController(mockMediator.Object, SuccessValidator());
            var result = (RedirectToActionResult)await controller.Edit(model);

            Assert.Equal("Details", result.ActionName);
            Assert.Equal(AreaNames.Admin, result.RouteValues["area"]);
            Assert.Equal(Id, result.RouteValues["id"]);
        }

        [Fact]
        public async Task EditPostAddsErrorToModelState_WhenModelStateIsValid_AndOrganizationNameIsNotUnique()
        {
            //Arrange
            var organizationValidatorMock = new Mock<IOrganizationEditModelValidator>();
            var errors = new List<KeyValuePair<string, string>> { };
            organizationValidatorMock.Setup(o => o.Validate(It.IsAny<OrganizationEditViewModel>())).Returns(errors);

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.SendAsync(It.IsAny<OrganizationNameUniqueQuery>())).ReturnsAsync(false);

            var sut = new OrganizationController(mediatorMock.Object, organizationValidatorMock.Object);

            //Act
            var result = await sut.Edit(_organizationEditModel) as ViewResult;

            //Assert
            result.ViewData.ModelState.ErrorCount.ShouldBe(1);
            result.ViewData.ModelState["Name"].ValidationState.ShouldBe(ModelValidationState.Invalid);
            result.ViewData.ModelState["Name"].Errors.Single().ErrorMessage
                  .ShouldBe("Organization with same name already exists. Please use different name.");
        }

        [Fact]
        public void EditPostShouldHaveValidateAntiForgeryTokenAttribute()
        {
            MethodShouldHaveValidateAntiForgeryTokenAttribute("Edit", typeof(OrganizationEditViewModel));
        }

        [Fact]
        public void EditPostShouldHaveHttpPostAttribute()
        {
            MethodShouldHaveHttpPostAttribute("Edit", typeof(OrganizationEditViewModel));
        }

        #endregion

        #region DeleteTests

        [Fact]
        public async Task DeleteGetSendsDeleteQueryWithTheCorrectOrgId()
        {
            const int orgId = 1;

            var mediator = new Mock<IMediator>();
            var sut = new OrganizationController(mediator.Object, null);

            await sut.Delete(orgId);

            mediator.Verify(x => x.SendAsync(It.Is<DeleteQuery>(y => y.OrgId == orgId)), Times.Once);
        }

        [Fact]
        public async Task DeleteGetReturnsNotFoundResultWhenViewModelIsNull()
        {
            var sut = new OrganizationController(Mock.Of<IMediator>(), null);

            var result = await sut.Delete(It.IsAny<int>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteGetAssignsCorrectTitleToViewModel()
        {
            var viewModel = new DeleteViewModel { Title = "Title" };

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.SendAsync(It.IsAny<DeleteQuery>())).ReturnsAsync(viewModel);

            var sut = new OrganizationController(mediator.Object, null);
            var result = await sut.Delete(It.IsAny<int>()) as ViewResult;
            var modelResult = result.Model as DeleteViewModel;

            Assert.Equal(modelResult.Title, $"Delete {viewModel.Name}");
        }

        [Fact]
        public void DeleteGetShouldHaveActionNameAttributeOfDelete()
        {
            MethodShouldHaveCorrectAttribute(DeleteConst, intType, typeof(ActionNameAttribute), DeleteConst);
        }

        [Fact]
        public async Task DeleteConfirmedShouldSendAMessageWithTheCorrectIdUsingTheMediator()
        {
            CreateSut();
            await _sut.DeleteConfirmed(Id);
            _mediator.Verify(x => x.SendAsync(It.Is<DeleteOrganization>(y => y.Id == Id)));
        }

        [Fact]
        public async Task DeleteConfirmedShouldRedirectToTheCorrectActionAndArea()
        {
            var routeValueDictionary = new RouteValueDictionary { ["area"] = AreaNames.Admin };

            var sut = new OrganizationController(Mock.Of<IMediator>(), null);
            var result = await sut.DeleteConfirmed(It.IsAny<int>()) as RedirectToActionResult;

            Assert.Equal(nameof(OrganizationController.Index), result.ActionName);
            Assert.Equal(result.RouteValues, routeValueDictionary);
        }

        [Fact]
        public void DeleteConfirmedShouldHaveValidateAntiForgeryTokenAttribute()
        {
            MethodShouldHaveValidateAntiForgeryTokenAttribute(DeleteConfirmedConst, intType);
        }

        [Fact]
        public void DeleteConfirmedShouldHaveActionNameAttributeWithDeleteAsAConstructorArguement()
        {
            MethodShouldHaveCorrectAttribute(DeleteConfirmedConst, intType, typeof(ActionNameAttribute), DeleteConst);
        }

        [Fact]
        public void DeleteConfirmedShouldHaveHttpPostAttribute()
        {
            MethodShouldHaveHttpPostAttribute(DeleteConfirmedConst, intType);
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
            mock.Setup(v => v.Validate(It.IsAny<OrganizationEditViewModel>())).Returns(new List<KeyValuePair<string, string>>());
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

            var areaAttribute = sutType.GetTypeInfo().CustomAttributes.First(x => x.AttributeType == attributeType);

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
        public static LocationEditViewModel BogusAve => new LocationEditViewModel { Address1 = "25 Bogus Ave", City = "Agincourt", State = "Ontario", Country = "Canada", PostalCode = "M1T2T9" };
        public static OrganizationEditViewModel AgincourtAware => new OrganizationEditViewModel { Name = "Agincourt Awareness", Location = BogusAve, WebUrl = "http://www.AgincourtAwareness.ca", LogoUrl = "http://www.AgincourtAwareness.ca/assets/LogoLarge.png" };
        #endregion
    }
}
