using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Features.Tasks
{
    public class TaskUnenrollHandler : IAsyncRequestHandler<TaskUnenrollCommand, TaskSignupResult>
    {
        private readonly IMediator _bus;
        private readonly AllReadyContext _context;

        public TaskUnenrollHandler(IMediator bus, AllReadyContext context)
        {
            _bus = bus;
            _context = context;
        }

        public async Task<TaskSignupResult> Handle(TaskUnenrollCommand message)
        {
            var task = _context.Tasks
                .Include(t => t.AssignedVolunteers)
                .Include(t => t.Activity).ThenInclude(a => a.UsersSignedUp)
                .Include(t => t.RequiredSkills).ThenInclude(s => s.Skill)
                .SingleOrDefault(c => c.Id == message.TaskId);

            var activity = task.Activity;

            var taskSignUp = task.AssignedVolunteers.SingleOrDefault(a => a.User.Id == message.UserId);
            if (taskSignUp != null)
            {
                task.AssignedVolunteers.Remove(taskSignUp);
            }

            if (activity.ActivityType != ActivityTypes.ActivityManaged && !activity.IsUserInAnyTask(message.UserId))
            {
                var activitySignup = activity.UsersSignedUp.FirstOrDefault(u => u.User.Id == message.UserId);
                if (activitySignup != null)
                {
                    activity.UsersSignedUp.Remove(activitySignup);
                }
            }
            
            _context.SaveChanges();

            await _bus.PublishAsync(new UserUnenrolls { ActivityId = activity.Id, UserId = message.UserId, TaskIds = new List<int> {task.Id} });

            return new TaskSignupResult {Status = "success", Task = task};
        }
    }
}
