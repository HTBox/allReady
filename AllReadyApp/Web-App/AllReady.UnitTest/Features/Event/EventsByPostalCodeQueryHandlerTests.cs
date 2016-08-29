using AllReady.Features.Event;
using AllReady.Models;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    using Event = AllReady.Models.Event;

    public class EventsByPostalCodeQueryHandlerTests : InMemoryContextTest
    {
        [Fact(Skip = "Can't mock FromSql()")]
        public async Task HandleCallsEventsByPostalCodeWithCorrectPostalCodeAndDistance()
        {
            var options = this.CreateNewContextOptions();

            var message = new EventsByPostalCodeQuery { PostalCode = "PostalCode", Distance = 100 };

            using (var context = new AllReadyContext(options)) {
                context.Events.Add(new Event());
                context.Events.Add(new Event());
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new EventsByPostalCodeQueryHandler(context);
                var events = sut.Handle(message);

                Assert.Equal(events.Count, 2);
            }
        }
    }
}
