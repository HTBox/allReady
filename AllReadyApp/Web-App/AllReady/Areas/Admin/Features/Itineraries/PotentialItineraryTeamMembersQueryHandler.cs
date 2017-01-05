using AllReady.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskStatus = AllReady.Models.TaskStatus;

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
            return await _context.TaskSignups
                .AsNoTracking()
                .Include(x => x.Task).ThenInclude(x => x.Event)
                .Include(x => x.User)
                .Include(x => x.Itinerary)
                .Where(x => x.Task.EventId == message.EventId)
                .Where(x => x.Status == TaskStatus.Accepted)
                .Where(x => x.Itinerary == null)
                .Where(x => x.Task.StartDateTime.Date == message.Date)
                .Select(x => new SelectListItem
                {
                    Text = string.Concat(x.User.Email, " : ", x.Task.Name),
                    Value = x.Id.ToString()
                }).ToListAsync();
        }
    }
}
