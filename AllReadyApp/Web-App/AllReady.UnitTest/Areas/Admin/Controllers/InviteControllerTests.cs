using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Invite;
using AllReady.Areas.Admin.ViewModels.Invite;
using AllReady.Features.Campaigns;
using AllReady.Features.Events;
using AllReady.Models;
using AllReady.UnitTest.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class InviteControllerTests
    {
        private const int campaignId = 100;
        private const int eventId = 200;

        #region SendCampaignManagerInvite GET Tests
        [Fact]
        public async Task SendCampaignManagerInviteSendsCampaignByCampaignIdQueryWithCorrectCampaignId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());

            // Act
            await sut.SendCampaignManagerInvite(campaignId);

            // Assert
            mockMediator.Verify(mock => mock.SendAsync(It.Is<CampaignByCampaignIdQuery>(c => c.CampaignId == campaignId)));
        }

        [Fact]
        public async Task SendCampaignManagerInviteReturnsNotFoundResult_WhenNoCampaignMatchesId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(null);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());

            // Act
            IActionResult result = await sut.SendCampaignManagerInvite(campaignId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SendCampaignManagerInviteReturnsUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(new Campaign());

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserNotAnOrgAdmin();

            // Act
            IActionResult result = await sut.SendCampaignManagerInvite(campaignId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendCampaignManagerInviteReturnsUnauthorizedResult_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(organizationId: "2");

            // Act
            IActionResult result = await sut.SendCampaignManagerInvite(campaignId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendCampaignManagerInviteReturnsSendView_WhenUserIsOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(organizationId: "1");

            // Act
            IActionResult result = await sut.SendCampaignManagerInvite(campaignId);

            // Assert
            Assert.IsType<ViewResult>(result);
            ViewResult view = result as ViewResult;
            Assert.Equal(view.ViewName, "Send");
        }

        [Fact]
        public async Task SendCampaignManagerInviteReturnsPassesCorrectViewModelToView()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(organizationId: "1");

            // Act
            IActionResult result = await sut.SendCampaignManagerInvite(campaignId);

            // Assert
            ViewResult view = result as ViewResult;
            var model = Assert.IsType<InviteViewModel>(view.ViewData.Model);
            Assert.Equal(campaignId, model.CampaignId);
            Assert.Equal("SendCampaignManagerInvite", model.FormAction);
            Assert.Equal("Send Campaign Manager Invite", model.Title);
        }

        #endregion

        #region SendEventManagerInvite GET Tests
        [Fact]
        public async Task SendEventManagerInviteSendsEventByEventIdQueryWithCorrectEventId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());

            // Act
            await sut.SendEventManagerInvite(eventId);

            // Assert
            mockMediator.Verify(mock => mock.SendAsync(It.Is<EventByEventIdQuery>(e => e.EventId == eventId)));
        }

        [Fact]
        public async Task SendEventManagerInviteReturnsNotFoundResult_WhenNoEventMatchesId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(null);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());

            // Act
            IActionResult result = await sut.SendEventManagerInvite(eventId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SendEventManagerInviteReturnsUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event();
            @event.Campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserNotAnOrgAdmin();

            // Act
            IActionResult result = await sut.SendEventManagerInvite(eventId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendEventManagerInviteReturnsUnauthorizedResult_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event();
            @event.Campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(organizationId: "2");

            // Act
            IActionResult result = await sut.SendEventManagerInvite(eventId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendEventManagerInviteReturnsSendView_WhenUserIsOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event();
            @event.Campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(organizationId: "1");

            // Act
            IActionResult result = await sut.SendEventManagerInvite(eventId);

            // Assert
            Assert.IsType<ViewResult>(result);
            ViewResult view = result as ViewResult;
            Assert.Equal(view.ViewName, "Send");
        }

        [Fact]
        public async Task SendEventManagerInviteReturnsPassesCorrectViewModelToView()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event();
            @event.Campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(organizationId: "1");

            // Act
            IActionResult result = await sut.SendEventManagerInvite(eventId);

            // Assert
            ViewResult view = result as ViewResult;
            var model = Assert.IsType<InviteViewModel>(view.ViewData.Model);
            Assert.Equal(eventId, model.EventId);
            Assert.Equal("Send Event Manager Invite", model.Title);
            Assert.Equal("SendEventManagerInvite", model.FormAction);
        }

        #endregion

        #region SendCampaignManagerInvite POST Tests
        [Fact]
        public async Task SendCampaignManagerInviteReturnsBadRequestResult_WhenViewModelIsNull()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());

            // Act
            IActionResult result = await sut.SendCampaignManagerInvite(campaignId, null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task SendCampaignManagerInviteShouldNotCreateInvite_WhenViewModelIsNull()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());

            // Act
            IActionResult result = await sut.SendCampaignManagerInvite(campaignId, null);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendCampaignManagerInviteReturnsSendView_WhenModelStateIsNotValid()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.ModelState.AddModelError("Error", "Message");

            // Act
            var result = await sut.SendCampaignManagerInvite(campaignId, new InviteViewModel());

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Send", view.ViewName);
        }

        [Fact]
        public async Task SendCampaignManagerInviteShouldNotCreateInvite_WhenModelStateIsNotValid()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.ModelState.AddModelError("Error", "Message");

            // Act
            var result = await sut.SendCampaignManagerInvite(campaignId, new InviteViewModel());

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendCampaignManagerInviteReturnsBadRequestResult_WhenNoCampaignMatchesId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(null);
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            var invite = new InviteViewModel();

            // Act
            var result = await sut.SendCampaignManagerInvite(campaignId, new InviteViewModel());

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task SendCampaignManagerInviteShouldNotCreateInvite_WhenNoCampaignMatchesId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(null);
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            var invite = new InviteViewModel();

            // Act
            var result = await sut.SendCampaignManagerInvite(campaignId, new InviteViewModel());

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendCampaignManagerInvitePostReturnsUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserNotAnOrgAdmin();
            var invite = new InviteViewModel();

            // Act
            IActionResult result = await sut.SendCampaignManagerInvite(campaignId, invite);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendCampaignManagerInviteShouldNotCreateInvite_WhenUserIsNotOrgAdmin()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserNotAnOrgAdmin();
            var invite = new InviteViewModel();

            // Act
            IActionResult result = await sut.SendCampaignManagerInvite(campaignId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendCampaignManagerInvitePostReturnsUnauthorizedResult_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(organizationId: "2");
            var invite = new InviteViewModel();

            // Act
            IActionResult result = await sut.SendCampaignManagerInvite(campaignId, invite);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendCampaignManagerInviteShouldNotCreateInvite_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(organizationId: "2");
            var invite = new InviteViewModel();

            // Act
            IActionResult result = await sut.SendCampaignManagerInvite(campaignId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendCampaignManagerInviteShouldCreateInvite_WhenUserIsOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var campaign = new Campaign() { ManagingOrganizationId = 1 };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<CampaignByCampaignIdQuery>())).ReturnsAsync(campaign);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(organizationId: "1");
            var invite = new InviteViewModel
            {
                CampaignId = 1,
                InviteeEmailAddress = "test@test.com",
                CustomMessage = "test message"
            };

            // Act
            IActionResult result = await sut.SendCampaignManagerInvite(campaignId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.Is<CreateCampaignManagerInviteCommand>(c => c.Invite == invite)), Times.Once);
        }

        #endregion

        #region SendEventManagerInvite POST Tests

        [Fact]
        public async Task SendEventManagerInviteReturnsBadRequestResult_WhenViewModelIsNull()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());

            // Act
            IActionResult result = await sut.SendEventManagerInvite(eventId, null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task SendEventManagerInviteShouldNotCreateInvite_WhenViewModelIsNull()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());

            // Act
            IActionResult result = await sut.SendEventManagerInvite(eventId, null);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendEventManagerInviteReturnsSendView_WhenModelStateIsNotValid()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.ModelState.AddModelError("Error", "Message");

            // Act
            var result = await sut.SendEventManagerInvite(eventId, new InviteViewModel());

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Send", view.ViewName);
        }

        [Fact]
        public async Task SendEventManagerInviteShouldNotCreateInvite_WhenModelStateIsNotValid()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.ModelState.AddModelError("Error", "Message");

            // Act
            var result = await sut.SendEventManagerInvite(eventId, new InviteViewModel());

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendEventManagerInviteReturnsBadRequestResult_WhenNoCampaignMatchesId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(null);
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            var invite = new InviteViewModel();

            // Act
            var result = await sut.SendEventManagerInvite(eventId, invite);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task SendEventManagerInviteShouldNotCreateInvite_WhenNoCampaignMatchesId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(x => x.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(null);
            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            var invite = new InviteViewModel();

            // Act
            var result = await sut.SendCampaignManagerInvite(eventId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendEventManagerInvitePostReturnsUnauthorizedResult_WhenUserIsNotOrgAdmin()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event
            {
                Campaign = new Campaign() { ManagingOrganizationId = 1 }
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserNotAnOrgAdmin();
            var invite = new InviteViewModel();

            // Act
            IActionResult result = await sut.SendEventManagerInvite(eventId, invite);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendEventManagerInviteShouldNotCreateInvite_WhenUserIsNotOrgAdmin()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event
            {
                Campaign = new Campaign() { ManagingOrganizationId = 1 }
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserNotAnOrgAdmin();
            var invite = new InviteViewModel();

            // Act
            IActionResult result = await sut.SendEventManagerInvite(eventId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendEventManagerInvitePostReturnsUnauthorizedResult_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event
            {
                Campaign = new Campaign() { ManagingOrganizationId = 1 }
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(organizationId: "2");
            var invite = new InviteViewModel();

            // Act
            IActionResult result = await sut.SendEventManagerInvite(eventId, invite);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SendEventManagerInviteShouldNotCreateInvite_WhenUserIsNotOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event
            {
                Campaign = new Campaign() { ManagingOrganizationId = 1 }
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(organizationId: "2");
            var invite = new InviteViewModel();

            // Act
            IActionResult result = await sut.SendEventManagerInvite(eventId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.IsAny<CreateCampaignManagerInviteCommand>()), Times.Never);
        }

        [Fact]
        public async Task SendEventManagerInviteShouldCreateInvite_WhenUserIsOrgAdminForCampaign()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var @event = new Event
            {
                Campaign = new Campaign() { ManagingOrganizationId = 1 }
            };
            mockMediator.Setup(mock => mock.SendAsync(It.IsAny<EventByEventIdQuery>())).ReturnsAsync(@event);

            var sut = new InviteController(mockMediator.Object, new FakeUserManager());
            sut.MakeUserAnOrgAdmin(organizationId: "1");
            var invite = new InviteViewModel
            {
                EventId = 1,
                InviteeEmailAddress = "test@test.com",
                CustomMessage = "test message"
            };

            // Act
            IActionResult result = await sut.SendEventManagerInvite(invite.EventId, invite);

            // Assert
            mockMediator.Verify(x => x.SendAsync(It.Is<CreateEventManagerInviteCommand>(c => c.Invite == invite)), Times.Once);
        }
        #endregion

        private class FakeUserManager : UserManager<ApplicationUser>
        {
            public FakeUserManager()
                : base(new Mock<IUserStore<ApplicationUser>>().Object,
                      new Mock<IOptions<IdentityOptions>>().Object,
                      new Mock<IPasswordHasher<ApplicationUser>>().Object,
                      new IUserValidator<ApplicationUser>[0],
                      new IPasswordValidator<ApplicationUser>[0],
                      new Mock<ILookupNormalizer>().Object,
                      new Mock<IdentityErrorDescriber>().Object,
                      new Mock<IServiceProvider>().Object,
                      new Mock<ILogger<UserManager<ApplicationUser>>>().Object)
            { }

            public int FindByEmailAsyncCallCount { get; private set; }

            public override Task<ApplicationUser> FindByEmailAsync(string email)
            {
                FindByEmailAsyncCallCount += 1;
                return Task.FromResult(new ApplicationUser { Id = "123", Email = email });
            }
        }
    }
}
