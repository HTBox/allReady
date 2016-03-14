using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class TaskStatusChangeHandlerAsync : IAsyncRequestHandler<TaskStatusChangeCommandAsync, TaskChangeResult>
    {
        private AllReadyContext _context;
        private IMediator _mediator;

        public TaskStatusChangeHandlerAsync(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TaskChangeResult> Handle(TaskStatusChangeCommandAsync message)
        {
            var task = await GetTask(message).ConfigureAwait(false);

            if (task == null)
                throw new InvalidOperationException($"Task {message.TaskId} does not exist");

            var taskSignup = task.AssignedVolunteers.SingleOrDefault(c => c.User.Id == message.UserId);
            if (taskSignup == null)
                throw new InvalidOperationException($"Sign-up for user {message.UserId} does not exist");

            TaskStatus currentStatus;
            if (!Enum.TryParse(taskSignup.Status, out currentStatus))
                currentStatus = TaskStatus.Assigned;

            switch (message.TaskStatus)
            {
                case TaskStatus.Assigned:
                    break;
                case TaskStatus.Accepted:
                    if (currentStatus != TaskStatus.Assigned && currentStatus != TaskStatus.CanNotComplete && currentStatus != TaskStatus.Completed) 
                        throw new ArgumentException($"Task must be assigned before being accepted or undoing CanNotComplete or Completed");
                    break;
                case TaskStatus.Rejected:
                    if (currentStatus != TaskStatus.Assigned)
                        throw new ArgumentException($"Task must be assigned before being rejected");
                    break;
                case TaskStatus.Completed:
                    if (currentStatus != TaskStatus.Accepted && currentStatus != TaskStatus.Assigned)
                        throw new ArgumentException($"Task must be accepted before being completed");
                    break;
                case TaskStatus.CanNotComplete:
                    if (currentStatus != TaskStatus.Accepted && currentStatus != TaskStatus.Assigned)
                        throw new ArgumentException($"Task must be assigned or accepted before it can be marked as {message.TaskStatus}");
                    break;
                default:
                    throw new ArgumentException($"Invalid sign-up status value: {message.TaskStatus}");
            }

            taskSignup.Status = message.TaskStatus.ToString();
            taskSignup.StatusDateTimeUtc = DateTime.UtcNow;
            taskSignup.StatusDescription = message.TaskStatusDescription;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            var notification = new TaskSignupStatusChanged { SignupId = taskSignup.Id };
            await _mediator.PublishAsync(notification).ConfigureAwait(false);
            
            return new TaskChangeResult { Status = "success", Task = task };
        }

        private async Task<AllReadyTask> GetTask(TaskStatusChangeCommand message)
        {
            return await _context.Tasks
                .Include(t => t.AssignedVolunteers).ThenInclude(ts => ts.User)
                .Include(t => t.RequiredSkills).ThenInclude(s => s.Skill)
                .SingleOrDefaultAsync(c => c.Id == message.TaskId)
                .ConfigureAwait(false);
        }
    }

    public class TaskChangeResult
    {
        public string Status { get; set; }
        public AllReadyTask Task { get; set; }
    }
}
