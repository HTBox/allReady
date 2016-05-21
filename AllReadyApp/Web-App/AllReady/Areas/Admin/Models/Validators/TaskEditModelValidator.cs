using AllReady.Features.Event;
using AllReady.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace AllReady.Areas.Admin.Models.Validators
{
    public class TaskSummaryModelValidator : ITaskSummaryModelValidator
    {
        private readonly IMediator _mediator;

        public TaskSummaryModelValidator(IMediator mediator)
        {
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            _mediator = mediator;
        }

        public List<KeyValuePair<string, string>> Validate(TaskSummaryModel model)
        {
            var result = new List<KeyValuePair<string, string>>();

            if (model.StartDateTime.HasValue || model.EndDateTime.HasValue)
            {
                var campaignEvent = _mediator.Send(new EventByIdQuery { EventId = model.EventId });

                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(campaignEvent.Campaign.TimeZoneId);

                // sgordon: Date time conversion seems overly complex and may be refactored per #710
                DateTimeOffset? convertedStartDateTime = null;
                if (model.StartDateTime.HasValue)
                {
                    var startDateValue = model.StartDateTime.Value;
                    var startDateTimeOffset = timeZoneInfo.GetUtcOffset(startDateValue);
                    convertedStartDateTime = new DateTimeOffset(startDateValue.Year, startDateValue.Month, startDateValue.Day, startDateValue.Hour, startDateValue.Minute, 0, startDateTimeOffset);
                }

                DateTimeOffset? convertedEndDateTime = null;
                if (model.EndDateTime.HasValue)
                {
                    var endDateValue = model.EndDateTime.Value;
                    var endDateTimeOffset = timeZoneInfo.GetUtcOffset(endDateValue);
                    convertedEndDateTime = new DateTimeOffset(endDateValue.Year, endDateValue.Month, endDateValue.Day, endDateValue.Hour, endDateValue.Minute, 0, endDateTimeOffset);
                }

                // Rule - End date cannot be earlier than start date
                if (convertedStartDateTime.HasValue && convertedEndDateTime.HasValue && convertedEndDateTime.Value < convertedStartDateTime.Value)
                {
                    result.Add(new KeyValuePair<string, string>(nameof(model.EndDateTime), "End date cannot be earlier than the start date"));
                }

                // Rule - Start date cannot be out of range of parent event
                if (convertedStartDateTime.HasValue && convertedStartDateTime < campaignEvent.StartDateTime)
                {
                    result.Add(new KeyValuePair<string, string>(nameof(model.StartDateTime), "Start date cannot be earlier than the event start date " + campaignEvent.StartDateTime.ToString("d")));
                }

                // Rule - End date cannot be out of range of parent event
                if (convertedEndDateTime.HasValue && convertedEndDateTime > campaignEvent.EndDateTime)
                {
                    result.Add(new KeyValuePair<string, string>(nameof(model.EndDateTime), "End date cannot be later than the event end date " + campaignEvent.EndDateTime.ToString("d")));
                }

                // Rule - Itinerary tasks must start and end on same calendar day
                if (campaignEvent.EventType == EventTypes.ItineraryManaged && convertedStartDateTime.HasValue && convertedEndDateTime.HasValue)
                {
                    if (convertedStartDateTime.Value.Date != convertedEndDateTime.Value.Date)
                    {
                        result.Add(new KeyValuePair<string, string>(nameof(model.EndDateTime), "For itinerary events the task end date must occur on the same day as the start date. Tasks cannot span multiple days"));
                    }
                }
            }

            return result;
        }
    }

    public interface ITaskSummaryModelValidator
    {
        List<KeyValuePair<string, string>> Validate(TaskSummaryModel model);
    }
}
