﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public TaskEditViewModelValidator(IMediator mediator)
        {
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            _mediator = mediator;
        }

        public async Task<List<KeyValuePair<string, string>>> Validate(EditViewModel viewModel)
        {
            var result = new List<KeyValuePair<string, string>>();

            var parentEvent = await _mediator.SendAsync(new EventByEventIdQuery { EventId = viewModel.EventId });


            // Rule - End date cannot be earlier than start date
            if (viewModel.EndDateTime < viewModel.StartDateTime)
            {
                result.Add(new KeyValuePair<string, string>(nameof(viewModel.EndDateTime), "End date cannot be earlier than the start date"));
            }

            // Rule - Start date cannot be out of range of parent event
            if (viewModel.StartDateTime < parentEvent.StartDateTime)
            {
                result.Add(new KeyValuePair<string, string>(nameof(viewModel.StartDateTime), String.Format("Start date cannot be earlier than the event start date {0:g}.", parentEvent.StartDateTime)));
            }

            // Rule - End date cannot be out of range of parent event
            if (viewModel.EndDateTime > parentEvent.EndDateTime)
            {
                result.Add(new KeyValuePair<string, string>(nameof(viewModel.EndDateTime), String.Format("The end date of this task cannot be after the end date of the event {0:g}", parentEvent.EndDateTime)));
            }

            // Rule - Itinerary tasks must start and end on same calendar day
            if (parentEvent.EventType == EventType.Itinerary)
            {
                if (viewModel.StartDateTime.Date != viewModel.EndDateTime.Date)
                {
                    result.Add(new KeyValuePair<string, string>(nameof(viewModel.EndDateTime), "For itinerary events the task end date must occur on the same day as the start date. Tasks cannot span multiple days"));
                }
            }

            return result;
        }
    }

    public interface ITaskEditViewModelValidator
    {
        Task<List<KeyValuePair<string, string>>> Validate(EditViewModel viewModel);
    }
}