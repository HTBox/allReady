using System;
using System.Collections.Generic;
using AllReady.Models;
using AllReady.Services;
using Newtonsoft.Json;
using System.Linq;
using AllReady.Models.Notifications;
using Hangfire;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public interface IRequestConfirmationSmsResender
    {
        void ResendSms(List<Guid> requestId, int itineraryId);
    }

    public class RequestConfirmationSmsResender : IRequestConfirmationSmsResender
    {
        private readonly AllReadyContext context;
        private readonly IQueueStorageService storageService;
        private readonly IBackgroundJobClient backgroundJob;

        public RequestConfirmationSmsResender(AllReadyContext context, IQueueStorageService storageService, IBackgroundJobClient backgroundJob)
        {
            this.context = context;
            this.storageService = storageService;
            this.backgroundJob = backgroundJob;
        }

        public void ResendSms(List<Guid> requestIds, int itineraryId)
        {
            //we only want the requests that are still pending confirmation for the Itinerary they were added to
            var requests = context.Requests.Where(x => requestIds.Contains(x.RequestId) && x.Status == RequestStatus.PendingConfirmation).ToList();
            var itineray = context.Itineraries.Single(x => x.Id == itineraryId);

            foreach (var request in requests)
            {
                //TODO mgmccarthy: need to convert itineraryDate to local time of requestor
                var queuedSms = new QueuedSmsMessage
                {
                    Recipient = request.Phone,
                    Message = $@"Your request has been scheduled by allReady for {itineray.Date}. Please response with ""Y"" to confirm this request or ""N"" to cancel this request."
                };
                var sms = JsonConvert.SerializeObject(queuedSms);
                storageService.SendMessageAsync(QueueStorageService.Queues.SmsQueue, sms);
            }

            //schedule job for one day before ItineraryRequets.DateAssigned for the requestIds
            backgroundJob.Schedule<IRequestConfirmationSmsResender>(x => x.ResendSms(requestIds, itineray.Id), OneDayBeforeAtNoon(itineray.Date));
        }

        private static DateTime OneDayBeforeAtNoon(DateTime itineraryDate)
        {
            var oneDayBefore = itineraryDate.AddDays(-1);
            var oneDayBeforeAtNoon = new DateTime(oneDayBefore.Year, oneDayBefore.Month, oneDayBefore.Day, 12, 0, 0);
            return oneDayBeforeAtNoon;
        }
    }
}
