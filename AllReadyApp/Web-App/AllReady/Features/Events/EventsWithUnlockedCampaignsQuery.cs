using System.Collections.Generic;
using AllReady.ViewModels;
using AllReady.ViewModels.Shared;
using MediatR;

namespace AllReady.Features.Event
{
    public class EventsWithUnlockedCampaignsQuery : IRequest<List<EventViewModel>>
    {
    }
}
