using AllReady.Models;
using System.Threading.Tasks;
using AllReady.Features.Events;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    using Event = AllReady.Models.Event;

    public class EventsByPostalCodeQueryHandlerShould : InMemoryContextTest
    {
        [Fact(Skip = "Can't mock FromSql()")]
        public async Task HandleCallsEventsByPostalCodeWithCorrectPostalCodeAndDistance()
        {
            var options = CreateNewContextOptions();

            var message = new EventsByPostalCodeQuery { PostalCode = "PostalCode", Distance = 100 };

            using (var context = new AllReadyContext(options))
            {
                context.Events.Add(new Event());
                context.Events.Add(new Event());
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options))
            {
                var sut = new EventsByPostalCodeQueryHandler(context);
                var events = sut.Handle(message);

                Assert.Equal(2, events.Count);
            }
        }
    }
}