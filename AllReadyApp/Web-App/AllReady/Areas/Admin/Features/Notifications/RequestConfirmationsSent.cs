using System;
using System.Collections.Generic;
using MediatR;

namespace AllReady.Areas.Admin.Features.Notifications
{
    //handler for this can write pertinent info to db and then use Hangfire to schedule the follow up text if no reply is recieved within the alloted time period
    public class RequestConfirmationsSent : IAsyncNotification
    {
        //since DateAssigned is a read-only field on ItineraryRequest, can we put DateAssigned on this message? That will allow us to include it in the serialized method
        //call to Hangfire background job. This keeps us from having to query RequestItinerary for the DateAssigned property
        public List<Guid> RequestIds { get; set; }
    }
}
