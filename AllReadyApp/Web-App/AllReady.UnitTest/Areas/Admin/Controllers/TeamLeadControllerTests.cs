using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.TeamLead;
using AllReady.Areas.Admin.ViewModels.TeamLead;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class TeamLeadControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            // Arrange
            MockTeamLeadItineraryListViewModelQuery(out var sut);

            var mockContext = MockControllerContextWithUser(BasicUser());
            sut.ControllerContext = mockContext.Object;

            // Act
            var result = await sut.Index();

            // Assert
            result.ShouldBeOfType(typeof(ViewResult));
        }

        [Fact]
        public async Task Index_CallsTeamLeadItineraryListViewModelQuery_Once()
        {
            // Arrange
            var mockMediator = MockTeamLeadItineraryListViewModelQuery(out var sut);

            var mockContext = MockControllerContextWithUser(BasicUser());
            sut.ControllerContext = mockContext.Object;

            // Act
            await sut.Index();

            // Assert
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<TeamLeadItineraryListViewModelQuery>()), Times.Once);
        }

        private static Mock<ControllerContext> MockControllerContextWithUser(ClaimsPrincipal principle)
        {
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User).Returns(() => principle);
            var mockContext = new Mock<ControllerContext>();

            mockContext.Object.HttpContext = mockHttpContext.Object;
            return mockContext;
        }

        private static ClaimsPrincipal BasicUser()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(AllReady.Security.ClaimTypes.UserType, nameof(UserType.BasicUser))
            }));
        }

        private static Mock<IMediator> MockTeamLeadItineraryListViewModelQuery(out TeamLeadController controller)
        {
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<TeamLeadItineraryListViewModelQuery>())).ReturnsAsync(new TeamLeadItineraryListerViewModel()).Verifiable();
            controller = new TeamLeadController(mockMediator.Object);
            return mockMediator;
        }
    }
}
