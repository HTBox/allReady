using AllReady.Areas.Admin.Features.CampaignManagerInvites;
using AllReady.Models;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.CampaignManagerInvites
{
    public class AcceptCampaignManagerInviteCommandHandlerShould : InMemoryContextTest
    {
        private DateTime AcceptedTime = new DateTime(2017, 5, 29);
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
        public async Task ShouldSetIsAcceptedToTrue()
        {
            // Arrange
            var handler = new AcceptCampaignManagerInviteCommandHandler(Context);
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
    }
}
