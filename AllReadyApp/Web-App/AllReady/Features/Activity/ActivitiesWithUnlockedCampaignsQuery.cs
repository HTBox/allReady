using System.Collections.Generic;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Event
{
    public class EventsWithUnlockedCampaignsQuery : IRequest<List<EventViewModel>>
    {
    }
}
