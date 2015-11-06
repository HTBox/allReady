using AllReady.Models;
using MediatR;
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
            var task = _context.Tasks.SingleOrDefault(t => t.Id == message.Task.Id);

            if (task == null)
            {
                task = new AllReadyTask();
            }

            task.Name = message.Task.Name;
            task.Description = message.Task.Description;
            task.Activity = _context.Activities.SingleOrDefault(a => a.Id == message.Task.ActivityId);
            task.Tenant = _context.Tenants.SingleOrDefault(t => t.Id == message.Task.TenantId);
            task.StartDateTimeUtc = message.Task.StartDateTime.Value.DateTime;
            task.EndDateTimeUtc = message.Task.EndDateTime.Value.DateTime;
            task.RequiredSkills = message.Task.RequiredSkills.ToList();

            _context.Update(task);
            _context.SaveChanges();
            return task.Id;
        }
    }
}
