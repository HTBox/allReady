using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Features.Requests;
using AllReady.Models;
using AllReady.Services;
using MediatR;
using TimeZoneConverter;
using Microsoft.EntityFrameworkCore;

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
                var itinerary = context.Itineraries.Include(i => i.Event).Single(x => x.Id == itineraryId);

                //don't send out messages if today is not the date of the Itinerary
                if (TodayIsTheDayOfThe(itinerary.Date, itinerary.Event.TimeZoneId))
                {
                    smsSender.SendSmsAsync(requests.Select(x => x.Phone).ToList(), "sorry you couldn't make it, we will reschedule.");
                }

                mediator.Publish(new DayOfRequestConfirmationsSent { RequestIds = requests.Select(x => x.RequestId).ToList() });
            }
        }

        private bool TodayIsTheDayOfThe(DateTime itineraryDate, string eventsTimeZoneId)
        {
            var timeZoneInfo = TZConvert.GetTimeZoneInfo(eventsTimeZoneId);
            var utcOffset = timeZoneInfo.GetUtcOffset(itineraryDate);
            var intineraryDateConvertedToEventsTimeZone = new DateTimeOffset(itineraryDate, utcOffset);
            return (intineraryDateConvertedToEventsTimeZone.Date - DateTimeUtcNow().Date).TotalDays == 0;
        }
    }

    public interface ISendRequestConfirmationMessagesTheDayOfAnItineraryDate
    {
        void SendSms(List<Guid> requestIds, int itineraryId);
    }
}
