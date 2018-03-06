using System.Security.Claims;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Features.Goals;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Areas.Admin.ViewModels.Goal;
using AllReady.Constants;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ClaimTypes = AllReady.Security.ClaimTypes;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class GoalAdminControllerTests
    {
        private const int OrgAdminOrgId = 123;

        #region Create

        [Fact]
        public async void TestGoalCreateForWrongOrgAdminReturns401()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>()))
                .ReturnsAsync(new CampaignSummaryViewModel())
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(654));
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.Create(123);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async void TestGoalCreateForOrgAdmin()
        {
            const int campaignId = 123;
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>()))
                .ReturnsAsync(new CampaignSummaryViewModel {Id = campaignId, OrganizationId = OrgAdminOrgId})
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(OrgAdminOrgId));
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.Create(campaignId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Edit", result.ViewName);
            var vm = result.ViewData.Model as GoalEditViewModel;
            Assert.NotNull(vm);

            Assert.Equal(campaignId, vm.CampaignId);
            Assert.Equal(OrgAdminOrgId, vm.OrganizationId);
        }

        [Fact]
        public async void TestGoalCreateForSiteAdmin()
        {
            const int campaignId = 123;
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>()))
                .ReturnsAsync(new CampaignSummaryViewModel {Id = campaignId, OrganizationId = OrgAdminOrgId})
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.Create(campaignId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Edit", result.ViewName);
            var vm = result.ViewData.Model as GoalEditViewModel;
            Assert.NotNull(vm);

            Assert.Equal(campaignId, vm.CampaignId);
            Assert.Equal(OrgAdminOrgId, vm.OrganizationId);
        }

        #endregion

        #region CreatePost

        [Fact]
        public async void TestGoalCreatePostForWrongOrgAdminReturns401()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>()))
                .ReturnsAsync(new CampaignSummaryViewModel())
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(654));
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.Create(123, new GoalEditViewModel());

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async void TestGoalCreatePostForSiteAdminWithValidModelStateReturnsRedirectToAction()
        {
            const int campaignId = 123;
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>()))
                .ReturnsAsync(new CampaignSummaryViewModel {Id = campaignId, OrganizationId = OrgAdminOrgId})
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            goalController.ControllerContext = mockContext.Object;

            var vm = new GoalEditViewModel {GoalType = GoalType.Text, TextualGoal = "Aim to please"};

            // Act
            var result = await goalController.Create(campaignId, vm) as RedirectToActionResult;


            // Assert
            Assert.NotNull(result);
            Assert.Equal("Details", result.ActionName);
            Assert.Equal("Campaign", result.ControllerName);
            Assert.Equal(AreaNames.Admin, result.RouteValues["area"]);
            Assert.Equal(campaignId, result.RouteValues["id"]);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<GoalEditCommand>()), Times.Once);
        }

        [Fact]
        public async void GoalCreatePostForOrgAdminWithValidModelStateReturnsRedirectToAction()
        {
            const int campaignId = 123;
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignSummaryQuery>()))
                .ReturnsAsync(new CampaignSummaryViewModel {Id = campaignId, OrganizationId = OrgAdminOrgId})
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(OrgAdminOrgId));
            goalController.ControllerContext = mockContext.Object;

            var vm = new GoalEditViewModel {GoalType = GoalType.Text, TextualGoal = "Aim to please"};

            // Act
            var result = await goalController.Create(campaignId, vm) as RedirectToActionResult;


            // Assert
            Assert.NotNull(result);
            Assert.Equal("Details", result.ActionName);
            Assert.Equal("Campaign", result.ControllerName);
            Assert.Equal(AreaNames.Admin, result.RouteValues["area"]);
            Assert.Equal(campaignId, result.RouteValues["id"]);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<GoalEditCommand>()), Times.Once);
        }

        #endregion

        #region Delete
        [Fact]
        public async void TestGoalDeleteForWrongOrgAdminReturns401()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalDeleteQuery>()))
                .ReturnsAsync(new GoalDeleteViewModel {OwningOrganizationId = OrgAdminOrgId})
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(654));
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.Delete(567);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async void TestGoalDeleteForOrgAdminForNonexistantGoalReturn404()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalDeleteQuery>()))
                .ReturnsAsync((GoalDeleteViewModel)null)
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, true, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(OrgAdminOrgId));
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.Delete(987);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void TestGoalDeleteOrgAdminDisplaysConfirmationPage()
        {
            const int goalId = 567;
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalDeleteQuery>()))
                .ReturnsAsync(new GoalDeleteViewModel
                {
                    Id = goalId,
                    OwningOrganizationId = OrgAdminOrgId,
                    TextualGoal = "This goal should be deleted",
                    GoalType = GoalType.Numeric,
                    NumericGoal = 100
                })
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, true, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(OrgAdminOrgId));
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.Delete(goalId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Delete", result.ViewName);
            var goalDeleteViewModel = (GoalDeleteViewModel) result.ViewData.Model;
            Assert.Equal("This goal should be deleted", goalDeleteViewModel.TextualGoal);
            Assert.Equal(GoalType.Numeric, goalDeleteViewModel.GoalType);
            Assert.Equal(100, goalDeleteViewModel.NumericGoal);
        }

        [Fact]
        public async void TestGoalDeleteSiteAdminDisplaysConfirmationPage()
        {
            const int goalId = 567;
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalDeleteQuery>()))
                .ReturnsAsync(new GoalDeleteViewModel
                {
                    Id = goalId,
                    OwningOrganizationId = OrgAdminOrgId,
                    TextualGoal = "This goal should be deleted",
                    GoalType = GoalType.Numeric,
                    NumericGoal = 100
                })
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, true, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.Delete(goalId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Delete", result.ViewName);
            var goalDeleteViewModel = (GoalDeleteViewModel)result.ViewData.Model;
            Assert.Equal("This goal should be deleted", goalDeleteViewModel.TextualGoal);
            Assert.Equal(GoalType.Numeric, goalDeleteViewModel.GoalType);
            Assert.Equal(100, goalDeleteViewModel.NumericGoal);
        }

        #endregion

        #region DeleteConfirmed

        [Fact]
        public async void TestGoalDeleteConfirmedForWrongOrgAdminReturns401()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalDeleteQuery>()))
                .ReturnsAsync(new GoalDeleteViewModel { OwningOrganizationId = OrgAdminOrgId})
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(654));
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.DeleteConfirmed(567);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async void TestGoalDeleteConfirmedForOrgAdminForNonexistantGoalReturn404()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalDeleteQuery>()))
                .ReturnsAsync((GoalDeleteViewModel)null)
                .Verifiable();

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(OrgAdminOrgId));
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.DeleteConfirmed(987);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void TestGoalDeleteConfirmedOrgAdminRedirectToAction()
        {
            const int campaignId = 123;
            const int goalId = 567;
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalDeleteQuery>()))
                .ReturnsAsync(new GoalDeleteViewModel
                {
                    Id = goalId,
                    OwningOrganizationId = OrgAdminOrgId,
                    CampaignId = campaignId
                })
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, true, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(OrgAdminOrgId));
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.DeleteConfirmed(goalId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Details", result.ActionName);
            Assert.Equal("Campaign", result.ControllerName);
            Assert.Equal(AreaNames.Admin, result.RouteValues["area"]);
            Assert.Equal(campaignId, result.RouteValues["id"]);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<GoalDeleteCommand>()), Times.Once);
        }

        [Fact]
        public async void TestGoalDeleteConfirmedSiteAdminRedirectToAction()
        {
            const int campaignId = 123;
            const int goalId = 567;
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalDeleteQuery>()))
                .ReturnsAsync(new GoalDeleteViewModel
                {
                    Id = goalId,
                    OwningOrganizationId = OrgAdminOrgId,
                    CampaignId = campaignId
                })
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, true, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.DeleteConfirmed(goalId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Details", result.ActionName);
            Assert.Equal("Campaign", result.ControllerName);
            Assert.Equal(AreaNames.Admin, result.RouteValues["area"]);
            Assert.Equal(campaignId, result.RouteValues["id"]);
            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<GoalDeleteCommand>()), Times.Once);
        }

        #endregion

        #region Edit

        [Fact]
        public async void TestGoalEditForWrongOrgAdminReturns401()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalEditQuery>()))
                .ReturnsAsync(new GoalEditViewModel(){OrganizationId = OrgAdminOrgId})
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(654));
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.Edit(456);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async void TestGoalEditForOrgAdminForNonexistantGoalReturn404()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalEditQuery>()))
                .ReturnsAsync((GoalEditViewModel)null)
                .Verifiable();

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(OrgAdminOrgId));
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.Edit(987);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void TestGoalEditForOrgAdmin()
        {
            const int campaignId = 123;
            const int goalId = 456;
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalEditQuery>()))
                .ReturnsAsync(new GoalEditViewModel
                {
                    Id = goalId,
                    CampaignId = campaignId,
                    OrganizationId = OrgAdminOrgId
                })
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(OrgAdminOrgId));
            goalController.ControllerContext = mockContext.Object;

            // Act
            var actionResult = await goalController.Edit(goalId);

            // Assert
            Assert.IsType<ViewResult>(actionResult);
            var viewResult = (ViewResult) actionResult;

            Assert.Equal("Edit", viewResult.ViewName);
            var vm = viewResult.ViewData.Model as GoalEditViewModel;
            Assert.NotNull(vm);

            Assert.Equal(campaignId, vm.CampaignId);
            Assert.Equal(OrgAdminOrgId, vm.OrganizationId);
        }


        [Fact]
        public async void TestGoalEditForSiteAdmin()
        {
            const int campaignId = 123;
            const int goalId = 456;
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalEditQuery>()))
                .ReturnsAsync(new GoalEditViewModel
                {
                    Id = goalId,
                    CampaignId = campaignId,
                    OrganizationId = OrgAdminOrgId
                })
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            goalController.ControllerContext = mockContext.Object;

            // Act
            var actionResult = await goalController.Edit(goalId);

            // Assert
            Assert.IsType<ViewResult>(actionResult);
            var viewResult = (ViewResult) actionResult;
            Assert.Equal("Edit", viewResult.ViewName);
            var vm = viewResult.ViewData.Model as GoalEditViewModel;
            Assert.NotNull(vm);

            Assert.Equal(campaignId, vm.CampaignId);
            Assert.Equal(OrgAdminOrgId, vm.OrganizationId);
        }
        
        #endregion

        #region EditPost

        [Fact]
        public async void GoalEditPostForOrgAdminWithIncorrectIdInModelStateReturnsBadRequest()
        {
            const int campaignId = 123;
            const int goalId = 456;
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalEditQuery>()))
                .ReturnsAsync(new GoalEditViewModel
                {
                    Id = goalId,
                    CampaignId = campaignId,
                    OrganizationId = OrgAdminOrgId
                })
                .Verifiable();

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(OrgAdminOrgId));
            goalController.ControllerContext = mockContext.Object;

            var vm = new GoalEditViewModel {Id = goalId, GoalType = GoalType.Text, TextualGoal = "Aim to please"};

            // Act
            var result = await goalController.Edit(987, vm);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void GoalEditPostForOrgAdminWithValidModelStateReturnsRedirectToAction()
        {
            const int campaignId = 123;
            const int goalId = 456;
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalEditQuery>()))
                .ReturnsAsync(new GoalEditViewModel
                {
                    Id = goalId,
                    CampaignId = campaignId,
                    OrganizationId = OrgAdminOrgId
                })
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(OrgAdminOrgId));
            goalController.ControllerContext = mockContext.Object;

            var vm = new GoalEditViewModel {Id = goalId, GoalType = GoalType.Text, TextualGoal = "Aim to please"};

            // Act
            var result = await goalController.Edit(goalId, vm) as RedirectToActionResult;


            // Assert
            Assert.NotNull(result);
            Assert.Equal("Details", result.ActionName);
            Assert.Equal("Campaign", result.ControllerName);
            Assert.Equal(AreaNames.Admin, result.RouteValues["area"]);
            Assert.Equal(campaignId, result.RouteValues["id"]);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<GoalEditCommand>()), Times.Once);
        }
        
        [Fact]
        public async void TestGoalEditPostForSiteAdminWithValidModelStateReturnsRedirectToAction()
        {
            const int campaignId = 123;
            const int goalId = 456;
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalEditQuery>()))
                .ReturnsAsync(new GoalEditViewModel
                {
                    Id = goalId,
                    CampaignId = campaignId,
                    OrganizationId = OrgAdminOrgId
                })
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, true, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(SiteAdmin());
            goalController.ControllerContext = mockContext.Object;

            var vm = new GoalEditViewModel {Id = goalId, GoalType = GoalType.Text, TextualGoal = "Aim to please"};

            // Act
            var result = await goalController.Edit(goalId, vm) as RedirectToActionResult;


            // Assert
            Assert.NotNull(result);
            Assert.Equal("Details", result.ActionName);
            Assert.Equal("Campaign", result.ControllerName);
            Assert.Equal(AreaNames.Admin, result.RouteValues["area"]);
            Assert.Equal(campaignId, result.RouteValues["id"]);

            mockMediator.Verify(mock => mock.SendAsync(It.IsAny<GoalEditCommand>()), Times.Once);
        }

        [Fact]
        public async void TestGoalEditPostForWrongOrgAdminReturns401()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<GoalEditQuery>()))
                .ReturnsAsync(new GoalEditViewModel())
                .Verifiable();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<AuthorizableCampaignQuery>())).ReturnsAsync(new FakeAuthorizableCampaign(false, false, false, false));

            var goalController = new GoalController(mockMediator.Object);

            var mockContext = MockControllerContextWithUser(OrgAdmin(654));
            goalController.ControllerContext = mockContext.Object;

            // Act
            var result = await goalController.Edit(456, new GoalEditViewModel(){Id = 456});

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        #endregion

        private static ClaimsPrincipal OrgAdmin(int orgAdminOrgId)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.UserType, UserType.OrgAdmin.ToString()),
                new Claim(ClaimTypes.Organization, orgAdminOrgId.ToString())
            }));
        }

        private static ClaimsPrincipal SiteAdmin()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.UserType, nameof(UserType.SiteAdmin))
            }));
        }

        private static Mock<ControllerContext> MockControllerContextWithUser(ClaimsPrincipal principle)
        {
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(mock => mock.User).Returns(() => principle);
            var mockContext = new Mock<ControllerContext>();

            mockContext.Object.HttpContext = mockHttpContext.Object;
            return mockContext;
        }
    }
}
