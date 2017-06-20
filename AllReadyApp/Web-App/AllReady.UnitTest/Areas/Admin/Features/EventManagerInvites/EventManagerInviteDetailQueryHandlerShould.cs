using AllReady.Areas.Admin.Features.EventManagerInvites;
using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using AllReady.Models;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.EventManagerInvites
{
    public class EventManagerInviteDetailQueryHandlerShould : InMemoryContextTest
    {
        private int inviteId = 1;
        private DateTime acceptedDateTimeUtc = new DateTime(2016, 5, 29);
        private string message = "testMessage";
        private int eventId = 2;
        private string eventName = "event";
        private string email = "test@test.com";
        private DateTime? rejectedTime = null;
        private DateTime? revokedTime = null;
        private string senderUserId = Guid.NewGuid().ToString();
        private string senderEmail = "send@test.com";
        private DateTime sentTime = new DateTime(2016, 2, 28);
        private int campaignId = 10;
        private string campaignName = "testCampaign";
        private int orgId = 100;

        protected override void LoadTestData()
        {
            Context.EventManagerInvites.Add(new EventManagerInvite
            {
                Id = inviteId,
                AcceptedDateTimeUtc = acceptedDateTimeUtc,
                CustomMessage = message,
                EventId = eventId,
                InviteeEmailAddress = email,
                RejectedDateTimeUtc = rejectedTime,
                RevokedDateTimeUtc = revokedTime,
                SenderUserId = senderUserId,
                SentDateTimeUtc = sentTime,
            });

            Context.Events.Add(new Event
            {
                Id = eventId,
                Name = eventName,
                CampaignId = campaignId,
            });

            Context.Campaigns.Add(new Campaign
            {
                Id = campaignId,
                Name = campaignName,
                ManagingOrganizationId = orgId,
            });

            Context.Users.Add(new ApplicationUser
            {
                Id = senderUserId,
                Email = senderEmail,
            });

            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnCorrectViewModel_WhenInviteExists()
        {
            var handler = new EventManagerInviteDetailQueryHandler(Context);
            EventManagerInviteDetailsViewModel result = await handler.Handle(new EventManagerInviteDetailQuery { EventManagerInviteId = 1 });

            result.Id.ShouldBe(inviteId);
            result.AcceptedDateTimeUtc.ShouldBe(acceptedDateTimeUtc);
            result.CampaignId.ShouldBe(campaignId);
            result.CampaignName.ShouldBe(campaignName);
            result.CustomMessage.ShouldBe(message);
            result.EventId.ShouldBe(eventId);
            result.EventName.ShouldBe(eventName);
            result.InviteeEmailAddress.ShouldBe(email);
            result.RejectedDateTimeUtc.ShouldBe(rejectedTime);
            result.RevokedDateTimeUtc.ShouldBe(revokedTime);
            result.SenderUserEmail.ShouldBe(senderEmail);
            result.SentDateTimeUtc.ShouldBe(sentTime);
            result.OrganizationId.ShouldBe(orgId);
            result.Status.ShouldBe("Accepted");
        }
    }
}
