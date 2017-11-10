using AllReady.Areas.Admin.Features.EventManagerInvites;
using AllReady.Models;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.EventManagerInvites
{
    public class UserHasCampaignManagerInviteQueryHandlerShould : InMemoryContextTest
    {
        protected override void LoadTestData()
        {
            var eventManagmentInvite = new EventManagerInvite
            {
                Id = 200,
                InviteeEmailAddress = "test@test.com",
                CustomMessage = "test",
                EventId = 2,
            };

            Context.Campaigns.Add(new Campaign
            {
                Id = 5,
                Name = "testCampaign",
            });

            Context.Events.Add(new Event
            {
                Id = 2,
                Name = "testEvent",
                CampaignId = 5,
            });

            Context.EventManagerInvites.Add(eventManagmentInvite);

            Context.SaveChanges();
        }

        [Fact]
        public async Task ReturnFalse_WhenInviteDoesNotExist()
        {
            var handler = new UserHasEventManagerInviteQueryHandler(Context);
            bool result = await handler.Handle(new UserHasEventManagerInviteQuery { InviteeEmail = "invalid@email.com", EventId = 2 });
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task ReturnTrue_WhenInviteExists()
        {
            var handler = new UserHasEventManagerInviteQueryHandler(Context);
            bool result = await handler.Handle(new UserHasEventManagerInviteQuery { InviteeEmail = "test@test.com", EventId = 2 });
            result.ShouldBeTrue();
        }
    }
}
