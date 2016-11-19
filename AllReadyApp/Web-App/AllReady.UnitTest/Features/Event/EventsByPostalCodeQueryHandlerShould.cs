using System.Collections.Generic;
using AllReady.Models;
using System.Threading.Tasks;
using AllReady.ExtensionWrappers;
using AllReady.Features.Events;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    using Event = AllReady.Models.Event;

    public class EventsByPostalCodeQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task HandleCallsEventsByPostalCodeWithCorrectPostalCodeAndDistance()
        {
            // arrange
            var options = CreateNewContextOptions();
            var message = new EventsByPostalCodeQuery { PostalCode = "PostalCode", Distance = 100 };
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
                var sut = new EventsByPostalCodeQueryHandler(context, mockFromSqlWrapper.Object);
                events = sut.Handle(message);
            }

            // Assert
            Assert.Equal(2, events.Count);
            mockFromSqlWrapper.Verify(w => w.FromSql(It.IsAny<DbSet<Event>>(), "EXEC GetClosestEventsByPostalCode '{0}', {1}, {2}", message.PostalCode, 50, message.Distance), Times.Once);
        }
    }
}
