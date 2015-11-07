using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskCommandHandler : IRequestHandler<EditTaskCommand, int>
    {
        private IAllReadyContext _context;

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
            task.Tenant = _context.Tenants.SingleOrDefault(t => t.Id == message.Task.TenantId);
            task.StartDateTimeUtc = message.Task.StartDateTime.HasValue ? message.Task.StartDateTime.Value.UtcDateTime : default(DateTime?);
            task.EndDateTimeUtc = message.Task.EndDateTime.HasValue ? message.Task.EndDateTime.Value.UtcDateTime : default(DateTime?);

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
