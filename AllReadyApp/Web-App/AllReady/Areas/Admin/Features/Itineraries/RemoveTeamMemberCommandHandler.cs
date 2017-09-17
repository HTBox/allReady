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
            var volunteerTaskSignup = await _context.VolunteerTaskSignups
                .FirstOrDefaultAsync(x => x.Id == message.VolunteerTaskSignupId);

            if (volunteerTaskSignup == null)
            {
                return false;
            }

            var itineraryId = volunteerTaskSignup.ItineraryId;
            volunteerTaskSignup.ItineraryId = null;
            volunteerTaskSignup.IsTeamLead = false;
            await _context.SaveChangesAsync();

            await _mediator
                .PublishAsync(new ItineraryVolunteerListUpdated { VolunteerTaskSignupId = message.VolunteerTaskSignupId, ItineraryId = itineraryId.Value, UpdateType = UpdateType.VolnteerUnassigned });

            return true;
        }
    }
}