using AllReady.Models;
using Xunit;
using System.Threading.Tasks;
using AllReady.Features.Events;

namespace AllReady.UnitTest.Features.Event
{
    using Event = AllReady.Models.Event;

    public class EventsByGeographyQueryHandlerShould : InMemoryContextTest
    {
        [Fact(Skip = "Can't mock FromSql()")]
        public async Task HandleCallsEventsByGeographyWithTheCorrectLatitiudeLongitudeAndMiles()
        {
            var options = this.CreateNewContextOptions();

            var message = new EventsByGeographyQuery() { Latitude = 1, Longitude = 2, Miles = 3 };

            using (var context = new AllReadyContext(options)) {
                context.Events.Add(new Event());
                context.Events.Add(new Event());
                await context.SaveChangesAsync();
            }

            using (var context = new AllReadyContext(options)) {
                var sut = new EventsByGeographyQueryHandler(context);
                var events = sut.Handle(message);

                Assert.Equal(2, events.Count);
            }
        }
    }
}
