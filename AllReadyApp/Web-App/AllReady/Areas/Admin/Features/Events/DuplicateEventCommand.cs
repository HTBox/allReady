using AllReady.Areas.Admin.ViewModels.Event;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class DuplicateEventCommand : IAsyncRequest<int>
    {
        public DuplicateEventViewModel DuplicateEventModel { get; set; }
    }
}
