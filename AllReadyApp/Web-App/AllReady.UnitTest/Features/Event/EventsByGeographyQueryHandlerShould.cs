using AllReady.Models;
using Xunit;
using System.Threading.Tasks;
using AllReady.Features.Events;

namespace AllReady.UnitTest.Features.Event
{
    using ExtensionWrappers;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using Event = AllReady.Models.Event;

    public class EventsByGeographyQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task HandleCallsEventsByGeographyWithTheCorrectLatitiudeLongitudeAndMiles()
        {
            // arrange
            var options = CreateNewContextOptions();
            var message = new EventsByGeographyQuery() { Latitude = 1, Longitude = 2, Miles = 3 };
            using (var context = new AllReadyContext(options)) {
                context.Events.Add(new Event());
                context.Events.Add(new Event());
                await context.SaveChangesAsync();
            }
            Mock<IFromSqlWrapper> mockFromSqlWrapper = new Mock<IFromSqlWrapper>();
            mockFromSqlWrapper.Setup(w => w.FromSql(It.IsAny<DbSet<Event>>(), It.IsAny<string>(), It.IsAny<object[]>()))
               .Returns(new AllReadyContext(options).Events);

            // act
            List<Event> events;
            using (var context = new AllReadyContext(options)) {

                var sut = new EventsByGeographyQueryHandler(context);
                var events = sut.Handle(message);

                Assert.Equal(2, events.Count);
            }

            // assert
            Assert.Equal(2, events.Count);
            mockFromSqlWrapper.Verify(w => w.FromSql(It.IsAny<DbSet<Event>>(), "EXEC GetClosestEvents {0}, {1}, {2}, {3}", message.Latitude, message.Longitude, 50, message.Miles), Times.Once);
        }
    }
}
