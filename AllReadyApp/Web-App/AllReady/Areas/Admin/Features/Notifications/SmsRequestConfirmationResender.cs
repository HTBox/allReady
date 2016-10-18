using System;
using AllReady.Models;
using AllReady.Services;
using Newtonsoft.Json;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public interface ISmsRequestConfirmationResender
    {
        void ResendSms(Guid requestId);
    }

    public class SmsRequestConfirmationResender : ISmsRequestConfirmationResender
    {
        private readonly AllReadyContext context;
        private readonly IQueueStorageService storageService;

        public SmsRequestConfirmationResender(AllReadyContext context, IQueueStorageService storageService)
        {
            this.context = context;
            this.storageService = storageService;
        }

        public void ResendSms(Guid requestId)
        {
            var request = context.Requests.Single(x => x.RequestId == requestId);
            if (request.Status == RequestStatus.Confirmed)
            {
                return;
            }
            
            var itinerayRequest = context.ItineraryRequests.Single(x => x.RequestId == requestId);
            
            //TODO mgmccarthy: need to convert DateAssigned to local time of requestor
            var queuedSms = new AllReady.Models.Notifications.QueuedSmsMessage
            {
                Recipient = request.Phone,
                Message = $@"Your request has been scheduled by allReady for {itinerayRequest.DateAssigned}. Please response with ""Y"" to confirm this request or ""N"" to cancel this request."
            };
            var sms = JsonConvert.SerializeObject(queuedSms);
            storageService.SendMessageAsync(QueueStorageService.Queues.SmsQueue, sms);

            //schedule job for one day before ItineraryRequets.DateAssigned for the requestId
        }
    }
}
