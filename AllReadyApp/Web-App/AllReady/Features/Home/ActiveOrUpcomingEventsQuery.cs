using System.Collections.Generic;
using AllReady.ViewModels.Home;
using MediatR;

namespace AllReady.Features.Home
{
    public class ActiveOrUpcomingEventsQuery : IAsyncRequest<List<ActiveOrUpcomingEvent>>
    {
    }
}