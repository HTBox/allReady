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
    public class EditTaskCommandHandler : IAsyncRequestHandler<EditTaskCommand, int>
    {
        private readonly AllReadyContext _context;
        private readonly IAttachmentService _attachmentService;

        public EditTaskCommandHandler(AllReadyContext context, IAttachmentService attachmentService)
        {
            _context = context;
            _attachmentService = attachmentService;
        }

        public async Task<int> Handle(EditTaskCommand message)
        {
            var @task = await _context.Tasks.Include(t => t.RequiredSkills).SingleOrDefaultAsync(t => t.Id == message.Task.Id) ?? new AllReadyTask();

            @task.Name = message.Task.Name;
            @task.Description = message.Task.Description;
            @task.Event = _context.Events.SingleOrDefault(a => a.Id == message.Task.EventId);
            @task.Organization = _context.Organizations.SingleOrDefault(t => t.Id == message.Task.OrganizationId);

            @task.StartDateTime = message.Task.StartDateTime;
            @task.EndDateTime = message.Task.EndDateTime;

            @task.NumberOfVolunteersRequired = message.Task.NumberOfVolunteersRequired;
            @task.IsLimitVolunteers = @task.Event.IsLimitVolunteers;
            @task.IsAllowWaitList = @task.Event.IsAllowWaitList;

            if (@task.Id > 0)
            {
                var taskSkillsToRemove = _context.TaskSkills.Where(ts => ts.TaskId == @task.Id && (message.Task.RequiredSkills == null || message.Task.RequiredSkills.All(ts1 => ts1.SkillId != ts.SkillId)));
                _context.TaskSkills.RemoveRange(taskSkillsToRemove);
            }

            if (message.Task.RequiredSkills != null)
            {
                @task.RequiredSkills.AddRange(message.Task.RequiredSkills.Where(mt => @task.RequiredSkills.All(ts => ts.SkillId != mt.SkillId)));
            }

            _context.AddOrUpdate(@task);
            
            // Delete existing attachments
            if (message.Task.DeleteAttachments.Count > 0)
            {
                var attachmentsToDelete = _context.Attachments.Where(a => a.Task.Id == @task.Id && message.Task.DeleteAttachments.Contains(a.Id)).ToList();
                _context.RemoveRange(attachmentsToDelete);
            }

            // Add new attachment
            if (message.Task.NewAttachment != null && !string.IsNullOrEmpty(message.Task.NewAttachment.FileName))
            {
                var attachmentModel = message.Task.NewAttachment;
                var attachmentUrl = await _attachmentService.UploadTaskAttachmentAsync(message.Task.Id, attachmentModel);

                var attachment = new FileAttachment
                {
                    Name = attachmentModel.FileName,
                    Description = message.Task.NewAttachmentDescription,
                    ContentType = attachmentModel.ContentType,
                    Url = attachmentUrl,
                    Task = @task,
                };

                _context.Add(attachment);
            }

            await _context.SaveChangesAsync();

            return @task.Id;
        }

        /// <summary>Gets the MIME type of the given file name</summary>
        /// <param name="fileName">The name of the file</param>
        /// <returns>The MIME type</returns>
        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
            {
                mimeType = regKey.GetValue("Content Type").ToString();
            }
            return mimeType;
        }
    }
}