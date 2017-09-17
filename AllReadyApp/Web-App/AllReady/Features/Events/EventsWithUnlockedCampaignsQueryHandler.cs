using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.ViewModels.Event;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Events
{
    public class EventsWithUnlockedCampaignsQueryHandler : IAsyncRequestHandler<EventsWithUnlockedCampaignsQuery, List<EventViewModel>>
    {
        private readonly AllReadyContext dataContext;

        public EventsWithUnlockedCampaignsQueryHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<List<EventViewModel>> Handle(EventsWithUnlockedCampaignsQuery message)
        {
            var @events = await dataContext.Events
                .Where(c => !c.Campaign.Locked)
                .Include(c => c.Campaign)
                .Include(c => c.Campaign.ManagingOrganization)
                .Include(c => c.Campaign.Location)
                .Include(c => c.VolunteerTasks)
                .ToListAsync();

            return @events.Select(@event => new EventViewModel(@event)).ToList();
        }
    }
}
