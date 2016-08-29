using AllReady.Features.Event;
using AllReady.Models;
using Xunit;
using System.Threading.Tasks;

namespace AllReady.UnitTest.Features.Event
{
    using Event = AllReady.Models.Event;

    public class EventByEventIdQueryHandlerTests : InMemoryContextTest
    {
        [Fact]
        public async Task CallsGetEventWithTheCorrectEventId()
        {
            var options = this.CreateNewContextOptions();

            const int eventId = 1;
            var message = new EventByIdQuery { EventId = eventId };

            using (var context = new AllReadyContext(options)) {
                context.Events.Add(new Event {
                    Id = eventId
                });
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new EventByIdQueryHandler(context);
                var e = sut.Handle(message);

                Assert.Equal(e.Id, eventId);
            }
        }
    }
}
