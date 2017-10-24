using AllReady.Areas.Admin.Features.EventManagerInvites;
using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace AllReady.UnitTest.Areas.Admin.Features.EventManagerInvites
{
    public class AcceptEventManagerInviteCommandHandlerShould : InMemoryContextTest
    {
        private DateTime AcceptedTime = new DateTime(2017, 5, 29);
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
        public async Task ShouldSetIsAcceptedToTrue()
        {
            // Arrange
            var handler = new AcceptEventManagerInviteCommandHandler(Context);
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
    }
}
