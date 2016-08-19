using System;
using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Features.Events;
using AllReady.Models;
using AllReady.Providers;
using MediatR;

namespace AllReady.Areas.Admin.ViewModels.Validators
{
    public class TaskEditViewModelValidator : ITaskEditViewModelValidator
    {
        private readonly IMediator _mediator;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public TaskSummaryModelValidator(IMediator mediator, IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            _mediator = mediator;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public List<KeyValuePair<string, string>> Validate(EditViewModel model)
        {
            var result = new List<KeyValuePair<string, string>>();

            var campaignEvent = _mediator.Send(new EventByIdQuery { EventId = model.EventId });

            var convertedStartDateTime = _dateTimeOffsetProvider.GetDateTimeOffsetFor(campaignEvent.Campaign.TimeZoneId, model.StartDateTime, model.StartDateTime.Hour, model.StartDateTime.Minute);
            var convertedEndDateTime = _dateTimeOffsetProvider.GetDateTimeOffsetFor(campaignEvent.Campaign.TimeZoneId, model.EndDateTime, model.EndDateTime.Hour, model.EndDateTime.Minute);

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

    public interface ITaskEditViewModelValidator
    {
        List<KeyValuePair<string, string>> Validate(EditViewModel model);
    }
}
