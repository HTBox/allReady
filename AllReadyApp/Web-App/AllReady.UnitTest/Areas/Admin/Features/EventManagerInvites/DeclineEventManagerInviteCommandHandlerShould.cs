using AllReady.Areas.Admin.Features.EventManagerInvites;
using AllReady.Models;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.EventManagerInvites
{
    public class DeclineEventManagerInviteCommandHandlerShould : InMemoryContextTest
    {
        private DateTime RejectedTime = new DateTime(2017, 5, 29);
        private int inviteId = 20;

        protected override void LoadTestData()
        {
            Context.EventManagerInvites.Add(new EventManagerInvite
            {
                Id = inviteId
            });

            Context.SaveChanges();
        }

        [Fact]
        public async Task ShouldSetIsRejectedToTrue()
        {
            // Arrange
            var handler = new DeclineEventManagerInviteCommandHandler(Context);
            handler.DateTimeUtcNow = () => { return RejectedTime; };

            // Act
            await handler.Handle(new DeclineEventManagerInviteCommand
            {
                EventManagerInviteId = inviteId
            });

            // Assert
            var invite = Context.EventManagerInvites.SingleOrDefault(i => i.Id == inviteId);
            invite.IsRejected.ShouldBe(true);
            invite.RejectedDateTimeUtc.ShouldBe(RejectedTime);
        }
    }
}
