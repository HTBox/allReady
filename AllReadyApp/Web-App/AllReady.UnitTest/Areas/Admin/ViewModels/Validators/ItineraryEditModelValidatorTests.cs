using System;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Areas.Admin.ViewModels.Validators;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.ViewModels.Validators
{
    public class ItineraryEditModelValidatorTests
    {
        private readonly DateTimeOffset eventStartDate = new DateTimeOffset(new DateTime(2016, 1, 1));
        private readonly DateTimeOffset eventEndDate = new DateTimeOffset(new DateTime(2020, 12, 31));

        [Fact]
        public void ReturnsCorrectErrorWhenDateIsLessThanParentEventStartDate()
        {
            var sut = new ItineraryEditModelValidator();

            var model = new ItineraryEditViewModel
            {
                EventId = 1,
                Date = eventStartDate.AddDays(-1).DateTime
            };

            var errors = sut.Validate(model, TestEvent);

            Assert.True(errors.Exists(x => x.Key.Equals("Date")));
            Assert.Equal(errors.Find(x => x.Key == "Date").Value, "Date cannot be earlier than the event start date " + eventStartDate.Date.ToString("d"));
        }

        [Fact]
        public void ReturnsCorrectErrorWhenModelsDateIsGreaterThanParentEventEndDate()
        {
            var sut = new ItineraryEditModelValidator();

            var model = new ItineraryEditViewModel
            {
                EventId = 1,
                Date = eventEndDate.AddDays(1).DateTime
            };

            var errors = sut.Validate(model, TestEvent);

            Assert.True(errors.Exists(x => x.Key.Equals("Date")));
            Assert.Equal(errors.Find(x => x.Key == "Date").Value, "Date cannot be later than the event end date " + eventEndDate.Date.ToString("d"));
        }

        [Fact]
        public void ReturnsNoErrorForWhenDatesIsBetweenStartAndEndOfParentEvent()
        {
            var sut = new ItineraryEditModelValidator();

            var model = new ItineraryEditViewModel
            {
                EventId = 1,
                Date = eventStartDate.AddDays(1).DateTime
            };

            var errors = sut.Validate(model, TestEvent);

            Assert.True(errors.Count == 0);
        }

        private EventSummaryViewModel TestEvent => new EventSummaryViewModel
        {
            Id = 1,
            StartDateTime = eventStartDate,
            EndDateTime = eventEndDate
        };
    }
}
