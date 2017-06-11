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
        public void ThrowArgumentException_WhenCampaignIdIsNotValid()
        {
            var handler = new CreateCampaignManagerInviteCommandHandler(Context);
            var invite = new CampaignManagerInviteViewModel
            {
                CampaignId = 10,
            };

            var inviteCommand = new CreateCampaignManagerInviteCommand
            {
                Invite = invite,
                UserId = "userId"
            };

            Should.Throw<ArgumentException>(async () => await handler.Handle(inviteCommand));
        }



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
            Context.CampaignManagerInvites.FirstOrDefault().AcceptedDateTimeUtc.ShouldBe(null);
            Context.CampaignManagerInvites.FirstOrDefault().CustomMessage.ShouldBe("message");
            Context.CampaignManagerInvites.FirstOrDefault().CampaignId.ShouldBe(1);
            Context.CampaignManagerInvites.FirstOrDefault().InviteeEmailAddress.ShouldBe("test@test.com");
            Context.CampaignManagerInvites.FirstOrDefault().RejectedDateTimeUtc.ShouldBe(null);
            Context.CampaignManagerInvites.FirstOrDefault().RevokedDateTimeUtc.ShouldBe(null);
            Context.CampaignManagerInvites.FirstOrDefault().SenderUserId.ShouldBe("userId");
            Context.CampaignManagerInvites.FirstOrDefault().SentDateTimeUtc.ShouldBe(new DateTime(2016, 5, 29));
        }
    }
}
