using AllReady.Areas.Admin.Features.EventManagerInvites;
using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.EventManagerInvites
{
    public class CreateEventManagerInviteCommandHandlerShould : InMemoryContextTest
    {
        //protected override void LoadTestData()
        //{
        //    Context.Campaigns.Add(new AllReady.Models.Campaign
        //    {
        //        Id = 1
        //    });

        //    Context.Events.Add(new AllReady.Models.Event
        //    {
        //        Id = 5,
        //        CampaignId = 1
        //    });

        //    Context.SaveChanges();
        //}

        [Fact]
        public async Task CreateEventManagerInvite()
        {
            var handler = new CreateEventManagerInviteCommandHandler(Context);
            var invite = new EventManagerInviteViewModel
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
            Context.EventManagerInvites.SingleOrDefault().AcceptedDateTimeUtc.ShouldBe(null);
            Context.EventManagerInvites.SingleOrDefault().CustomMessage.ShouldBe("message");
            Context.EventManagerInvites.SingleOrDefault().EventId.ShouldBe(5);
            Context.EventManagerInvites.SingleOrDefault().InviteeEmailAddress.ShouldBe("test@test.com");
            Context.EventManagerInvites.SingleOrDefault().RejectedDateTimeUtc.ShouldBe(null);
            Context.EventManagerInvites.SingleOrDefault().RevokedDateTimeUtc.ShouldBe(null);
            Context.EventManagerInvites.SingleOrDefault().SenderUserId.ShouldBe("userId");
            Context.EventManagerInvites.SingleOrDefault().SentDateTimeUtc.ShouldBe(new DateTime(2016, 5, 29));
        }
    }
}
