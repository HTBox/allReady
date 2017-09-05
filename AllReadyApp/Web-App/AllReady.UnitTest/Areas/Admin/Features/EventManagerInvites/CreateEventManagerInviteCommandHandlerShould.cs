using AllReady.Areas.Admin.Features.EventManagerInvites;
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

namespace AllReady.UnitTest.Areas.Admin.Features.EventManagerInvites
{
    public class CreateEventManagerInviteCommandHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task CreateEventManagerInvite()
        {
            var mockMediator = new Mock<IMediator>();
            var urlHelper = new Mock<IUrlHelper>();
            var handler = new CreateEventManagerInviteCommandHandler(Context, mockMediator.Object, urlHelper.Object);
            var invite = new EventManagerInviteViewModel
            {
                CustomMessage = "message",
                EventId = 5,
                InviteeEmailAddress = "test@test.com",
                Id = 5
            };

            var inviteCommand = new CreateEventManagerInviteCommand
            {
                Invite = invite,
                UserId = "userId"
            };

            handler.DateTimeUtcNow = () => new DateTime(2016, 5, 29);

            await handler.Handle(inviteCommand);

            Context.CampaignManagerInvites.Count().ShouldBe(0);
            Context.EventManagerInvites.Count().ShouldBe(1);
            Context.EventManagerInvites.SingleOrDefault().AcceptedDateTimeUtc.ShouldBe(null);
            Context.EventManagerInvites.SingleOrDefault().CustomMessage.ShouldBe("message");
            Context.EventManagerInvites.SingleOrDefault().EventId.ShouldBe(5);
            Context.EventManagerInvites.SingleOrDefault().InviteeEmailAddress.ShouldBe("test@test.com");
            Context.EventManagerInvites.SingleOrDefault().RejectedDateTimeUtc.ShouldBe(null);
            Context.EventManagerInvites.SingleOrDefault().RevokedDateTimeUtc.ShouldBe(null);
            Context.EventManagerInvites.SingleOrDefault().SenderUserId.ShouldBe("userId");
            Context.EventManagerInvites.SingleOrDefault().SentDateTimeUtc.ShouldBe(new DateTime(2016, 5, 29));
        }

        [Fact]
        public async Task ShouldSendEventManagerInviteEmail()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Link("EventManagerInviteAcceptRoute", It.IsAny<object>())).Returns("http://accept.com/");
            urlHelper.Setup(x => x.Link("EventManagerInviteDeclineRoute", It.IsAny<object>())).Returns("http://decline.com/");

            var handler = new CreateEventManagerInviteCommandHandler(Context, mockMediator.Object, urlHelper.Object);
            var invite = new EventManagerInviteViewModel
            {
                CustomMessage = "test message",
                CampaignId = 1,
                InviteeEmailAddress = "test@test.com",
                EventName = "Test Event",
            };

            var inviteCommand = new CreateEventManagerInviteCommand
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
            mockMediator.Verify(x => x.PublishAsync(It.Is<EventManagerInvited>(i =>
                i.InviteeEmail == invite.InviteeEmailAddress &&
                i.EventName == invite.EventName &&
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
