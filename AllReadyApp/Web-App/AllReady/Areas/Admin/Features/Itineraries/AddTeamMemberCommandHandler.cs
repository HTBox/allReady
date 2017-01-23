using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using System.Linq;
using AllReady.Features.Notifications;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class AddTeamMemberCommandHandler : IAsyncRequestHandler<AddTeamMemberCommand, bool>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public AddTeamMemberCommandHandler(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<bool> Handle(AddTeamMemberCommand message)
        {
            var itinerary = await _context.Itineraries
                .Where(x => x.Id == message.ItineraryId)
                .Select(x => new { x.EventId, x.Date })
                .SingleOrDefaultAsync();

            if (itinerary == null)
            {
                return false;
            }

            // We requery for potential team members in case something has changed or the task signup id was modified before posting
            var potentialTaskSignups = await _mediator.SendAsync(new PotentialItineraryTeamMembersQuery { EventId = itinerary.EventId, Date = itinerary.Date });

            var matchedSignup = false;
            foreach(var signup in potentialTaskSignups)
            {
                var id = int.Parse(signup.Value);
                if (id == message.VolunteerTaskSignupId)
                {
                    matchedSignup = true;
                    break;
                }
            }
                        
            if (matchedSignup)
            {
                var volunteerTaskSignup = new VolunteerTaskSignup
                {
                    Id = message.VolunteerTaskSignupId,
                    ItineraryId = message.ItineraryId
                };

                _context.VolunteerTaskSignups.Attach(volunteerTaskSignup);
                _context.Entry(volunteerTaskSignup).Property(x => x.ItineraryId).IsModified = true;
                await _context.SaveChangesAsync();

                await _mediator
                    .PublishAsync(new ItineraryVolunteerListUpdated { VolunteerTaskSignupId = message.VolunteerTaskSignupId, ItineraryId = message.ItineraryId, UpdateType = UpdateType.VolunteerAssigned });
            }

            return true;
        }
    }
}