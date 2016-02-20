using System.Linq;
using AllReady.Controllers;
using AllReady.Features.Organizations;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNet.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;
using AllReady.Extensions;

namespace AllReady.UnitTest.Controllers
{
    public class OrganizationControllerTests
    {

        #region ShowOrganization Action Tests 

        [Fact]
        public void IndexReturnsAView()
        {
            var controller = new OrganizationController(new Mock<IMediator>().Object);
            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void IndexSendsOrganizationsQuery()
        {
            var mockedMediator = new Mock<IMediator>();
            var controller = new OrganizationController(mockedMediator.Object);
            controller.Index();

            mockedMediator.Verify(x => x.Send(It.IsAny<OrganizationsQuery>()), Times.Once());
        }

        [Fact]
        public void IndexHasRouteAttributeWithCorrectRoute()
        {
            var sut = new OrganizationController(null);
            var routeAttribute = sut.GetAttributesOn(x => x.Index()).OfType<RouteAttribute>().SingleOrDefault();
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Organizations/");
        }

        [Fact]
        public void ShowOrganizationHasRouteAttributeWithCorrectRoute()
        {
            var sut = new OrganizationController(null);
            var routeAttribute = (RouteAttribute)sut.GetAttributesOn(x => x.ShowOrganization(It.IsAny<int>())).SingleOrDefault(x => x.GetType() == typeof(RouteAttribute));
            Assert.NotNull(routeAttribute);
            Assert.Equal(routeAttribute.Template, "Organization/{id}/");
        }

        [Fact]
        public async Task ShowOrganization_ReturnsCorrectView()
        {
            OrganizationController controller;
            MockMediatorOrganizationDetailsQuery(out controller);

            var result = await controller.ShowOrganization(1);

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var finalResult = result as ViewResult;

            Assert.Equal("Organization", finalResult.ViewName);
        }

        [Fact]
        public async Task ShowOrganization_ReturnsNotFoundForNullOrganization()
        {
            OrganizationController controller;
            MockMediatorOrganizationDetailsQueryNullResult(out controller);

            var result = await controller.ShowOrganization(1) as HttpNotFoundResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void ShowOrganization_ReturnsNotFoundForZeroId()
        {
            OrganizationController controller;
            var mockMediator = MockMediatorOrganizationDetailsQuery(out controller);

            var result = controller.ShowOrganization(0).Result as HttpNotFoundResult;

            Assert.NotNull(result);
        }

        #endregion

        #region PrivacyPolicy Action Tests

        [Fact]
        public void PrivacyPolicy_ReturnsCorrectViewWhenOrgHasAPolicyDefined()
        {
            var modelWithPolicy = new OrganizationPrivacyPolicyViewModel { OrganizationName = "Org 2", Content = "A privacy policy" };

            OrganizationController controller;
            var mockMediator = MockMediatorOrganizationPrivacyPolicyQuery(out controller, modelWithPolicy);

            var result = controller.OrganizationPrivacyPolicy(2);

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result.Result);

            var finalResult = result.Result as ViewResult;

            Assert.Equal("OrgPrivacyPolicy", finalResult.ViewName);
        }

        [Fact]
        public void PrivacyPolicy_ReturnsRedirectToActionWhenOrgHasNoPolicyDefined()
        {
            OrganizationController controller;
            var mockMediator = MockMediatorOrganizationPrivacyPolicyQuery(out controller);

            var result = controller.OrganizationPrivacyPolicy(2);

            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result.Result);
        }

        [Fact]
        public void PrivacyPolicy_ReturnsNotFoundForNullOrganization()
        {
            OrganizationController controller;
            var mockMediator = MockMediatorOrganizationPrivacyPolicyQueryNullResult(out controller);

            var result = controller.OrganizationPrivacyPolicy(1).Result as HttpNotFoundResult;

            Assert.NotNull(result);
        }


        [Fact]
        public void PrivacyPolicy_ReturnsNotFoundForZeroId()
        {
            OrganizationController controller;
            var mockMediator = MockMediatorOrganizationPrivacyPolicyQuery(out controller);

            var result = controller.OrganizationPrivacyPolicy(0).Result as HttpNotFoundResult;

            Assert.NotNull(result);
        }

        #endregion

        #region Helper Methods

        private static void MockMediatorOrganizationDetailsQuery(out OrganizationController controller, OrganizationViewModel model = null)
        {
            if (model == null)
                model = new OrganizationViewModel { Id = 1, Name = "Org 1" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationDetailsQueryAsync>())).Returns(() => Task.FromResult(model)).Verifiable();
            controller = new OrganizationController(mockMediator.Object);
        }

        private static void MockMediatorOrganizationDetailsQueryNullResult(out OrganizationController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationDetailsQueryAsync>())).Returns(() => Task.FromResult((OrganizationViewModel)null)).Verifiable();
            controller = new OrganizationController(mockMediator.Object, dataMock.Object);
            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorOrganizationPrivacyPolicyQuery(out OrganizationController controller, OrganizationPrivacyPolicyViewModel model = null)
        {
            var dataMock = new Mock<IAllReadyDataAccess>();

            if (model == null) model = new OrganizationPrivacyPolicyViewModel { OrganizationName = "Org 1", Content = null };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganziationPrivacyPolicyQueryAsync>())).Returns(() => Task.FromResult(model)).Verifiable();
            controller = new OrganizationController(mockMediator.Object, dataMock.Object);
            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorOrganizationPrivacyPolicyQueryNullResult(out OrganizationController controller)
        {
            var dataMock = new Mock<IAllReadyDataAccess>();

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganziationPrivacyPolicyQueryAsync>())).Returns(() => Task.FromResult((OrganizationPrivacyPolicyViewModel)null)).Verifiable();
            controller = new OrganizationController(mockMediator.Object, dataMock.Object);
            return mockMediator;
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
    }
}