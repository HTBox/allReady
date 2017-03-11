using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class RemoveTeamLeadCommandHandler : IAsyncRequestHandler<RemoveTeamLeadCommand, bool>
    {
        private readonly AllReadyContext _context;

        public RemoveTeamLeadCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RemoveTeamLeadCommand message)
        {
            var existingTeamLead = await
                _context.VolunteerTaskSignups.Where(
                    x => x.ItineraryId == message.ItineraryId).FirstOrDefaultAsync(x => x.IsTeamLead);

            if (existingTeamLead != null)
            {
                existingTeamLead.IsTeamLead = false;

                var changed = await _context.SaveChangesAsync();

                return changed == 0 ? false : true;
            }

            return false;
        }
    }
}