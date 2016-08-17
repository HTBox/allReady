using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AllReady.Extensions;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskCommandHandlerAsync : IAsyncRequestHandler<EditTaskCommandAsync, int>
    {
        private readonly AllReadyContext _context;

        public EditTaskCommandHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(EditTaskCommandAsync message)
        {
            var task = await _context.Tasks.Include(t => t.RequiredSkills).SingleOrDefaultAsync(t => t.Id == message.Task.Id).ConfigureAwait(false) ?? new AllReadyTask();

            task.Name = message.Task.Name;
            task.Description = message.Task.Description;
            task.Event = _context.Events.SingleOrDefault(a => a.Id == message.Task.EventId);
            task.Organization = _context.Organizations.SingleOrDefault(t => t.Id == message.Task.OrganizationId);

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(message.Task.TimeZoneId);

            var startDateValue = message.Task.StartDateTime;
            task.StartDateTime = new DateTimeOffset(startDateValue.Year, startDateValue.Month, startDateValue.Day, startDateValue.Hour, startDateValue.Minute, 0, timeZoneInfo.GetUtcOffset(startDateValue));

            var endDateValue = message.Task.EndDateTime;
            task.EndDateTime = new DateTimeOffset(endDateValue.Year, endDateValue.Month, endDateValue.Day, endDateValue.Hour, endDateValue.Minute, 0, timeZoneInfo.GetUtcOffset(endDateValue));

            task.NumberOfVolunteersRequired = message.Task.NumberOfVolunteersRequired;
            task.IsLimitVolunteers = task.Event.IsLimitVolunteers;
            task.IsAllowWaitList = task.Event.IsAllowWaitList;

            if (task.Id > 0)
            {
                var taskSkillsToRemove = _context.TaskSkills.Where(ts => ts.TaskId == task.Id && (message.Task.RequiredSkills == null || message.Task.RequiredSkills.All(ts1 => ts1.SkillId != ts.SkillId)));
                _context.TaskSkills.RemoveRange(taskSkillsToRemove);
            }

            if (message.Task.RequiredSkills != null)
            {
                task.RequiredSkills.AddRange(message.Task.RequiredSkills.Where(mt => task.RequiredSkills.All(ts => ts.SkillId != mt.SkillId)));
            }

            _context.AddOrUpdate(task);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return task.Id;
        }
    }
}