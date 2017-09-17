using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class SetTeamLeadCommandHandler : IAsyncRequestHandler<SetTeamLeadCommand, SetTeamLeadResult>
    {
        private readonly AllReadyContext _context;

        public SetTeamLeadCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<SetTeamLeadResult> Handle(SetTeamLeadCommand message)
        {
            var itineraryTaskSignups = await
                _context.VolunteerTaskSignups.Where(
                    x => x.ItineraryId == message.ItineraryId).ToListAsync();

            var existingTeamLead = itineraryTaskSignups.FirstOrDefault(x => x.IsTeamLead);

            if (existingTeamLead != null)
            { 
                existingTeamLead.IsTeamLead = false;
            }

            var newTeamLead = itineraryTaskSignups.FirstOrDefault(x => x.Id == message.VolunteerTaskId);

            if (newTeamLead == null)
            {
                return SetTeamLeadResult.VolunteerTaskSignupNotFound;
            }

            newTeamLead.IsTeamLead = true;

            var changed = await _context.SaveChangesAsync();

            return changed == 0 ? SetTeamLeadResult.SaveChangesFailed : SetTeamLeadResult.Success;
        }
    }
}
