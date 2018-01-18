using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.CampaignManagerInvites;
using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using AllReady.Features.Campaigns;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class CampaignManagerInviteControllerTests
    {
        private const int campaignId = 100;
        private const int eventId = 200;
        private const int inviteId = 300;

        #region Send GET Tests
        [Fact]
        public async Task SendSendsCampaignByCampaignIdQueryWithCorrectCampaignId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);

            // Act
            await sut.Send(campaignId);

            // Assert
            mockMediator.Verify(mock => mock.SendAsync(It.Is<CampaignManagerInviteQuery>(c => c.CampaignId == campaignId)));
        }

        [Fact]
        public async Task SendReturnsNotFoundResult_WhenNoCampaignMatchesId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignManagerInviteQuery>())).ReturnsAsync((CampaignManagerInviteViewModel)null);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);

            // Act
            IActionResult result = await sut.Send(campaignId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SendReturnsUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignManagerInviteQuery>())).ReturnsAsync(new CampaignManagerInviteViewModel() { CampaignId = campaignId });

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserNotAnOrgAdmin();

            // Act
            IActionResult result = await sut.Send(campaignId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendReturnsUnauthorizedResult_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new CampaignManagerInviteViewModel() { CampaignId = campaignId, OrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignManagerInviteQuery>())).ReturnsAsync(campaign);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "2");

            // Act
            IActionResult result = await sut.Send(campaignId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendReturnsSendView_WhenUserIsOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new CampaignManagerInviteViewModel() { CampaignId = campaignId, OrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignManagerInviteQuery>())).ReturnsAsync(campaign);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "1");

            // Act
            IActionResult result = await sut.Send(campaignId);

            // Assert
            Assert.IsType<ViewResult>(result);
            ViewResult view = result as ViewResult;
            Assert.Equal("Send", view.ViewName);
        }

        [Fact]
        public async Task SendPassesCorrectViewModelToView()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new CampaignManagerInviteViewModel() { CampaignId = campaignId, OrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignManagerInviteQuery>())).ReturnsAsync(campaign);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "1");

            // Act
            IActionResult result = await sut.Send(campaignId);

            // Assert
            ViewResult view = result as ViewResult;
            var model = Assert.IsType<CampaignManagerInviteViewModel>(view.ViewData.Model);
            Assert.Equal(campaignId, model.CampaignId);
        }

        #endregion

        #region Send POST Tests
        [Fact]
        public async Task SendReturnsBadRequestResult_WhenViewModelIsNull()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);

            // Act
            IActionResult result = await sut.Send(campaignId, null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task SendShouldNotCreateInvite_WhenViewModelIsNull()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);

            // Act
            IActionResult result = await sut.Send(campaignId, null);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendReturnsSendView_WhenModelStateIsNotValid()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.ModelState.AddModelError("Error", "Message");

            // Act
            var result = await sut.Send(campaignId, new CampaignManagerInviteViewModel());

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Send", view.ViewName);
        }

        [Fact]
        public async Task SendShouldNotCreateInvite_WhenModelStateIsNotValid()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.ModelState.AddModelError("Error", "Message");

            // Act
            var result = await sut.Send(campaignId, new CampaignManagerInviteViewModel());

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendPostReturnsUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserNotAnOrgAdmin();
            var invite = new CampaignManagerInviteViewModel();

            // Act
            IActionResult result = await sut.Send(campaignId, invite);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendShouldNotCreateInvite_WhenUserIsNotOrgAdmin()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserNotAnOrgAdmin();
            var invite = new CampaignManagerInviteViewModel();

            // Act
            IActionResult result = await sut.Send(campaignId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendPostReturnsUnauthorizedResult_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "2");
            var invite = new CampaignManagerInviteViewModel();

            // Act
            IActionResult result = await sut.Send(campaignId, invite);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendShouldNotCreateInvite_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "2");
            var invite = new CampaignManagerInviteViewModel();

            // Act
            IActionResult result = await sut.Send(campaignId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendShouldNotCreateInvite_WhenAnInviteAlreadyExistsForInviteeEmailAddress()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<UserHasCampaignManagerInviteQuery>())).ReturnsAsync(true);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "1");
            var invite = new CampaignManagerInviteViewModel
            {
                CampaignId = 1,
                InviteeEmailAddress = "test@test.com",
                CustomMessage = "test message"
            };

            // Act
            IActionResult result = await sut.Send(invite.CampaignId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.Is<CreateCampaignManagerInviteCommand>(c => c.Invite == invite)), Times.Never);
        }

        [Fact]
        public async Task SendShouldNotCreateInvite_WhenUserIsAllreadyManagerForEvent()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<UserHasCampaignManagerInviteQuery>())).ReturnsAsync(false);

            var mockUserManager = UserManagerMockHelper.CreateUserManagerMock();
            mockUserManager.Setup(mock => mock.FindByEmailAsync(It.Is<string>(e => e == "test@test.com"))).ReturnsAsync(new ApplicationUser
            {
                ManagedCampaigns = new List<CampaignManager> { new CampaignManager() { CampaignId = 1 } },
            });

            var sut = new CampaignManagerInviteController(mockMediator.Object, mockUserManager.Object);
            sut.MakeUserAnOrgAdmin(organizationId: "1");
            var invite = new CampaignManagerInviteViewModel
            {
                CampaignId = 1,
                InviteeEmailAddress = "test@test.com",
                CustomMessage = "test message"
            };

            // Act
            IActionResult result = await sut.Send(invite.CampaignId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.Is<CreateCampaignManagerInviteCommand>(c => c.Invite == invite)), Times.Never);
        }


        [Fact]
        public async Task SendShouldCreateInvite_WhenUserIsOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser());

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(It.IsAny<string>());

            var sut = new CampaignManagerInviteController(mockMediator.Object, userManager.Object) { Url = urlHelper.Object };
            sut.MakeUserAnOrgAdmin(organizationId: "1");
            var invite = new CampaignManagerInviteViewModel
            {
                CampaignId = 1,
                InviteeEmailAddress = "test@test.com",
                CustomMessage = "test message"
            };

            // Act
            IActionResult result = await sut.Send(campaignId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.Is<CreateCampaignManagerInviteCommand>(c => c.Invite == invite)), Times.Once);
        }

        #endregion

        #region Details Tests
        [Fact]
        public async Task DetailsSendsCampaignManagerInviteDetailQueryWithCorrectInviteId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);

            // Act
            await sut.Details(inviteId);

            // Assert
            mockMediator.Verify(mock => mock.SendAsync(It.Is<CampaignManagerInviteDetailQuery>(c => c.CampaignManagerInviteId == inviteId)));
        }

        [Fact]
        public async Task DetailsReturnsNotFoundResult_WhenNoInviteMatchesId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);

            // Act
            IActionResult result = await sut.Details(inviteId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsReturnsUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignManagerInviteDetailQuery>()))
                .ReturnsAsync(new CampaignManagerInviteDetailsViewModel { Id = inviteId });

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserNotAnOrgAdmin();

            // Act
            IActionResult result = await sut.Details(inviteId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DetailsReturnsUnauthorizedResult_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var invite = new CampaignManagerInviteDetailsViewModel() { Id = inviteId, OrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignManagerInviteDetailQuery>())).ReturnsAsync(invite);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "2");

            // Act
            IActionResult result = await sut.Details(inviteId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DetailsReturnsView_WhenUserIsOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var invite = new CampaignManagerInviteDetailsViewModel() { Id = inviteId, OrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignManagerInviteDetailQuery>())).ReturnsAsync(invite);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "1");

            // Act
            IActionResult result = await sut.Details(inviteId);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task DetailsPassesCorrectViewModelToView()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var invite = new CampaignManagerInviteDetailsViewModel() { Id = inviteId, OrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignManagerInviteDetailQuery>())).ReturnsAsync(invite);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "1");

            // Act
            IActionResult result = await sut.Details(inviteId);

            // Assert
            ViewResult view = result as ViewResult;
            var model = Assert.IsType<CampaignManagerInviteDetailsViewModel>(view.ViewData.Model);
            Assert.Equal(inviteId, model.Id);
        }

        #endregion

        #region CancelInvice tests
        [Fact]
        public async Task CancelReturnsUnauthorizedResult_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new CampaignManagerInviteDetailsViewModel() { CampaignId = campaignId, OrganizationId = 1, Id = 5};
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignManagerInviteDetailQuery>())).ReturnsAsync(campaign);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserNotAnOrgAdmin();

            // Act
            var result = await sut.Cancel(5);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task CancelReturnsNotFoundResult_WhenInviteIsMissing()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin("1");

            // Act
            var result = await sut.Cancel(5);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CancelReturnsRedirectResult_WhenInviteIsOk()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new CampaignManagerInviteDetailsViewModel() { CampaignId = campaignId, OrganizationId = 1, Id = 5 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignManagerInviteDetailQuery>())).ReturnsAsync(campaign);

            var sut = new CampaignManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin("1");

            // Act
            var result = await sut.Cancel(5);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        #endregion
    }
}
