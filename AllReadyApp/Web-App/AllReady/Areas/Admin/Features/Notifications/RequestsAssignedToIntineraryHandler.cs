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
            var results = await context.ItineraryRequests.AsNoTracking()
                .Include(ir => ir.Request)
                .Where(x => notification.RequestIds.Contains(x.RequestId))
                .Select(x => new RequestorPhoneNumberAndDateAssigned
                {
                    PhoneNumber = x.Request.Phone,
                    DateAssigned = x.DateAssigned
                })
                .ToListAsync();

            foreach (var result in results)
            {
                //TODO mgmccarthy: need to convert DateAssigned to local time of requestor
                var queuedSms = new QueuedSmsMessage
                {
                    Recipient = result.PhoneNumber,
                    Message = $@"Your request has been scheduled by allReady for {result.DateAssigned}. Please response with ""Y"" to confirm this request or ""N"" to cancel this request."
                };
                var sms = JsonConvert.SerializeObject(queuedSms);
                await storageService.SendMessageAsync(QueueStorageService.Queues.SmsQueue, sms);
            }

            await mediator.PublishAsync(new RequestConfirmationsSent { RequestIds = notification.RequestIds});
        }

        public class RequestorPhoneNumberAndDateAssigned
        {
            public string PhoneNumber { get; set; }
            public DateTime DateAssigned { get; set; }
        }
    }
}