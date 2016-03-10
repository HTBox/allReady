using AllReady.Features.Activity;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Activity
{
    public class ActivititesByGeographyQueryHandlerTests
    {
        [Fact]
        public void HandleCallsActivitiesByGeographyWithTheCorrectLatitiudeLongitudeAndMiles()
        {
            var message = new ActivitiesByGeographyQuery() { Latitude = 1, Longitude = 2, Miles = 3 };
            var dataAccess = new Mock<IAllReadyDataAccess>();

            var sut = new ActivititesByGeographyQueryHandler(dataAccess.Object);
            sut.Handle(message);

            dataAccess.Verify(x => x.ActivitiesByGeography(message.Latitude, message.Longitude, message.Miles));
        }
    }
}
