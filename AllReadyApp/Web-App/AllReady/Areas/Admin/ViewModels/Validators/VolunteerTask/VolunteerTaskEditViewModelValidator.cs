using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.Features.Events;
using AllReady.Models;

using MediatR;

namespace AllReady.Areas.Admin.ViewModels.Validators.VolunteerTask
{
    public class VolunteerTaskEditViewModelValidator : IValidateVolunteerTaskEditViewModelValidator
    {
        private readonly IMediator _mediator;

        // list of executable extensions (same extensions as blocked by Gmail)
        private readonly HashSet<string> ForbiddenExtensions = new HashSet<string> { ".ade", ".adp", ".bat", ".chm", ".cmd", ".com", ".cpl", ".exe", ".hta", ".ins", ".isp", ".jar", ".jse", ".lib", ".lnk", ".mde", ".msc", ".msp", ".mst", ".pif", ".scr", ".sct", ".shb", ".sys", ".vb", ".vbe", ".vbs", ".vxd", ".wsc", ".wsf", ".wsh" };

        public VolunteerTaskEditViewModelValidator(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<List<KeyValuePair<string, string>>> Validate(EditViewModel viewModel)
        {
            var result = new List<KeyValuePair<string, string>>();

            var parentEvent = await this._mediator.SendAsync(new EventByEventIdQuery { EventId = viewModel.EventId });

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

            // Rule - Attachments are optional
            if (!string.IsNullOrEmpty(viewModel.NewAttachment?.FileName))
            {
                // Rule - New attachment must have content
                if (viewModel.NewAttachment == null || viewModel.NewAttachment.Length == 0)
                {
                    result.Add(new KeyValuePair<string, string>(nameof(viewModel.NewAttachment), "The attachment is empty"));
                }

                // Rule - Attachment must not be executable
                string ext = Path.GetExtension(viewModel.NewAttachment.FileName).ToLower();
                if (this.ForbiddenExtensions.Contains(ext))
                {
                    result.Add(new KeyValuePair<string, string>(nameof(viewModel.NewAttachment), "The attachment has an invalid extension. The disallowed file types are :" + string.Join(", ", this.ForbiddenExtensions)));
                }
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
