using System.Collections.Generic;
using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.Features.Events
{
    public class EventsWithUnlockedCampaignsQuery : IAsyncRequest<List<EventViewModel>>
    {
    }
}
