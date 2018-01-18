using AllReady.Areas.Admin.Features.CampaignManagerInvites;
using AllReady.Models;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.CampaignManagerInvites
{
    public class DeclineCampaignManagerInviteCommandHandlerShould : InMemoryContextTest
    {
        private DateTime RejectedTime = new DateTime(2017, 5, 29);
        private int inviteId = 20;

        protected override void LoadTestData()
        {
            Context.CampaignManagerInvites.Add(new CampaignManagerInvite
            {
                Id = inviteId
            });

            Context.SaveChanges();
        }

        [Fact]
        public async Task ShouldSetIsRejectedToTrue()
        {
            // Arrange
            var handler = new DeclineCampaignManagerInviteCommandHandler(Context);
            handler.DateTimeUtcNow = () => { return RejectedTime; };

            // Act
            await handler.Handle(new DeclineCampaignManagerInviteCommand
            {
                CampaignManagerInviteId = inviteId
            });

            // Assert
            var invite = Context.CampaignManagerInvites.SingleOrDefault(i => i.Id == inviteId);
            invite.IsRejected.ShouldBe(true);
            invite.RejectedDateTimeUtc.ShouldBe(RejectedTime);
        }
    }
}
