using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Hangfire.Jobs
{
    public class SendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDate : ISendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDate
    {
        private readonly AllReadyContext context;
        private readonly IBackgroundJobClient backgroundJob;
        private readonly ISmsSender smsSender;

        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public SendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDate(AllReadyContext context, IBackgroundJobClient backgroundJob, ISmsSender smsSender)
        {
            this.context = context;
            this.backgroundJob = backgroundJob;
            this.smsSender = smsSender;
        }

        public void SendSms(List<Guid> requestIds, int itineraryId)
        {
            var requestorPhoneNumbers = context.Requests.Where(x => requestIds.Contains(x.RequestId) && x.Status == RequestStatus.PendingConfirmation).Select(x => x.Phone).ToList();
            if (requestorPhoneNumbers.Count > 0)
            {
                var itinerary = context.Itineraries.Include(i => i.Event).Single(x => x.Id == itineraryId);

                //don't send out messages if today is not 7 days before the Itinerary.Date. This sceanrio can happen if:
                //1. a request is added to an itinereary less than 7 days before the itinerary's date
                //2. if the Hangfire server is offline for the period where it would have tried to process this job. Hangfire processes jobs in the "past" by default
                if (TodayIsSevenDaysBeforeThe(itinerary.Date))
                {
                    smsSender.SendSmsAsync(requestorPhoneNumbers,
                        $@"Your request has been scheduled by allReady for {itinerary.Date.Date}. Please response with ""Y"" to confirm this request or ""N"" to cancel this request.");
                }

                //schedule job for one day before Itinerary.Date
                backgroundJob.Schedule<ISendRequestConfirmationMessagesADayBeforeAnItineraryDate>(x => x.SendSms(requestIds, itinerary.Id), OneDayBefore(itinerary.Date, itinerary.Event.TimeZoneId));
            }
        }

        private bool TodayIsSevenDaysBeforeThe(DateTime itineraryDate)
        {
            return (itineraryDate.Date - DateTimeUtcNow().Date).TotalDays == 7;
        }

        private static DateTimeOffset OneDayBefore(DateTime itineraryDate, string eventsTimeZoneId)
        {
            var oneDayAgoAtNoon = itineraryDate.Date.AddDays(-1).AddHours(12);
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(eventsTimeZoneId);
            var utcOffset = timeZoneInfo.GetUtcOffset(oneDayAgoAtNoon);
            return new DateTimeOffset(oneDayAgoAtNoon, utcOffset);
        }
    }

    public interface ISendRequestConfirmationMessagesSevenDaysBeforeAnItineraryDate
    {
        void SendSms(List<Guid> requestIds, int itineraryId);
    }
}