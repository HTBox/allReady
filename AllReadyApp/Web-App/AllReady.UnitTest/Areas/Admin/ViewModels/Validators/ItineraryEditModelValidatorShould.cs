using System;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Areas.Admin.ViewModels.Validators;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.ViewModels.Validators
{
    public class ItineraryEditModelValidatorShould
    {
        private readonly DateTimeOffset _eventStartDate = new DateTimeOffset(new DateTime(2016, 1, 1));
        private readonly DateTimeOffset _eventEndDate = new DateTimeOffset(new DateTime(2020, 12, 31));

        [Fact]
        public void ReturnCorrectError_WhenItineraryDate_IsLessThanEventStartDate()
        {
            var sut = new ItineraryEditModelValidator();

            var model = new ItineraryEditViewModel
            {
                EventId = 1,
                Date = _eventStartDate.AddDays(-1).DateTime
            };

            var errors = sut.Validate(model, TestEvent);

            Assert.True(errors.Exists(x => x.Key.Equals("Date")));
            Assert.Equal(errors.Find(x => x.Key == "Date").Value, "Date cannot be earlier than the event start date " + _eventStartDate.Date.ToString("d"));
        }

        [Fact]
        public void ReturnCorrectError_WhenItineraryDate_IsGreaterThanEventEndDate()
        {
            var sut = new ItineraryEditModelValidator();

            var model = new ItineraryEditViewModel
            {
                EventId = 1,
                Date = _eventEndDate.AddDays(1).DateTime
            };

            var errors = sut.Validate(model, TestEvent);

            Assert.True(errors.Exists(x => x.Key.Equals("Date")));
            Assert.Equal(errors.Find(x => x.Key == "Date").Value, "Date cannot be later than the event end date " + _eventEndDate.Date.ToString("d"));
        }

        private EventSummaryViewModel TestEvent => new EventSummaryViewModel
        {
            Id = 1,
            StartDateTime = _eventStartDate,
            EndDateTime = _eventEndDate,
            TimeZoneId = "Central Standard Time"
        };
    }
}
