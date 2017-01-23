using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Features.Events;
using AllReady.Models;
using MediatR;
using System.IO;
using AllReady.Services;

namespace AllReady.Areas.Admin.ViewModels.Validators.Task
{
    public class TaskEditViewModelValidator : ITaskEditViewModelValidator
    {
        private readonly IMediator _mediator;
        private readonly IAttachmentService _attachmentService;

        public TaskEditViewModelValidator(IMediator mediator, IAttachmentService attachmentService)
        {
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            if (attachmentService == null)
            {
                throw new ArgumentNullException(nameof(attachmentService));
            }

            _mediator = mediator;
            _attachmentService = attachmentService;
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
            
            // Rule - Attachments are optional
            if (viewModel.NewAttachment != null && !string.IsNullOrEmpty(viewModel.NewAttachment.FileName))
            {
                // Rule - New attachment must have content
                if (viewModel.NewAttachment == null || viewModel.NewAttachment.Length == 0)
                {
                    result.Add(new KeyValuePair<string, string>(nameof(viewModel.NewAttachment), "The attachment is empty"));
                }

                // Rule - New attachment must have a maximum size
                int maxBytes = _attachmentService.GetMaxAttachmentBytes();
                if (viewModel.NewAttachment.Length > maxBytes)
                {
                    result.Add(new KeyValuePair<string, string>(nameof(viewModel.NewAttachment), string.Format("The attachment is too large. It should have a maximum of {0} bytes.", maxBytes)));
                }

                // Rule - Attachment must be a document or an image
                string ext = Path.GetExtension(viewModel.NewAttachment.FileName).ToLower();
                IList<string> allowedFileExtensions = _attachmentService.GetAllowedExtensions();
                if (!allowedFileExtensions.Contains(ext))
                {
                    result.Add(new KeyValuePair<string, string>(nameof(viewModel.NewAttachment), "The attachment has an invalid extension. Allowed file types are :" + string.Join(", ", allowedFileExtensions)));
                }
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
