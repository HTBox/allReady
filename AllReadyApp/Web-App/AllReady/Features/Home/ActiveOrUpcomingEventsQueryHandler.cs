using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.ViewModels.Home;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Home
{
    public class ActiveOrUpcomingEventsQueryHandler : IAsyncRequestHandler<ActiveOrUpcomingEventsQuery, List<ActiveOrUpcomingEvent>>
    {
        private readonly AllReadyContext _context;
        public Func<DateTimeOffset> DateTimeOffsetUtcNow = () => DateTimeOffset.UtcNow;

        public ActiveOrUpcomingEventsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public Task<List<ActiveOrUpcomingEvent>> Handle(ActiveOrUpcomingEventsQuery message)
        {
            return _context.Events
                .AsNoTracking()
                .Include(x => x.Campaign).ThenInclude(x => x.ManagingOrganization)
                .Where(ev => ev.EndDateTime.Date >= DateTimeOffsetUtcNow().Date && 
                            !ev.Campaign.Locked && 
                             ev.Campaign.Published)
                .Select(ev => new ActiveOrUpcomingEvent()
                {
                    Id = ev.Id,
                    Name = ev.Name,
                    Description = ev.Description,
                    StartDate = ev.StartDateTime,
                    EndDate = ev.EndDateTime,
                    ImageUrl = ev.ImageUrl,
                    CampaignName = ev.Campaign.Name,
                    CampaignManagedOrganizerName = ev.Campaign.ManagingOrganization.Name
                })
                .OrderBy(ev => ev.EndDate)
                .ToListAsync();
        }
    }
}
