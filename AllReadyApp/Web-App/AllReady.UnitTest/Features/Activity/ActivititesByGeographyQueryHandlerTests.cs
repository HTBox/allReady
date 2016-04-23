using AllReady.Features.Event;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Event
{
    public class EventsByGeographyQueryHandlerTests
    {
        [Fact]
        public void HandleCallsEventsByGeographyWithTheCorrectLatitiudeLongitudeAndMiles()
        {
            var message = new EventsByGeographyQuery() { Latitude = 1, Longitude = 2, Miles = 3 };
            var dataAccess = new Mock<IAllReadyDataAccess>();

            var sut = new EventsByGeographyQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.EventsByGeography(message.Latitude, message.Longitude, message.Miles));
        }
    }
}
