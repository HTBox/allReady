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
            var volunteerTaskSignup = await _context.VolunteerTaskSignups
                .Include(rec => rec.VolunteerTask).ThenInclude(rec => rec.Event)
                .SingleOrDefaultAsync(a => a.User.Id == message.UserId && a.VolunteerTask.Id == message.VolunteerTaskId);

            if (volunteerTaskSignup == null)
            {
                return new VolunteerTaskUnenrollResult { Status = "failure" };
            }

            _context.VolunteerTaskSignups.Remove(volunteerTaskSignup);

            await _context.SaveChangesAsync();

            await _mediator.PublishAsync(new UserUnenrolled { UserId = message.UserId, VolunteerTaskId = volunteerTaskSignup.VolunteerTask.Id });

            return new VolunteerTaskUnenrollResult { Status = "success", VolunteerTask = volunteerTaskSignup.VolunteerTask };
        }
    }
}
