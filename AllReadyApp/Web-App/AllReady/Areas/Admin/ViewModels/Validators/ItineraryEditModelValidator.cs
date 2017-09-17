using System;
using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.Shared;
using TimeZoneConverter;

namespace AllReady.Areas.Admin.ViewModels.Validators
{
    public class ItineraryEditModelValidator : IItineraryEditModelValidator
    {
        public List<KeyValuePair<string, string>> Validate(ItineraryEditViewModel model, EventSummaryViewModel eventSummary)
        {
            var result = new List<KeyValuePair<string, string>>();

            var itineraryDateConverted = ConvertIntineraryDateToEventsTimeZone(model.Date, eventSummary.TimeZoneId);

            if (itineraryDateConverted.Date < eventSummary.StartDateTime.Date)
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.Date), $"Date cannot be earlier than the event start date {eventSummary.StartDateTime.Date:d}"));
            }

            if (itineraryDateConverted.Date > eventSummary.EndDateTime.Date)
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.Date), $"Date cannot be later than the event end date {eventSummary.EndDateTime.Date:d}"));
            }

            return result;
        }

        private static DateTimeOffset ConvertIntineraryDateToEventsTimeZone(DateTime itineraryDate, string eventsTimeZoneId)
        {
            var timeZoneInfo = TZConvert.GetTimeZoneInfo(eventsTimeZoneId);
            var utcOffset = timeZoneInfo.GetUtcOffset(itineraryDate);
            return new DateTimeOffset(itineraryDate, utcOffset);
        }
    }
    public interface IItineraryEditModelValidator
    {
        List<KeyValuePair<string, string>> Validate(ItineraryEditViewModel model, EventSummaryViewModel eventSummary);
    }
}
