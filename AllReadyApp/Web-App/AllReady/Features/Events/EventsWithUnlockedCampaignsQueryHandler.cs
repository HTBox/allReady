using System.Collections.Generic;
using AllReady.Models;
using MediatR;
using System.Linq;
using AllReady.ViewModels.Event;

namespace AllReady.Features.Event
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
