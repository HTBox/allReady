using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class PotentialItineraryTeamMembersQueryHandler : IAsyncRequestHandler<PotentialItineraryTeamMembersQuery, IEnumerable<SelectListItem>>
    {
        private readonly AllReadyContext _context;

        public PotentialItineraryTeamMembersQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SelectListItem>> Handle(PotentialItineraryTeamMembersQuery message)
        {
            return await _context.VolunteerTaskSignups
                .AsNoTracking()
                .Include(x => x.VolunteerTask).ThenInclude(x => x.Event)
                .Include(x => x.User)
                .Include(x => x.Itinerary)
                .Where(x => x.VolunteerTask.EventId == message.EventId)
                .Where(x => x.Status == VolunteerTaskStatus.Accepted)
                .Where(x => x.Itinerary == null)
                .Where(x => x.VolunteerTask.StartDateTime.Date == message.Date)
                .Select(x => new SelectListItem
                {
                    Text = string.Concat(x.User.Email, " : ", x.VolunteerTask.Name),
                    Value = x.Id.ToString()
                }).ToListAsync();
        }
    }
}
