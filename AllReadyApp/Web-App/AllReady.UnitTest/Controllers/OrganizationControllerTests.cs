using AllReady.Controllers;
using AllReady.Features.Organizations;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Controllers
{
    public class OrganizationControllerTests
    {
        [Fact]
        public void ShowOrganization_ReturnsCorrectView()
        {
            OrganizationController controller;
            var mockMediator = MockMediatorOrganizationDetailsQuery(out controller);

            var result = controller.ShowOrganization(1);

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result.Result);

            var finalResult = result.Result as ViewResult;

            Assert.Equal("Organization", finalResult.ViewName);
        }

        [Fact]
        public void ShowOrganization_ReturnsNotFoundForInvalidId()
        {
            OrganizationController controller;
            var mockMediator = MockMediatorOrganizationDetailsQuery(out controller);

            var result = controller.ShowOrganization(0).Result as HttpNotFoundResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void ShowOrganization_ReturnsNotFoundForNullOrganization()
        {
            OrganizationController controller;
            var mockMediator = MockMediatorOrganizationDetailsQueryNullResult(out controller);

            var result = controller.ShowOrganization(1).Result as HttpNotFoundResult;

            Assert.NotNull(result);
        }

        #region Helper Methods

        private static Mock<IMediator> MockMediatorOrganizationDetailsQuery(out OrganizationController controller, OrganizationViewModel model = null)
        {
            var dataMock = new Mock<IAllReadyDataAccess>();

            if (model == null) model = new OrganizationViewModel { Id = 1, Name = "Org 1" };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationDetailsQueryAsync>())).Returns(() => Task.FromResult(model)).Verifiable();
            controller = new OrganizationController(mockMediator.Object, dataMock.Object);
            return mockMediator;
        }

        private static Mock<IMediator> MockMediatorOrganizationDetailsQueryNullResult(out OrganizationController controller)
        {
            var dataMock = new Mock<IAllReadyDataAccess>();            

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<OrganizationDetailsQueryAsync>())).Returns(() => Task.FromResult((OrganizationViewModel)null)).Verifiable();
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