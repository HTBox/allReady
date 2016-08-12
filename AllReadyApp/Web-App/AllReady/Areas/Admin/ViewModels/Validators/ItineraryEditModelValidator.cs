using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.Shared;

namespace AllReady.Areas.Admin.ViewModels.Validators
{
    public class ItineraryEditModelValidator : IItineraryEditModelValidator
    {
        public List<KeyValuePair<string, string>> Validate(ItineraryEditModel model, EventSummaryModel eventSummary)
        {
            var result = new List<KeyValuePair<string, string>>();

            if (model.Date < eventSummary.StartDateTime.Date)
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.Date), "Date cannot be earlier than the event start date " + eventSummary.StartDateTime.Date.ToString("d")));
            }

            if (model.Date > eventSummary.EndDateTime.Date)
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.Date), "Date cannot be later than the event end date " + eventSummary.EndDateTime.Date.ToString("d")));
            }

            return result;
        }
    }

    public interface IItineraryEditModelValidator
    {
        List<KeyValuePair<string, string>> Validate(ItineraryEditModel model, EventSummaryModel eventSummary);
    }
}
