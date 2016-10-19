using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Models.Notifications;
using AllReady.Services;
using Hangfire;
using Newtonsoft.Json;

namespace AllReady.Areas.Admin.RequestConfirmationMessageSenders
{
    public interface IWeekBeforeRequestConfirmationMessageSender
    {
        void SendSms(List<Guid> requestIds, int itineraryId);
    }

    public class WeekBeforeRequestConfirmationMessageSender : IWeekBeforeRequestConfirmationMessageSender
    {
        private readonly AllReadyContext context;
        private readonly IQueueStorageService storageService;
        private readonly IBackgroundJobClient backgroundJob;

        public WeekBeforeRequestConfirmationMessageSender(AllReadyContext context, IQueueStorageService storageService, IBackgroundJobClient backgroundJob)
        {
            this.context = context;
            this.storageService = storageService;
            this.backgroundJob = backgroundJob;
        }

        public void SendSms(List<Guid> requestIds, int itineraryId)
        {
            var requests = context.Requests.Where(x => requestIds.Contains(x.RequestId) && x.Status == RequestStatus.PendingConfirmation).ToList();

            //TODO: mgmccarthy we only want to send messages if the itinerary is 7 or more days away from now, if it's not, return and don't process
            var itinerary = context.Itineraries.Single(x => x.Id == itineraryId);

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

            //schedule job for one day before Itinerary.Date
            backgroundJob.Schedule<IDayBeforeRequestConfirmationMessageSender>(x => x.SendSms(requestIds, itinerary.Id), itinerary.Date.AddDays(-1).AtNoon());
        }
    }
}