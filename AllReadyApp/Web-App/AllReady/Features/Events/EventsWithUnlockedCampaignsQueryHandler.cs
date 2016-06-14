using System.Collections.Generic;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using System.Linq;
using AllReady.ViewModels.Shared;

namespace AllReady.Features.Event
{
    public class EventsWithUnlockedCampaignsQueryHandler : IRequestHandler<EventsWithUnlockedCampaignsQuery, List<EventViewModel>>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public EventsWithUnlockedCampaignsQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public List<EventViewModel> Handle(EventsWithUnlockedCampaignsQuery message)
        {
            return dataAccess.Events.Where(c => !c.Campaign.Locked)
                .Select(a => new EventViewModel(a))
                .ToList();
        }
    }
}
