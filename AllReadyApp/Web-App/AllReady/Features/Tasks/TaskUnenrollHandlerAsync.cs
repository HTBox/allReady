using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Tasks
{
    public class TaskUnenrollHandlerAsync : IAsyncRequestHandler<TaskUnenrollCommand, TaskUnenrollResult>
    {
        private readonly IMediator _bus;
        private readonly AllReadyContext _context;

        public TaskUnenrollHandlerAsync(IMediator bus, AllReadyContext context)
        {
            _bus = bus;
            _context = context;
        }

        public async Task<TaskUnenrollResult> Handle(TaskUnenrollCommand message)
        {
            var taskSignUp = await _context.TaskSignups
                .Include(rec => rec.Task).ThenInclude(rec => rec.Event)
                .SingleOrDefaultAsync(a => a.User.Id == message.UserId && a.Task.Id == message.TaskId);

            if (taskSignUp == null)
            {
                return new TaskUnenrollResult { Status = "failure" };
            }

            var taskSignUps = await _context.TaskSignups.Where(a => a.User.Id == message.UserId && a.Task.Event.Id == taskSignUp.Task.Event.Id).CountAsync();

            _context.TaskSignups.Remove(taskSignUp);

            if (taskSignUps == 1)
            {
                var eventSignup = await _context.EventSignup.SingleOrDefaultAsync(u => u.User.Id == message.UserId && u.Event.Id == taskSignUp.Task.Event.Id);

                if (eventSignup != null)
                {
                    _context.EventSignup.Remove(eventSignup);
                }
            }

            await _context.SaveChangesAsync();

            await _bus.PublishAsync(new UserUnenrolls { EventId = taskSignUp.Task.Event.Id, UserId = message.UserId, TaskIds = new List<int> { taskSignUp.Task.Id } });

            return new TaskUnenrollResult { Status = "success", Task = taskSignUp.Task };
        }
    }
}
