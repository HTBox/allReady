using AllReady.Areas.Admin.Features.CampaignManagerInvites;
using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.CampaignManagerInvites
{
    public class AcceptCampaignManagerInviteCommandHandlerShould : InMemoryContextTest
    {
        private DateTime AcceptedTime = new DateTime(2017, 5, 29);
        private const int inviteId = 20;
        private const int campaignId = 30;
        private const string senderUserId = "40";
        private const string campaignName = "My campaign";
        private const string inviteeEmail = "test@test.com";
        private const string senderEmail = "sender@test.com";
        

        protected override void LoadTestData()
        {
            Context.Campaigns.Add(new Campaign
            {
                Id = campaignId,
                Name = campaignName
            });

            Context.Users.Add(new ApplicationUser
            {
                Id = senderUserId,
                Email = senderEmail
            });

            Context.CampaignManagerInvites.Add(new CampaignManagerInvite
            {
                Id = inviteId,
                CampaignId = campaignId,
                InviteeEmailAddress = inviteeEmail,
                SenderUserId = senderUserId
            });

            Context.SaveChanges();
        }

        [Fact]
        public async Task ShouldSetIsAcceptedToTrue()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockUrlHelper = new Mock<IUrlHelper>();
            var handler = new AcceptCampaignManagerInviteCommandHandler(Context, mockMediator.Object, mockUrlHelper.Object);
            handler.DateTimeUtcNow = () => { return AcceptedTime; };

            // Act
            await handler.Handle(new AcceptCampaignManagerInviteCommand
            {
                CampaignManagerInviteId = inviteId
            });

            // Assert
            var invite = Context.CampaignManagerInvites.SingleOrDefault(i => i.Id == inviteId);
            invite.IsAccepted.ShouldBe(true);
            invite.AcceptedDateTimeUtc.ShouldBe(AcceptedTime);
        }

        [Fact]
        public async Task ShouldSendEmailWhenAccepting()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockUrlHelper = new Mock<IUrlHelper>();
            string campaignUrl = $"http://localhost/campaigns/details/{campaignId}";
            mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(campaignUrl);
            mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(campaignUrl);
            var handler = new AcceptCampaignManagerInviteCommandHandler(Context, mockMediator.Object, mockUrlHelper.Object);
            handler.DateTimeUtcNow = () => { return AcceptedTime; };

            // Act
            await handler.Handle(new AcceptCampaignManagerInviteCommand
            {
                CampaignManagerInviteId = inviteId
            });

            // Assert
            mockMediator.Verify(x => x.PublishAsync(It.Is<CampaignManagerInviteAccepted>(c =>
                c.CampaignName == campaignName &&
                c.CampaignUrl == campaignUrl &&
                c.InviteeEmail == inviteeEmail &&
                c.SenderEmail == senderEmail
            )), Times.Once);
        }
    }
}
