using System.Threading.Tasks;
using AllReady.Features.Notifications;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Tasks
{
    public class VolunteerTaskUnenrollCommandHandler : IAsyncRequestHandler<VolunteerTaskUnenrollCommand, VolunteerTaskUnenrollResult>
    {
        private readonly IMediator _mediator;
        private readonly AllReadyContext _context;

        public VolunteerTaskUnenrollCommandHandler(IMediator mediator, AllReadyContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task<VolunteerTaskUnenrollResult> Handle(VolunteerTaskUnenrollCommand message)
        {
            var taskSignUp = await _context.TaskSignups
                .Include(rec => rec.VolunteerTask).ThenInclude(rec => rec.Event)
                .SingleOrDefaultAsync(a => a.User.Id == message.UserId && a.VolunteerTask.Id == message.TaskId);

            if (taskSignUp == null)
            {
                return new VolunteerTaskUnenrollResult { Status = "failure" };
            }

            _context.TaskSignups.Remove(taskSignUp);

            await _context.SaveChangesAsync();

            await _mediator.PublishAsync(new UserUnenrolled { UserId = message.UserId, TaskId = taskSignUp.VolunteerTask.Id });

            return new VolunteerTaskUnenrollResult { Status = "success", Task = taskSignUp.VolunteerTask };
        }
    }
}
