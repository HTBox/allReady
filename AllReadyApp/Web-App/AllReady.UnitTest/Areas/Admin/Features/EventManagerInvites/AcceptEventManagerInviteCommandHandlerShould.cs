using AllReady.Areas.Admin.Features.EventManagerInvites;
using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Moq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using AllReady.Areas.Admin.Features.Notifications;

namespace AllReady.UnitTest.Areas.Admin.Features.EventManagerInvites
{
    public class AcceptEventManagerInviteCommandHandlerShould : InMemoryContextTest
    {
        private DateTime AcceptedTime = new DateTime(2017, 5, 29);
        private const int inviteId = 20;
        private const int eventId = 10;
        private const int campaignId = 30;
        private const string senderUserId = "40";
        private const string eventUrl = "http://localhost/event/{eventId}";
        private const string eventName = "My Event";
        private const string campaignName = "My campaign";
        private const string inviteeEmail = "invitee@test.com";
        private const string senderEmail = "sender@test.com";


        protected override void LoadTestData()
        {
            Context.EventManagerInvites.Add(new EventManagerInvite
            {
                Id = inviteId,
                EventId = eventId,
                InviteeEmailAddress = inviteeEmail,
                SenderUserId = senderUserId
            });

            Context.Events.Add(new Event
            {
                Id = eventId,
                Name = eventName,
                CampaignId = campaignId
            });

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

            Context.SaveChanges();
        }

        [Fact]
        public async Task ShouldSetIsAcceptedToTrue()
        {
            // Arrange
            var handler = new AcceptEventManagerInviteCommandHandler(Context, Mock.Of<IMediator>(), Mock.Of<IUrlHelper>());
            handler.DateTimeUtcNow = () => { return AcceptedTime; };

            // Act
            await handler.Handle(new AcceptEventManagerInviteCommand
            {
                EventManagerInviteId = inviteId
            });

            // Assert
            var invite = Context.EventManagerInvites.SingleOrDefault(i => i.Id == inviteId);
            invite.IsAccepted.ShouldBe(true);
            invite.AcceptedDateTimeUtc.ShouldBe(AcceptedTime);
        }

        [Fact]
        public async Task ShouldSendEmailWhenAccepting()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockUrlHelper = new Mock<IUrlHelper>();

            mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(eventUrl);
            mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(eventUrl);
            var handler = new AcceptEventManagerInviteCommandHandler(Context, mockMediator.Object, mockUrlHelper.Object);
            handler.DateTimeUtcNow = () => { return AcceptedTime; };

            // Act
            await handler.Handle(new AcceptEventManagerInviteCommand
            {
                EventManagerInviteId = inviteId
            });

            // Assert
            mockMediator.Verify(x => x.PublishAsync(It.Is<EventManagerInviteAccepted>(c =>
                c.EventName == eventName &&
                c.CampaignName == campaignName &&
                c.EventUrl == eventUrl &&
                c.InviteeEmail == inviteeEmail &&
                c.SenderEmail == senderEmail
            )), Times.Once);
        }
    }
}
