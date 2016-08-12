using AllReady.Areas.Admin.ViewModels.Event;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class EditEventCommand : IAsyncRequest<int>
    {
        public EventEditModel Event {get; set;}
    }
}
