using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using AllReady.Features.Notifications;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class RemoveTeamMemberCommandHandler : IAsyncRequestHandler<RemoveTeamMemberCommand, bool>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public RemoveTeamMemberCommandHandler(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<bool> Handle(RemoveTeamMemberCommand message)
        {
            var taskSignup = await _context.TaskSignups
                .FirstOrDefaultAsync(x => x.Id == message.TaskSignupId);

            if (taskSignup == null)
            {
                return false;
            }

            var itineraryId = taskSignup.ItineraryId;
            taskSignup.ItineraryId = null;
            await _context.SaveChangesAsync();

            await _mediator
                .PublishAsync(new ItineraryVolunteerListUpdated { TaskSignupId = message.TaskSignupId, ItineraryId = itineraryId.Value, UpdateType = UpdateType.VolnteerUnassigned });

            return true;
        }
    }
}