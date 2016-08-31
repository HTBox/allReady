using AllReady.Features.Event;
using AllReady.Models;
using Moq;
using Xunit;
using System.Threading.Tasks;

namespace AllReady.UnitTest.Features.Event
{
    using Event = AllReady.Models.Event;

    public class EventsByGeographyQueryHandlerTests : InMemoryContextTest
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

                Assert.Equal(events.Count, 2);
            }
        }
    }
}
