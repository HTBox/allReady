using System.Globalization;
using AllReady.Areas.Admin.ViewModels.Event;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.ViewModels
{
    public class EventDetailViewModelShould
    {
        [Fact]
        public void DisplayItineraries_ReturnsTrue_WhenEventIsItineraryManaged()
        {
            var sut = new EventDetailViewModel {EventType = AllReady.Models.EventType.Itinerary};

            Assert.True(sut.IsItineraryEvent);
        }

        [Fact]
        public void DisplayItineraries_ReturnsFalse_WhenEventIsNotItineraryManaged()
        {
            var sut = new EventDetailViewModel {EventType = AllReady.Models.EventType.Rally};

            Assert.False(sut.IsItineraryEvent);
        }

        [Fact]
        public void UnassignedPercentage_ReturnsZero_WhenTotalRequestsIsZero()
        {
            var sut = new EventDetailViewModel
            {
                TotalRequests = 0,
                UnassignedRequests = 0
            };

            var result = sut.UnassignedPercentage;

            result.ShouldBe(0D.ToString("0.0"));
        }

        [Fact]
        public void UnassignedPercentage_ReturnsCorrectPercentage()
        {
            var sut = new EventDetailViewModel
            {
                TotalRequests = 100,
                UnassignedRequests = 20
            };

            var result = sut.UnassignedPercentage;

            result.ShouldBe(20D.ToString("0.0"));
        }

        [Fact]
        public void AssignedPercentage_ReturnsZero_WhenTotalRequestsIsZero()
        {
            var sut = new EventDetailViewModel
            {
                TotalRequests = 0,
                AssignedRequests = 0
            };

            var result = sut.AssignedPercentage;

            result.ShouldBe(0D.ToString("0.0"));
        }

        [Fact]
        public void AssignedPercentage_ReturnsCorrectPercentage()
        {
            var sut = new EventDetailViewModel
            {
                TotalRequests = 100,
                AssignedRequests = 20
            };

            var result = sut.AssignedPercentage;

            result.ShouldBe(20D.ToString("0.0"));
        }

        [Fact]
        public void CompletedPercentage_ReturnsZero_WhenTotalRequestsIsZero()
        {
            var sut = new EventDetailViewModel
            {
                TotalRequests = 0,
                CompletedRequests = 0
            };

            var result = sut.CompletedPercentage;

            result.ShouldBe(0D.ToString("0.0"));
        }

        [Fact]
        public void CompletedPercentage_ReturnsCorrectPercentage()
        {
            var sut = new EventDetailViewModel
            {
                TotalRequests = 100,
                CompletedRequests = 20
            };

            var result = sut.CompletedPercentage;

            result.ShouldBe(20D.ToString("0.0"));
        }

        [Fact]
        public void CanceledPercentage_ReturnsZero_WhenTotalRequestsIsZero()
        {
            var sut = new EventDetailViewModel
            {
                TotalRequests = 0,
                CanceledRequests = 0
            };

            var result = sut.CanceledPercentage;

            result.ShouldBe(0D.ToString("0.0"));
        }

        [Fact]
        public void CanceledPercentage_ReturnsCorrectPercentage()
        {
            var sut = new EventDetailViewModel
            {
                TotalRequests = 100,
                CanceledRequests = 20
            };

            var result = sut.CanceledPercentage;

            result.ShouldBe(20D.ToString("0.0"));
        }

        [Fact]
        public void VolunteerFulfilmentPercentage_ReturnsZero_WhenVoluneersRequiredIsZero()
        {
            var sut = new EventDetailViewModel
            {
                VolunteersRequired = 0,
                AcceptedVolunteers = 0
            };

            var result = sut.VolunteerFulfilmentPercentage;

            result.ShouldBe(0D.ToString("0.0"));
        }

        [Fact]
        public void VolunteerFulfilmentPercentage_ReturnsCorrectPercentage()
        {
            var sut = new EventDetailViewModel
            {
                VolunteersRequired = 10,
                AcceptedVolunteers = 2
            };

            var result = sut.VolunteerFulfilmentPercentage;

            result.ShouldBe(20D.ToString("0.0"));
        }
    }
}
