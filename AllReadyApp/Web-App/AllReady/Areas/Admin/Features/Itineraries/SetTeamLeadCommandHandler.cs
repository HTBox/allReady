using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AllReady.Areas.Admin.Features.Notifications;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class SetTeamLeadCommandHandler : IAsyncRequestHandler<SetTeamLeadCommand, SetTeamLeadResult>
    {
        private readonly AllReadyContext _context;
        private readonly IMediator _mediator;

        public SetTeamLeadCommandHandler(AllReadyContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<SetTeamLeadResult> Handle(SetTeamLeadCommand message)
        {
            var itineraryTaskSignups = await
                _context.VolunteerTaskSignups.Where(
                    x => x.ItineraryId == message.ItineraryId)
                    .Include(x => x.Itinerary)
                    .Include(x => x.User)
                    .ToListAsync();

            var existingTeamLead = itineraryTaskSignups.FirstOrDefault(x => x.IsTeamLead);

            if (existingTeamLead != null)
            {
                existingTeamLead.IsTeamLead = false;
            }

            VolunteerTaskSignup newTeamLead = itineraryTaskSignups.FirstOrDefault(x => x.Id == message.VolunteerTaskId);

            if (newTeamLead == null)
            {
                return SetTeamLeadResult.VolunteerTaskSignupNotFound;
            }

            newTeamLead.IsTeamLead = true;

            var changed = await _context.SaveChangesAsync();
            if (changed <= 0) return SetTeamLeadResult.SaveChangesFailed;

            await _mediator.PublishAsync(new IteneraryTeamLeadAssigned
            {
                AssigneePhone = newTeamLead.User.PhoneNumber,
                AssigneeEmail = newTeamLead.User.Email,
                ItineraryName = newTeamLead.Itinerary.Name,
                ItineraryUrl = message.ItineraryUrl
            });
            return SetTeamLeadResult.Success;
        }
    }
}
