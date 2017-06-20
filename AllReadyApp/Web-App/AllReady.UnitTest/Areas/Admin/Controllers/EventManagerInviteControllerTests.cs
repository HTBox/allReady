using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.EventManagerInvites;
using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using AllReady.Features.Events;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class EventManagerInviteControllerTests
    {
        private const int campaignId = 100;
        private const int eventId = 200;
        private const int inviteId = 300;

        #region Send GET Tests
        [Fact]
        public async Task SendEventByEventIdQueryWithCorrectEventId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);

            // Act
            await sut.Send(eventId);

            // Assert
            mockMediator.Verify(mock => mock.SendAsync(It.Is<EventManagerInviteQuery>(e => e.EventId == eventId)));
        }

        [Fact]
        public async Task SendReturnsNotFoundResult_WhenNoEventMatchesId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventManagerInviteQuery>())).ReturnsAsync(null);

            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);

            // Act
            IActionResult result = await sut.Send(eventId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SendReturnsUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var viewModel = new EventManagerInviteViewModel()
            {
                OrganizationId = 1
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventManagerInviteQuery>())).ReturnsAsync(viewModel);

            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserNotAnOrgAdmin();

            // Act
            IActionResult result = await sut.Send(eventId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendReturnsUnauthorizedResult_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var viewModel = new EventManagerInviteViewModel
            {
                OrganizationId = 1
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventManagerInviteQuery>())).ReturnsAsync(viewModel);

            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "2");

            // Act
            IActionResult result = await sut.Send(eventId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendReturnsSendView_WhenUserIsOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var viewModel = new EventManagerInviteViewModel
            {
                OrganizationId = 1
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventManagerInviteQuery>())).ReturnsAsync(viewModel);
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser());

            var sut = new EventManagerInviteController(mockMediator.Object, userManager.Object);
            sut.MakeUserAnOrgAdmin(organizationId: "1");

            // Act
            IActionResult result = await sut.Send(eventId);

            // Assert
            Assert.IsType<ViewResult>(result);
            ViewResult view = result as ViewResult;
        }

        [Fact]
        public async Task SendPassesCorrectViewModelToView()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var viewModel = new EventManagerInviteViewModel
            {
                EventId = eventId,
                EventName = "TestEvent",
                CampaignId = campaignId,
                CampaignName = "TestCampaign",
                OrganizationId = 1
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventManagerInviteQuery>())).ReturnsAsync(viewModel);

            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "1");

            // Act
            IActionResult result = await sut.Send(eventId);

            // Assert
            ViewResult view = result as ViewResult;
            var model = Assert.IsType<EventManagerInviteViewModel>(view.ViewData.Model);
            Assert.Equal(eventId, model.EventId);
            Assert.Equal(campaignId, model.CampaignId);
            Assert.Equal("TestCampaign", model.CampaignName);
            Assert.Equal("TestEvent", model.EventName);
        }

        #endregion

        #region Send POST Tests

        [Fact]
        public async Task SendReturnsBadRequestResult_WhenViewModelIsNull()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);

            // Act
            IActionResult result = await sut.Send(eventId, null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task SendShouldNotCreateInvite_WhenViewModelIsNull()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);

            // Act
            IActionResult result = await sut.Send(eventId, null);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateEventManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendReturnsSendView_WhenModelStateIsNotValid()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.ModelState.AddModelError("Error", "Message");

            // Act
            var result = await sut.Send(eventId, new EventManagerInviteViewModel());

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Send", view.ViewName);
        }

        [Fact]
        public async Task SendShouldNotCreateInvite_WhenModelStateIsNotValid()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.ModelState.AddModelError("Error", "Message");

            // Act
            var result = await sut.Send(eventId, new EventManagerInviteViewModel());

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateEventManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendPostReturnsUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event
            {
                Campaign = new Campaign() { ManagingOrganizationId = 1 }
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserNotAnOrgAdmin();
            var invite = new EventManagerInviteViewModel();

            // Act
            IActionResult result = await sut.Send(eventId, invite);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendShouldNotCreateInvite_WhenUserIsNotOrgAdmin()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event
            {
                Campaign = new Campaign() { ManagingOrganizationId = 1 }
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserNotAnOrgAdmin();
            var invite = new EventManagerInviteViewModel();

            // Act
            IActionResult result = await sut.Send(eventId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateEventManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendPostReturnsUnauthorizedResult_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event
            {
                Campaign = new Campaign() { ManagingOrganizationId = 1 }
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "2");
            var invite = new EventManagerInviteViewModel();

            // Act
            IActionResult result = await sut.Send(eventId, invite);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendShouldNotCreateInvite_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event
            {
                Campaign = new Campaign() { ManagingOrganizationId = 1 }
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "2");
            var invite = new EventManagerInviteViewModel();

            // Act
            IActionResult result = await sut.Send(eventId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateEventManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendShouldNotCreateInvite_WhenAnInviteAlreadyExistsForInviteeEmailAddress()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event
            {
                Campaign = new Campaign() { ManagingOrganizationId = 1 }
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<UserHasEventManagerInviteQuery>())).ReturnsAsync(true);

            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "1");
            var invite = new EventManagerInviteViewModel
            {
                EventId = 1,
                InviteeEmailAddress = "test@test.com",
                CustomMessage = "test message"
            };

            // Act
            IActionResult result = await sut.Send(invite.EventId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.Is<CreateEventManagerInviteCommand>(c => c.Invite == invite)), Times.Never);
        }

        [Fact]
        public async Task SendShouldNotCreateInvite_WhenUserIsAllreadyManagerForEvent()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event
            {
                Campaign = new Campaign() { ManagingOrganizationId = 1 }
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<UserHasEventManagerInviteQuery>())).ReturnsAsync(false);

            var mockUserManger = UserManagerMockHelper.CreateUserManagerMock();
            mockUserManger.Setup(mock => mock.FindByEmailAsync(It.Is<string>(e => e == "test@test.com"))).ReturnsAsync(new ApplicationUser
            {
                ManagedEvents = new List<EventManager> { new EventManager() { EventId = 1 } },
            });

            var sut = new EventManagerInviteController(mockMediator.Object, mockUserManger.Object);
            sut.MakeUserAnOrgAdmin(organizationId: "1");
            var invite = new EventManagerInviteViewModel
            {
                EventId = 1,
                InviteeEmailAddress = "test@test.com",
                CustomMessage = "test message"
            };

            // Act
            IActionResult result = await sut.Send(invite.EventId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.Is<CreateEventManagerInviteCommand>(c => c.Invite == invite)), Times.Never);
        }

        [Fact]
        public async Task SendShouldCreateInvite_WhenUserIsOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event
            {
                Campaign = new Campaign() { ManagingOrganizationId = 1 }
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);
            var userManager = UserManagerMockHelper.CreateUserManagerMock();
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser());

            var sut = new EventManagerInviteController(mockMediator.Object, userManager.Object);
            sut.MakeUserAnOrgAdmin(organizationId: "1");
            var invite = new EventManagerInviteViewModel
            {
                EventId = 1,
                InviteeEmailAddress = "test@test.com",
                CustomMessage = "test message"
            };

            // Act
            IActionResult result = await sut.Send(invite.EventId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.Is<CreateEventManagerInviteCommand>(c => c.Invite == invite)), Times.Once);
        }
        #endregion

        #region Details Tests
        [Fact]
        public async Task DetailsSendsEventManagerInviteDetailQueryWithCorrectInviteId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);

            // Act
            await sut.Details(inviteId);

            // Assert
            mockMediator.Verify(mock => mock.SendAsync(It.Is<EventManagerInviteDetailQuery>(c => c.EventManagerInviteId == inviteId)));
        }

        [Fact]
        public async Task DetailsReturnsNotFoundResult_WhenNoInviteMatchesId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);

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
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventManagerInviteDetailQuery>()))
                .ReturnsAsync(new EventManagerInviteDetailsViewModel { Id = inviteId });

            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
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
            var invite = new EventManagerInviteDetailsViewModel() { Id = inviteId, OrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventManagerInviteDetailQuery>())).ReturnsAsync(invite);

            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
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
            var invite = new EventManagerInviteDetailsViewModel() { Id = inviteId, OrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventManagerInviteDetailQuery>())).ReturnsAsync(invite);

            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
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
            var invite = new EventManagerInviteDetailsViewModel() { Id = inviteId, OrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventManagerInviteDetailQuery>())).ReturnsAsync(invite);

            var sut = new EventManagerInviteController(mockMediator.Object, UserManagerMockHelper.CreateUserManagerMock().Object);
            sut.MakeUserAnOrgAdmin(organizationId: "1");

            // Act
            IActionResult result = await sut.Details(inviteId);

            // Assert
            ViewResult view = result as ViewResult;
            var model = Assert.IsType<EventManagerInviteDetailsViewModel>(view.ViewData.Model);
            Assert.Equal(inviteId, model.Id);
        }

        #endregion
    }
}
