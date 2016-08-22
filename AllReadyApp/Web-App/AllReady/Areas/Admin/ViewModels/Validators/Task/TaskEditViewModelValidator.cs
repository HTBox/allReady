using System;
using System.Collections.Generic;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Features.Events;
using AllReady.Models;
using AllReady.Providers;
using MediatR;

namespace AllReady.Areas.Admin.ViewModels.Validators.Task
{
    public class TaskEditViewModelValidator : ITaskEditViewModelValidator
    {
        private readonly IMediator _mediator;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public TaskEditViewModelValidator(IMediator mediator, IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            _mediator = mediator;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public List<KeyValuePair<string, string>> Validate(EditViewModel viewModel)
        {
            var result = new List<KeyValuePair<string, string>>();

            var @event = _mediator.Send(new EventByIdQuery { EventId = viewModel.EventId });

            var convertedStartDateTime = _dateTimeOffsetProvider.GetDateTimeOffsetFor(@event.Campaign.TimeZoneId, viewModel.StartDateTime, viewModel.StartDateTime.Hour, viewModel.StartDateTime.Minute);
            var convertedEndDateTime = _dateTimeOffsetProvider.GetDateTimeOffsetFor(@event.Campaign.TimeZoneId, viewModel.EndDateTime, viewModel.EndDateTime.Hour, viewModel.EndDateTime.Minute);

            // Rule - End date cannot be earlier than start date
            if (convertedEndDateTime < convertedStartDateTime)
            {
                result.Add(new KeyValuePair<string, string>(nameof(viewModel.EndDateTime), "End date cannot be earlier than the start date"));
            }

            // Rule - Start date cannot be out of range of parent event
            if (convertedStartDateTime < @event.StartDateTime)
            {
                result.Add(new KeyValuePair<string, string>(nameof(viewModel.StartDateTime), "Start date cannot be earlier than the event start date " + @event.StartDateTime.ToString("d")));
            }

            // Rule - End date cannot be out of range of parent event
            if (convertedEndDateTime > @event.EndDateTime)
            {
                result.Add(new KeyValuePair<string, string>(nameof(viewModel.EndDateTime), "End date cannot be later than the event end date " + @event.EndDateTime.ToString("d")));
            }

            // Rule - Itinerary tasks must start and end on same calendar day
            if (@event.EventType == EventType.Itinerary)
            {
                if (convertedStartDateTime.Date != convertedEndDateTime.Date)
                {
                    result.Add(new KeyValuePair<string, string>(nameof(viewModel.EndDateTime), "For itinerary events the task end date must occur on the same day as the start date. Tasks cannot span multiple days"));
                }
            }

            return result;
        }
    }

    public interface ITaskEditViewModelValidator
    {
        List<KeyValuePair<string, string>> Validate(EditViewModel viewModel);
    }
}