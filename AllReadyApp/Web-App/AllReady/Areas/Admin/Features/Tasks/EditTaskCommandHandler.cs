using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AllReady.Extensions;
using AllReady.Providers;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditTaskCommandHandler : IAsyncRequestHandler<EditTaskCommand, int>
    {
        private readonly AllReadyContext _context;

        public EditTaskCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(EditTaskCommand message)
        {
            var theTask = await _context.Tasks.Include(t => t.RequiredSkills).SingleOrDefaultAsync(t => t.Id == message.Task.Id) ?? new AllReadyTask();

            theTask.Name = message.Task.Name;
            theTask.Description = message.Task.Description;
            theTask.Event = _context.Events.SingleOrDefault(a => a.Id == message.Task.EventId);
            theTask.Organization = _context.Organizations.SingleOrDefault(t => t.Id == message.Task.OrganizationId);

            theTask.StartDateTime = message.Task.StartDateTime;
            theTask.EndDateTime = message.Task.EndDateTime;

            theTask.NumberOfVolunteersRequired = message.Task.NumberOfVolunteersRequired;
            theTask.IsLimitVolunteers = theTask.Event.IsLimitVolunteers;
            theTask.IsAllowWaitList = theTask.Event.IsAllowWaitList;

            if (theTask.Id > 0)
            {
                var taskSkillsToRemove = _context.TaskSkills.Where(ts => ts.TaskId == theTask.Id && (message.Task.RequiredSkills == null || message.Task.RequiredSkills.All(ts1 => ts1.SkillId != ts.SkillId)));
                _context.TaskSkills.RemoveRange(taskSkillsToRemove);
            }

            if (message.Task.RequiredSkills != null)
            {
                theTask.RequiredSkills.AddRange(message.Task.RequiredSkills.Where(mt => theTask.RequiredSkills.All(ts => ts.SkillId != mt.SkillId)));
            }

            _context.AddOrUpdate(theTask);

            await _context.SaveChangesAsync();

            return theTask.Id;
        }
    }
}