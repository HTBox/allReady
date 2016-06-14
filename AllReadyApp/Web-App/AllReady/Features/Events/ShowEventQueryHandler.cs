using AllReady.Models;
using AllReady.ViewModels;
using AllReady.ViewModels.Shared;
using MediatR;

namespace AllReady.Features.Event
{
    public class ShowEventQueryHandler : IRequestHandler<ShowEventQuery, EventViewModel>
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public ShowEventQueryHandler(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public EventViewModel Handle(ShowEventQuery message)
        {
            var campaignEvent = _dataAccess.GetEvent(message.EventId);

            if (campaignEvent == null || campaignEvent.Campaign.Locked)
            {
                return null;
            }

            return new EventViewModel(campaignEvent)
                .WithUserInfo(campaignEvent, message.User, _dataAccess);
        }
    }
}
