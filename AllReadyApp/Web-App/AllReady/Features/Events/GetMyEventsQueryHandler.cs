using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.Features.Event
{
    public class GetMyEventsQueryHandler : IRequestHandler<GetMyEventsQuery, MyEventsResultsScreenViewModel>
    {
        private readonly IAllReadyDataAccess _allReadyDataAccess;

        public GetMyEventsQueryHandler(IAllReadyDataAccess allReadyDataAccess)
        {
            _allReadyDataAccess = allReadyDataAccess;
        }


        public MyEventsResultsScreenViewModel Handle(GetMyEventsQuery message)
        {
            var myEvents = _allReadyDataAccess.GetEventSignups(message.UserId).Where(a => !a.Event.Campaign.Locked);
            var signedUp = myEvents.Select(a => new EventViewModel(a.Event)).ToList();
            return new MyEventsResultsScreenViewModel("My Events", signedUp);
        }
    }
}
