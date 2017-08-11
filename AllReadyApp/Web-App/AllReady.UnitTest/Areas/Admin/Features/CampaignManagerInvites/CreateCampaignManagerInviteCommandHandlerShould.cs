using AllReady.Areas.Admin.Features.CampaignManagerInvites;
using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.CampaignManagerInvites
{
    public class CreateCampaignManagerInviteCommandHandlerShould : InMemoryContextTest
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
        public async Task CreateCampaignManagerInvite()
        {
            var handler = new CreateCampaignManagerInviteCommandHandler(Context);
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
    }
}
