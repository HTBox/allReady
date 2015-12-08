using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskCommandHandler : IRequestHandler<EditTaskCommand, int>
    {
        private AllReadyContext _context;

        public EditTaskCommandHandler(AllReadyContext context)
        {
            _context = context;
        }
        public int Handle(EditTaskCommand message)
        {
            var task = _context.Tasks.Include(t => t.RequiredSkills).SingleOrDefault(t => t.Id == message.Task.Id);

            if (task == null)
            {
                task = new AllReadyTask();
            }

            task.Name = message.Task.Name;
            task.Description = message.Task.Description;
            task.Activity = _context.Activities.SingleOrDefault(a => a.Id == message.Task.ActivityId);
            task.Organization = _context.Organizations.SingleOrDefault(t => t.Id == message.Task.TenantId);

            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(message.Task.TimeZoneId);
            if (message.Task.StartDateTime.HasValue)
            {
                var startDateValue = message.Task.StartDateTime.Value;
                var startDateTimeOffset = timeZone.GetUtcOffset(startDateValue);
                task.StartDateTime = new DateTimeOffset(startDateValue.Year, startDateValue.Month, startDateValue.Day, startDateValue.Hour, startDateValue.Minute, 0, startDateTimeOffset);
            }
            else
            {
                task.StartDateTime = null;
            }

            if (message.Task.EndDateTime.HasValue)
            {
                var endDateValue = message.Task.EndDateTime.Value;
                var endDateTimeOffset = timeZone.GetUtcOffset(endDateValue);
                task.EndDateTime = new DateTimeOffset(endDateValue.Year, endDateValue.Month, endDateValue.Day, endDateValue.Hour, endDateValue.Minute, 0, endDateTimeOffset);
            }
            else
            {
                task.EndDateTime = null;
            }
            task.NumberOfVolunteersRequired = message.Task.NumberOfVolunteersRequired;
            if (task.Id > 0)
            {
                var tsToRemove = _context.TaskSkills.Where(ts => ts.TaskId == task.Id && (message.Task.RequiredSkills == null ||
                    !message.Task.RequiredSkills.Any(ts1 => ts1.SkillId == ts.SkillId)));
                _context.TaskSkills.RemoveRange(tsToRemove);
            }
            if (message.Task.RequiredSkills != null)
            {
                task.RequiredSkills.AddRange(message.Task.RequiredSkills.Where(mt => !task.RequiredSkills.Any(ts => ts.SkillId == mt.SkillId)));
            }

            _context.Update(task);
            _context.SaveChanges();
            return task.Id;
        }
    }
}
