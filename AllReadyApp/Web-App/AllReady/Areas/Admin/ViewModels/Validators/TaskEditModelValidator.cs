using System;
using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Features.Event;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.ViewModels.Validators
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

            var campaignEvent = _mediator.Send(new EventByIdQuery { EventId = model.EventId });

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(campaignEvent.Campaign.TimeZoneId);

            // sgordon: Date time conversion seems overly complex and may be refactored per #710
            var startDateValue = model.StartDateTime;
            var startDateTimeOffset = timeZoneInfo.GetUtcOffset(startDateValue);
            var convertedStartDateTime = new DateTimeOffset(startDateValue.Year, startDateValue.Month, startDateValue.Day, startDateValue.Hour, startDateValue.Minute, 0, startDateTimeOffset);

            var endDateValue = model.EndDateTime;
            var endDateTimeOffset = timeZoneInfo.GetUtcOffset(endDateValue);
            var convertedEndDateTime = new DateTimeOffset(endDateValue.Year, endDateValue.Month, endDateValue.Day, endDateValue.Hour, endDateValue.Minute, 0, endDateTimeOffset);

            // Rule - End date cannot be earlier than start date
            if (convertedEndDateTime < convertedStartDateTime)
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.EndDateTime), "End date cannot be earlier than the start date"));
            }

            // Rule - Start date cannot be out of range of parent event
            if (convertedStartDateTime < campaignEvent.StartDateTime)
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.StartDateTime), "Start date cannot be earlier than the event start date " + campaignEvent.StartDateTime.ToString("d")));
            }

            // Rule - End date cannot be out of range of parent event
            if (convertedEndDateTime > campaignEvent.EndDateTime)
            {
                result.Add(new KeyValuePair<string, string>(nameof(model.EndDateTime), "End date cannot be later than the event end date " + campaignEvent.EndDateTime.ToString("d")));
            }

            // Rule - Itinerary tasks must start and end on same calendar day
            if (campaignEvent.EventType == EventType.Itinerary)
            {
                if (convertedStartDateTime.Date != convertedEndDateTime.Date)
                {
                    result.Add(new KeyValuePair<string, string>(nameof(model.EndDateTime), "For itinerary events the task end date must occur on the same day as the start date. Tasks cannot span multiple days"));
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
