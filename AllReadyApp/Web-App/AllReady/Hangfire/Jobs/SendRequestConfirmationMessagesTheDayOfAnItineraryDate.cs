using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Hangfire.MediatR;
using AllReady.Models;
using AllReady.Services;
using MediatR;

namespace AllReady.Hangfire.Jobs
{
    public class SendRequestConfirmationMessagesTheDayOfAnItineraryDate : ISendRequestConfirmationMessagesTheDayOfAnItineraryDate
    {
        private readonly AllReadyContext context;
        private readonly ISmsSender smsSender;
        private readonly IMediator mediator;

        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public SendRequestConfirmationMessagesTheDayOfAnItineraryDate(AllReadyContext context, ISmsSender smsSender, IMediator mediator)
        {
            this.context = context;
            this.smsSender = smsSender;
            this.mediator = mediator;
        }

        public void SendSms(List<Guid> requestIds, int itineraryId)
        {
            var requests = context.Requests.Where(x => requestIds.Contains(x.RequestId) && x.Status == RequestStatus.PendingConfirmation).ToList();
            if (requests.Count > 0)
            {
                //TODO mgmccarthy: need to convert itinerary.Date to local time of the request's intinerary's campaign's timezoneid. Waiting on the final word for how we'll store DateTime, as well as Issue #1386
                var itineraryDate = context.Itineraries.Single(x => x.Id == itineraryId).Date;

                //don't send out messages if today is not the date of the Itinerary
                if (DateTimeUtcNow().Date == itineraryDate.Date)
                {
                    smsSender.SendSmsAsync(requests.Select(x => x.Phone).ToList(), "sorry you couldn't make it, we will reschedule.");
                }

                mediator.Send(new SetRequestsToUnassignedCommand { RequestIds = requests.Select(x => x.RequestId).ToList() });
            }
        }
    }

    public interface ISendRequestConfirmationMessagesTheDayOfAnItineraryDate
    {
        void SendSms(List<Guid> requestIds, int itineraryId);
    }
}