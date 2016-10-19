using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Models;
using AllReady.Models.Notifications;
using AllReady.Services;
using MediatR;
using Newtonsoft.Json;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public interface IDayOfRequestConfirmationMessageSender
    {
        void SendSms(List<Guid> requestIds, int itineraryId);
    }

    public class DayOfRequestConfirmationMessageSender : IDayOfRequestConfirmationMessageSender
    {
        private readonly AllReadyContext context;
        private readonly IQueueStorageService storageService;
        private readonly IMediator mediator;

        public DayOfRequestConfirmationMessageSender(AllReadyContext context, IQueueStorageService storageService, IMediator mediator)
        {
            this.context = context;
            this.storageService = storageService;
            this.mediator = mediator;
        }

        public void SendSms(List<Guid> requestIds, int itineraryId)
        {
            var requests = context.Requests.Where(x => requestIds.Contains(x.RequestId) && x.Status == RequestStatus.PendingConfirmation).ToList();

            //TODO: we only want to send messages if the itinerary is today
            var itineray = context.Itineraries.Single(x => x.Id == itineraryId);

            foreach (var request in requests)
            {
                var queuedSms = new QueuedSmsMessage
                {
                    Recipient = request.Phone,
                    Message = "sorry you couldn't make it, we will reschedule."
                };
                var sms = JsonConvert.SerializeObject(queuedSms);
                storageService.SendMessageAsync(QueueStorageService.Queues.SmsQueue, sms);
            }

            mediator.Send(new SetRequstsToUnassignedCommand { RequestIds = requests.Select(x => x.RequestId).ToList() });
        }
    }
}
