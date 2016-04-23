using AllReady.Features.Event;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class EventsByPostalCodeQueryHandlerTests
    {
        [Fact]
        public void HandleCallsEventsByPostalCodeWithCorrectPostalCodeAndDistance()
        {
            var message = new EventsByPostalCodeQuery { PostalCode = "PostalCode", Distance = 100 };
            var dataStore = new Mock<IAllReadyDataAccess>();
            var sut = new EventsByPostalCodeQueryHandler(dataStore.Object);
            sut.Handle(message);

            dataStore.Verify(x => x.EventsByPostalCode(message.PostalCode, message.Distance), Times.Once);
        }
    }
}
