using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Hangfire.MediatR;
using AllReady.Models;
using AllReady.Models.Notifications;
using AllReady.Services;
using MediatR;
using Newtonsoft.Json;

namespace AllReady.Hangfire.Jobs
{
    public class SendRequestConfirmationMessagesTheDayOfAnItineraryDate : ISendRequestConfirmationMessagesTheDayOfAnItineraryDate
    {
        private readonly AllReadyContext context;
        private readonly IQueueStorageService storageService;
        private readonly IMediator mediator;

        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public SendRequestConfirmationMessagesTheDayOfAnItineraryDate(AllReadyContext context, IQueueStorageService storageService, IMediator mediator)
        {
            this.context = context;
            this.storageService = storageService;
            this.mediator = mediator;
        }

        public void SendSms(List<Guid> requestIds, int itineraryId)
        {
            var requests = context.Requests.Where(x => requestIds.Contains(x.RequestId) && x.Status == RequestStatus.PendingConfirmation).ToList();
            var itineraryDate = context.Itineraries.Single(x => x.Id == itineraryId).Date;

            if (DateTimeUtcNow().Date == itineraryDate.Date)
            {
                //TODO mgmccarthy: need to convert intinerary.Date to local time of the request's intinerary's campaign's timezoneid. Waiting on the final word for how we'll store DateTime, as well as Issue #1386
                requests.ForEach(request =>
                {
                    var queuedSms = new QueuedSmsMessage
                    {
                        Recipient = request.Phone,
                        Message = "sorry you couldn't make it, we will reschedule."
                    };
                    var sms = JsonConvert.SerializeObject(queuedSms);
                    storageService.SendMessageAsync(QueueStorageService.Queues.SmsQueue, sms);
                });
            }
            
            mediator.Send(new SetRequstsToUnassignedCommand { RequestIds = requests.Select(x => x.RequestId).ToList() });
        }
    }

    public interface ISendRequestConfirmationMessagesTheDayOfAnItineraryDate
    {
        void SendSms(List<Guid> requestIds, int itineraryId);
    }
}