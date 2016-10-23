using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Models.Notifications;
using AllReady.Services;
using Hangfire;
using Newtonsoft.Json;

namespace AllReady.Hangfire.Jobs
{
    public class DayBeforeRequestConfirmationMessageSender : IDayBeforeRequestConfirmationMessageSender
    {
        private readonly AllReadyContext context;
        private readonly IQueueStorageService storageService;
        private readonly IBackgroundJobClient backgroundJob;

        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public DayBeforeRequestConfirmationMessageSender(AllReadyContext context, IQueueStorageService storageService, IBackgroundJobClient backgroundJob)
        {
            this.context = context;
            this.storageService = storageService;
            this.backgroundJob = backgroundJob;
        }

        public void SendSms(List<Guid> requestIds, int itineraryId)
        {
            var requests = context.Requests.Where(x => requestIds.Contains(x.RequestId) && x.Status == RequestStatus.PendingConfirmation).ToList();
            var itinerary = context.Itineraries.Single(x => x.Id == itineraryId);

            //don't send out messages if DateTime.UtcNow is less than 1 days away from the Itinerary.Date. 
            //This can happen if a request is added to an itinereary less than 7 days away from the itinerary's date
            //Hangfire will execute the job immediately when the scheduled time has already passed
            if (DateTimeUtcNow().Date >= itinerary.Date.AddDays(-1).Date)
            {
                //TODO mgmccarthy: need to convert intinerary.Date to local time of the request's intinerary's campaign's timezone
                requests.ForEach(request =>
                {
                    var queuedSms = new QueuedSmsMessage
                    {
                        Recipient = request.Phone,
                        Message = $@"Your request has been scheduled by allReady for {itinerary.Date}. Please response with ""Y"" to confirm this request or ""N"" to cancel this request."
                    };
                    var sms = JsonConvert.SerializeObject(queuedSms);
                    storageService.SendMessageAsync(QueueStorageService.Queues.SmsQueue, sms);
                });
            }
            
            backgroundJob.Schedule<IDayOfRequestConfirmationMessageSender>(x => x.SendSms(requestIds, itinerary.Id), itinerary.Date.AtNoon());
        }
    }

    public interface IDayBeforeRequestConfirmationMessageSender
    {
        void SendSms(List<Guid> requestIds, int itineraryId);
    }
}