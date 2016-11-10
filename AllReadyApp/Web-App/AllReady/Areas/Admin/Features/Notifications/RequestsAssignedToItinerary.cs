using System;
using System.Collections.Generic;
using MediatR;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class RequestsAssignedToItinerary : IAsyncNotification
    {
        //TODO mgmccarthy: do we need to carry ItineraryId on here? This notification is published from AddRequestsToItineraryCommandHandler, which adds all requests to the same itinerary, so the itinerary will always remain the same for this grouping of requests
        public int ItineraryId { get; set; }
        public List<Guid> RequestIds { get; set; }
    }
}
