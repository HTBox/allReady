using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EditEventCommand : IAsyncRequest<int>
    {
        public EventDetailModel Event {get; set;}
    }
}
