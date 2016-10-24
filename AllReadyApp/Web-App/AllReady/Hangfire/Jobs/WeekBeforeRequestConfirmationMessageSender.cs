using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Services;
using Hangfire;

namespace AllReady.Hangfire.Jobs
{
    public class SendRequestConfirmationMessagesAWeekBeforeAnItineraryDate : ISendRequestConfirmationMessagesAWeekBeforeAnItineraryDate
    {
        private readonly AllReadyContext context;
        private readonly IBackgroundJobClient backgroundJob;
        private readonly ISmsSender smsSender;

        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public SendRequestConfirmationMessagesAWeekBeforeAnItineraryDate(AllReadyContext context, IBackgroundJobClient backgroundJob, ISmsSender smsSender)
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
                //TODO mgmccarthy: need to convert intinerary.Date to local time of the request's intinerary's campaign's timezoneid. Waiting on the final word for how we'll store DateTime, as well as Issue #1386
                var itinerary = context.Itineraries.Single(x => x.Id == itineraryId);

                //don't send out messages if today is less than 7 days away from the Itinerary.Date.
                //This can happen if a request is added to an itinereary less than 7 days away from the itinerary's date
                if (TodayIsEqualToOrGreaterThanSevenDaysBefore(itinerary.Date))
                {
                    smsSender.SendSmsAsync(requestorPhoneNumbers,
                        $@"Your request has been scheduled by allReady for {itinerary.Date.Date}. Please response with ""Y"" to confirm this request or ""N"" to cancel this request.");
                }

                //schedule job for one day before Itinerary.Date
                backgroundJob.Schedule<ISendRequestConfirmationMessagesADayBeforeAnItineraryDate>(x => x.SendSms(requestIds, itinerary.Id), itinerary.Date.AddDays(-1).AtNoon());
            }
        }

        private bool TodayIsEqualToOrGreaterThanSevenDaysBefore(DateTime itineraryDate)
        {
            return (itineraryDate.Date - DateTimeUtcNow().Date).TotalDays >= 7;
        }
    }

    public interface ISendRequestConfirmationMessagesAWeekBeforeAnItineraryDate
    {
        void SendSms(List<Guid> requestIds, int itineraryId);
    }
}