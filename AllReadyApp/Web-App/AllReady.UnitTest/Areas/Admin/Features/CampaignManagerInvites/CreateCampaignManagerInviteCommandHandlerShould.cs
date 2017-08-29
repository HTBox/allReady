using AllReady.Areas.Admin.Features.CampaignManagerInvites;
using AllReady.Areas.Admin.Features.Notifications;
using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.CampaignManagerInvites
{
    public class CreateCampaignManagerInviteCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task CreateCampaignManagerInvite()
        {
            var mockMediator = new Mock<IMediator>();
            var urlHelper = new Mock<IUrlHelper>();
            var handler = new CreateCampaignManagerInviteCommandHandler(Context, mockMediator.Object, urlHelper.Object);
            var invite = new CampaignManagerInviteViewModel
            {
                CustomMessage = "message",
                CampaignId = 1,
                InviteeEmailAddress = "test@test.com",
            };

            var inviteCommand = new CreateCampaignManagerInviteCommand
            {
                Invite = invite,
                UserId = "userId"
            };

            handler.DateTimeUtcNow = () => new DateTime(2016, 5, 29);

            await handler.Handle(inviteCommand);

            Context.EventManagerInvites.Count().ShouldBe(0);
            Context.CampaignManagerInvites.Count().ShouldBe(1);
            Context.CampaignManagerInvites.SingleOrDefault().AcceptedDateTimeUtc.ShouldBe(null);
            Context.CampaignManagerInvites.SingleOrDefault().CustomMessage.ShouldBe("message");
            Context.CampaignManagerInvites.SingleOrDefault().CampaignId.ShouldBe(1);
            Context.CampaignManagerInvites.SingleOrDefault().InviteeEmailAddress.ShouldBe("test@test.com");
            Context.CampaignManagerInvites.SingleOrDefault().RejectedDateTimeUtc.ShouldBe(null);
            Context.CampaignManagerInvites.SingleOrDefault().RevokedDateTimeUtc.ShouldBe(null);
            Context.CampaignManagerInvites.SingleOrDefault().SenderUserId.ShouldBe("userId");
            Context.CampaignManagerInvites.SingleOrDefault().SentDateTimeUtc.ShouldBe(new DateTime(2016, 5, 29));
        }

        [Fact]
        public async Task ShouldSendCampaignManagerInviteEmail()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Link("CampaignManagerInviteAcceptRoute", It.IsAny<object>())).Returns("http://accept.com/");
            urlHelper.Setup(x => x.Link("CampaignManagerInviteDeclineRoute", It.IsAny<object>())).Returns("http://decline.com/");

            var handler = new CreateCampaignManagerInviteCommandHandler(Context, mockMediator.Object, urlHelper.Object);
            var invite = new CampaignManagerInviteViewModel
            {
                CustomMessage = "test message",
                CampaignId = 1,
                InviteeEmailAddress = "test@test.com",
                CampaignName = "Test Campaign",
                
            };

            var inviteCommand = new CreateCampaignManagerInviteCommand
            {
                Invite = invite,
                UserId = "userId",
                SenderName = "John Smith",
                RegisterUrl = "http://register.com/",
                IsInviteeRegistered = true
            };

            handler.DateTimeUtcNow = () => new DateTime(2016, 5, 29);

            // Act
            await handler.Handle(inviteCommand);

            // Assert
            mockMediator.Verify(x => x.PublishAsync(It.Is<CampaignManagerInvited>(i =>
                i.InviteeEmail == invite.InviteeEmailAddress &&
                i.CampaignName == invite.CampaignName &&
                i.SenderName == "John Smith" &&
                i.AcceptUrl == "http://accept.com/" &&
                i.DeclineUrl == "http://decline.com/" &&
                i.RegisterUrl == "http://register.com/" &&
                i.IsInviteeRegistered &&
                i.Message == "test message"
            )), Times.Once);
        }
    }
}
