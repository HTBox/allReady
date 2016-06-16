using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Event
{
    public class GetMyEventsQuery : IRequest<MyEventsResultsScreenViewModel>
    {
        public string UserId { get; set; }
    }
}
