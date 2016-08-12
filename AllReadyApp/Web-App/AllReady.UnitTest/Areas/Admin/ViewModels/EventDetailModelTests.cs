using AllReady.Areas.Admin.ViewModels.Event;
using Xunit;

namespace AllReady.UnitTest.ModelTests
{
    public class EventDetailModelTests
    {
        [Fact]
        public void DisplayItineraries_ReturnsTrue_WhenEventIsItineraryManaged()
        {
            var sut = new EventDetailViewModel();
            sut.EventType = Models.EventType.Itinerary;

            Assert.True(sut.IsItineraryEvent);
        }

        [Fact]
        public void DisplayItineraries_ReturnsFalse_WhenEventIsNotItineraryManaged()
        {
            var sut = new EventDetailViewModel();
            sut.EventType = Models.EventType.Rally;

            Assert.False(sut.IsItineraryEvent);
        }
    }
}
