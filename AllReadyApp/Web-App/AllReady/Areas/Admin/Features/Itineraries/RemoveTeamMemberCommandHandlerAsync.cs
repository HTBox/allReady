using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using AllReady.Features.Notifications;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class RemoveTeamMemberCommandHandlerAsync : IAsyncRequestHandler<RemoveTeamMemberCommand, bool>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public RemoveTeamMemberCommandHandlerAsync(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<bool> Handle(RemoveTeamMemberCommand message)
        {
            var taskSignup = await _context.TaskSignups
                .FirstOrDefaultAsync(x => x.Id == message.TaskSignupId).ConfigureAwait(false);

            if (taskSignup == null)
            {
                return false;
            }

            var itineraryId = taskSignup.ItineraryId;
            taskSignup.ItineraryId = null;
            await _context.SaveChangesAsync().ConfigureAwait(false);

            await _mediator
                .PublishAsync(new IntineraryVolunteerListUpdated { TaskSignupId = message.TaskSignupId, ItineraryId = itineraryId.Value, UpdateType = UpdateType.VolnteerUnassigned })
                .ConfigureAwait(false);

            return true;
        }
    }
}