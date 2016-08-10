using AllReady.Areas.Admin.Models;
using Xunit;

namespace AllReady.UnitTest.ModelTests
{
    public class EventDetailModelTests
    {
        [Fact]
        public void DisplayItineraries_ReturnsTrue_WhenEventIsItineraryManaged()
        {
            var sut = new EventDetailModel();
            sut.EventType = AllReady.Models.EventType.Itinerary;

            Assert.True(sut.IsItineraryEvent);
        }

        [Fact]
        public void DisplayItineraries_ReturnsFalse_WhenEventIsNotItineraryManaged()
        {
            var sut = new EventDetailModel();
            sut.EventType = AllReady.Models.EventType.Rally;

            Assert.False(sut.IsItineraryEvent);
        }
    }
}
