using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class TaskStatusChangeHandler : RequestHandler<TaskStatusChangeCommand>
    {
        private AllReadyContext _context;

        public TaskStatusChangeHandler(AllReadyContext context)
        {
            _context = context;
        }

        protected override void HandleCore(TaskStatusChangeCommand message)
        {
            var task = _context.Tasks.SingleOrDefault(c => c.Id == message.TaskId);
            if (task == null)
                throw new InvalidOperationException($"Task {message.TaskId} does not exist");

            var taskSignup = task.AssignedVolunteers.SingleOrDefault(c => c.User.Id == message.UserId);
            if (taskSignup == null)
                throw new InvalidOperationException($"Sign-up for user {message.UserId} does not exist");

            TaskStatus currentStatus;
            if (!Enum.TryParse<TaskStatus>(taskSignup.Status, out currentStatus))
                currentStatus = TaskStatus.Assigned;

            switch (message.TaskStatus)
            {
                case TaskStatus.Assigned:
                    break;
                case TaskStatus.Accepted:
                    if (currentStatus != TaskStatus.Assigned)
                        throw new ArgumentException($"Task must be assigned before being accepted");
                    break;
                case TaskStatus.Rejected:
                    if (currentStatus != TaskStatus.Assigned)
                        throw new ArgumentException($"Task must be assigned before being rejected");
                    break;
                case TaskStatus.Completed:
                    if (currentStatus != TaskStatus.Accepted)
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
            _context.SaveChanges();
        }
    }
}
