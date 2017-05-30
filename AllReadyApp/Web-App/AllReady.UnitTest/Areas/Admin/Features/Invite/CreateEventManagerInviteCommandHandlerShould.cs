using AllReady.Areas.Admin.Features.Invite;
using AllReady.Areas.Admin.ViewModels.Invite;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Invite
{
    public class CreateEventManagerInviteCommandHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            Context.Campaigns.Add(new AllReady.Models.Campaign
            {
                Id = 1
            });

            Context.Events.Add(new AllReady.Models.Event
            {
                Id = 5,
                CampaignId = 1
            });

            Context.SaveChanges();
        }

        [Fact]
        public void ThrowArgumentException_WhenEventIdIsNotValid_AndInviteTypeIsEventManager()
        {
            var handler = new CreateEventManagerInviteCommandHandler(Context);
            var invite = new InviteViewModel
            {
                EventId = 2,
            };

            var inviteCommand = new CreateEventManagerInviteCommand
            {
                Invite = invite,
                UserId = "userId"
            };

            Should.Throw<ArgumentException>(async () => await handler.Handle(inviteCommand));
        }

        [Fact]
        public async Task CreateEventManagerInvite()
        {
            var handler = new CreateEventManagerInviteCommandHandler(Context);
            var invite = new InviteViewModel
            {
                CustomMessage = "message",
                EventId = 5,
                InviteeEmailAddress = "test@test.com",
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
            Context.EventManagerInvites.FirstOrDefault().AcceptedDateTimeUtc.ShouldBe(null);
            Context.EventManagerInvites.FirstOrDefault().CustomMessage.ShouldBe("message");
            Context.EventManagerInvites.FirstOrDefault().EventId.ShouldBe(5);
            Context.EventManagerInvites.FirstOrDefault().InviteeEmailAddress.ShouldBe("test@test.com");
            Context.EventManagerInvites.FirstOrDefault().RejectedDateTimeUtc.ShouldBe(null);
            Context.EventManagerInvites.FirstOrDefault().RevokedDateTimeUtc.ShouldBe(null);
            Context.EventManagerInvites.FirstOrDefault().SenderUserId.ShouldBe("userId");
            Context.EventManagerInvites.FirstOrDefault().SentDateTimeUtc.ShouldBe(new DateTime(2016, 5, 29));
        }
    }
}
