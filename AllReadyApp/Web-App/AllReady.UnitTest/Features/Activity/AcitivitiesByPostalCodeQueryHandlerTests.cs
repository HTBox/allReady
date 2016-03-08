using AllReady.Features.Activity;
using AllReady.Models;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Features.Activity
{
    public class AcitivitiesByPostalCodeQueryHandlerTests
    {
        [Fact]
        public void HandleCallsActivitiesByPostalCodeWithCorrectPostalCodeAndDistance()
        {
            var message = new AcitivitiesByPostalCodeQuery { PostalCode = "PostalCode", Distance = 100 };
            var dataStore = new Mock<IAllReadyDataAccess>();
            var sut = new AcitivitiesByPostalCodeQueryHandler(dataStore.Object);
            var results = sut.Handle(message);

            dataStore.Verify(x => x.ActivitiesByPostalCode(message.PostalCode, message.Distance), Times.Once);
        }
    }
}
