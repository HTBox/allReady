using System;
using System.Collections.Generic;
using MediatR;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class RequestsAssignedToIntinerary : IAsyncNotification
    {
        public int ItineraryId { get; set; }
        public List<Guid> RequestIds { get; set; }
    }
}
