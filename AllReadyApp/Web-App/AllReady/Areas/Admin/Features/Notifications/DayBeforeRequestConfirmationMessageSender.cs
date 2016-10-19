using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Models.Notifications;
using AllReady.Services;
using Hangfire;
using Newtonsoft.Json;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public interface IDayBeforeRequestConfirmationMessageSender
    {
        void SendSms(List<Guid> requestIds, int itineraryId);
    }

    public class DayBeforeRequestConfirmationMessageSender : IDayBeforeRequestConfirmationMessageSender
    {
        private readonly AllReadyContext context;
        private readonly IQueueStorageService storageService;
        private readonly IBackgroundJobClient backgroundJob;

        public DayBeforeRequestConfirmationMessageSender(AllReadyContext context, IQueueStorageService storageService, IBackgroundJobClient backgroundJob)
        {
            this.context = context;
            this.storageService = storageService;
            this.backgroundJob = backgroundJob;
        }

        public void SendSms(List<Guid> requestIds, int itineraryId)
        {
            var requests = context.Requests.Where(x => requestIds.Contains(x.RequestId) && x.Status == RequestStatus.PendingConfirmation).ToList();

            //TODO: mgmccarthy we only want to send messages if the itinerary is 1 or more days away from now, if it's not, return and don't process
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

            //schedule job for the day of Itinerary.Date
            //TODO: mgmccarthy: do we want to send out the "sorry you couldn't make it, we will reschedule." message in the DayOfRequestConfirmationMessageSender in the morning instead of at noon?
            backgroundJob.Schedule<IDayOfRequestConfirmationMessageSender>(x => x.SendSms(requestIds, itineray.Id), itineray.Date.AtNoon());
        }
    }
}
