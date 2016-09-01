using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.Features.Events
{
    public class EventsWithUnlockedCampaignsQueryHandler : IRequestHandler<EventsWithUnlockedCampaignsQuery, List<EventViewModel>>
    {
        private readonly AllReadyContext dataContext;

        public EventsWithUnlockedCampaignsQueryHandler(AllReadyContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public List<EventViewModel> Handle(EventsWithUnlockedCampaignsQuery message)
        {
            return dataContext.Events.Where(c => !c.Campaign.Locked)
                .ToList() // get from SQL to C#
                .Select(a => new EventViewModel(a))
                .ToList();
        }
    }
}
