using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AllReady.Extensions;
using System.IO;
using AllReady.Services;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditVolunteerTaskCommandHandler : IAsyncRequestHandler<EditVolunteerTaskCommand, int>
    {
        private readonly AllReadyContext _context;
        private readonly IAttachmentService attachmentService;
        
        public EditVolunteerTaskCommandHandler(AllReadyContext context, IAttachmentService attachmentService)
        {
            _context = context;
            this.attachmentService = attachmentService;
        }

        public async Task<int> Handle(EditVolunteerTaskCommand message)
        {
            var volunteerTask = await _context.VolunteerTasks.Include(t => t.RequiredSkills).SingleOrDefaultAsync(t => t.Id == message.VolunteerTask.Id) ?? _context.Add(new VolunteerTask()).Entity;

            volunteerTask.Name = message.VolunteerTask.Name;
            volunteerTask.Description = message.VolunteerTask.Description;
            volunteerTask.Event = _context.Events.SingleOrDefault(a => a.Id == message.VolunteerTask.EventId);
            volunteerTask.Organization = _context.Organizations.SingleOrDefault(t => t.Id == message.VolunteerTask.OrganizationId);

            volunteerTask.StartDateTime = message.VolunteerTask.StartDateTime;
            volunteerTask.EndDateTime = message.VolunteerTask.EndDateTime;

            volunteerTask.NumberOfVolunteersRequired = message.VolunteerTask.NumberOfVolunteersRequired;
            volunteerTask.IsLimitVolunteers = volunteerTask.Event.IsLimitVolunteers;
            volunteerTask.IsAllowWaitList = volunteerTask.Event.IsAllowWaitList;

            if (volunteerTask.Id > 0)
            {
                var volunteerTaskSkillsToRemove = _context.VolunteerTaskSkills.Where(ts => ts.VolunteerTaskId == volunteerTask.Id && (message.VolunteerTask.RequiredSkills == null || message.VolunteerTask.RequiredSkills.All(ts1 => ts1.SkillId != ts.SkillId)));
                _context.VolunteerTaskSkills.RemoveRange(volunteerTaskSkillsToRemove);
            }

            if (message.VolunteerTask.RequiredSkills != null)
            {
                volunteerTask.RequiredSkills.AddRange(message.VolunteerTask.RequiredSkills.Where(mt => volunteerTask.RequiredSkills.All(ts => ts.SkillId != mt.SkillId)));
            }

            // Delete existing attachments
            if (message.VolunteerTask.DeleteAttachments.Count > 0)
            {
                var attachmentsToDelete = _context.Attachments.Where(a => a.VolunteerTask.Id == volunteerTask.Id && message.VolunteerTask.DeleteAttachments.Contains(a.Id)).ToList();
                _context.RemoveRange(attachmentsToDelete);
            }

            // Add new attachment
            if (message.VolunteerTask.NewAttachment != null && !string.IsNullOrEmpty(message.VolunteerTask.NewAttachment.FileName))
            {
                var attachmentModel = message.VolunteerTask.NewAttachment;
                var attachmentUrl = await attachmentService.UploadTaskAttachmentAsync(message.VolunteerTask.Id, attachmentModel);
                var attachment = new FileAttachment
                {
                    Name = attachmentModel.FileName,
                    Description = message.VolunteerTask.NewAttachmentDescription,
                    Url = attachmentUrl,
                    VolunteerTask = volunteerTask,
                };

                _context.Add(attachment);
            }

            await _context.SaveChangesAsync();

            return volunteerTask.Id;
        }
    }
}
