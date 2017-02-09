using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Features.Events;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.ViewModels.Validators.Task
{
    public class VolunteerTaskEditViewModelValidator : IValidateVolunteerTaskEditViewModelValidator
    {
        private readonly IMediator _mediator;

        public VolunteerTaskEditViewModelValidator(IMediator mediator)
        {
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
                result.Add(new KeyValuePair<string, string>(nameof(viewModel.StartDateTime), $"Start date cannot be earlier than the event start date {parentEvent.StartDateTime:g}."));
            }

            // Rule - End date cannot be out of range of parent event
            if (viewModel.EndDateTime > parentEvent.EndDateTime)
            {
                result.Add(new KeyValuePair<string, string>(nameof(viewModel.EndDateTime), $"The end date of this task cannot be after the end date of the event {parentEvent.EndDateTime:g}"));
            }

            // Rule - Itinerary volunteerTasks must start and end on same calendar day
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
}