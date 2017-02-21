using System;
using System.Collections.Generic;
using MediatR;

namespace AllReady.Areas.Admin.Features.UnlinkedRequests
{
    public class AddRequestsToEventCommand : IAsyncRequest<bool>
    {
        public int EventId { get; set; }
        public List<Guid> SelectedRequestIds { get; set; }
    }
}   
