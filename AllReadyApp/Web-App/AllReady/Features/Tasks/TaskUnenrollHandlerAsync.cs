using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Tasks
{
    public class TaskUnenrollHandlerAsync : IAsyncRequestHandler<TaskUnenrollCommand, TaskUnenrollResult>
    {
        private readonly IMediator _mediator;
        private readonly AllReadyContext _context;

        public TaskUnenrollHandlerAsync(IMediator mediator, AllReadyContext context)
        {
            _mediator = mediator;
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

            _context.TaskSignups.Remove(taskSignUp);

            await _context.SaveChangesAsync();

            await _mediator.PublishAsync(new UserUnenrolls { UserId = message.UserId, TaskId = taskSignUp.Task.Id });

            return new TaskUnenrollResult { Status = "success", Task = taskSignUp.Task };
        }
    }
}
