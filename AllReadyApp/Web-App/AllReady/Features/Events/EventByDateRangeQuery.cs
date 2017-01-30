using System;
using System.Collections.Generic;
using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.Features.Events
{
    public class EventByDateRangeQuery : IAsyncRequest<IEnumerable<EventViewModel>>
    {
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset StartDate { get; set; }
    }
}