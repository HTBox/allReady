using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;
using AllReady.Features.Notifications;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class AddTeamMemberCommandHandlerAsync : IAsyncRequestHandler<AddTeamMemberCommand, bool>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public AddTeamMemberCommandHandlerAsync(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<bool> Handle(AddTeamMemberCommand message)
        {
            var itinerary = await _context.Itineraries
                .Where(x => x.Id == message.ItineraryId)
                .Select(x => new { x.EventId, x.Date })
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (itinerary == null)
            {
                // todo: sgordon: enhance this with a error message so the controller can better respond to the issue
                return false;
            }

            // We requery for potential team members in case something has changed or the task signup id was modified before posting
            var potentialTaskSignups = await _mediator.SendAsync(new PotentialItineraryTeamMembersQuery { EventId = itinerary.EventId, Date = itinerary.Date })
                .ConfigureAwait(false);

            var matchedSignup = false;
            foreach(var signup in potentialTaskSignups)
            {
                var id = int.Parse(signup.Value);
                if (id == message.TaskSignupId)
                {
                    matchedSignup = true;
                    break;
                }
            }
                        
            if (matchedSignup)
            {
                var taskSignup = new TaskSignup
                {
                    Id = message.TaskSignupId,
                    ItineraryId = message.ItineraryId
                };

                _context.TaskSignups.Attach(taskSignup);
                _context.Entry(taskSignup).Property(x => x.ItineraryId).IsModified = true;
                await _context.SaveChangesAsync().ConfigureAwait(false);

                await _mediator.PublishAsync(new VolunteerAssignedToItinerary { TaskSignupId = message.TaskSignupId, ItineraryId = message.ItineraryId})
                    .ConfigureAwait(false);
            }

            return true;
        }
    }
}