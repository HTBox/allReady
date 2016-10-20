using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.Models.Notifications;
using AllReady.Services;
using MediatR;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class RequestsAssignedToIntineraryHandler : IAsyncNotificationHandler<RequestsAssignedToIntinerary>
    {
        private readonly AllReadyContext context;
        private readonly IQueueStorageService storageService;
        private readonly IMediator mediator;

        public RequestsAssignedToIntineraryHandler(AllReadyContext context, IQueueStorageService storageService, IMediator mediator)
        {
            this.context = context;
            this.storageService = storageService;
            this.mediator = mediator;
        }

        public async Task Handle(RequestsAssignedToIntinerary notification)
        {
            var requestorPhoneNumbers = await context.Requests.Where(x => notification.RequestIds.Contains(x.RequestId)).Select(x => x.Phone).ToListAsync();
            var itinerary = await context.Itineraries.SingleAsync(x => x.Id == notification.ItineraryId);

            requestorPhoneNumbers.ForEach(async requestorPhoneNumber => 
            {
                //TODO mgmccarthy: need to convert intinerary.Date to local time of the request's intinerary's campaign's timezone
                var queuedSms = new QueuedSmsMessage
                {
                    Recipient = requestorPhoneNumber,
                    Message = $@"Your request has been scheduled by allReady for {itinerary.Date}. Please respond with ""Y"" to confirm this request or ""N"" to cancel this request."
                };
                var sms = await JsonConvert.SerializeObjectAsync(queuedSms);
                await storageService.SendMessageAsync(QueueStorageService.Queues.SmsQueue, sms);
            });
            
            await mediator.PublishAsync(new RequestConfirmationsSent { ItineraryId = itinerary.Id, RequestIds = notification.RequestIds });
        }

        public class RequestorPhoneNumberAndDateAssigned
        {
            public string PhoneNumber { get; set; }
            public DateTime DateAssigned { get; set; }
        }
    }
}