using AllReady.Models;
using Xunit;
using System.Threading.Tasks;
using AllReady.Features.Events;

namespace AllReady.UnitTest.Features.Event
{
    using Event = AllReady.Models.Event;

    public class EventByEventIdQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task CallsGetEventWithTheCorrectEventId()
        {
            var options = CreateNewContextOptions();

            const int eventId = 1;
            var message = new EventByEventIdQuery { EventId = eventId };

            using (var context = new AllReadyContext(options))
            {
                context.Events.Add(new Event
                {
                    Id = eventId,
                    Location = new Location(),
                    Campaign = new Campaign {  ManagingOrganization = new Organization()},
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new EventByEventIdQueryHandler(context);
                var e = await sut.Handle(message);

                Assert.Equal(e.Id, eventId);
            }
        }
    }
}
