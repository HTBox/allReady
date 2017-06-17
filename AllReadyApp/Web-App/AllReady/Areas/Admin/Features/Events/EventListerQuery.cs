using AllReady.Areas.Admin.ViewModels.Event;
using MediatR;
using System.Collections.Generic;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EventListerQuery : IAsyncRequest<List<EventListerViewModel>>        
    {
        public string UserId { get; set; }
    }
}