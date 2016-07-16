using System.Collections.Generic;
using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.Features.Event
{
    public class EventsWithUnlockedCampaignsQuery : IRequest<List<EventViewModel>>
    {
    }
}
