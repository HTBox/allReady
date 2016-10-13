using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.Services;
using LinqToTwitter;
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
            //for each request that has been assigend, look up requestor's phone for sms/mms
            var requestorPhoneNumbers = await context.Requests.Where(x => notification.RequestIds.Contains(x.RequestId)).Select(x => x.Phone).ToListAsync();

            var itineraryRequest = await context.ItineraryRequests.SingleAsync(x => x.ItineraryId == notification.ItineraryId);
            var dateAssigned = itineraryRequest.DateAssigned;

            //send message out for initial verification
            foreach (var phoneNumber in requestorPhoneNumbers)
            {
                //var queuedSms = new AllReady.Models.Notifications.QueuedSmsMessage
                //{
                //    Recipient = phoneNumber,
                //    Message = $@"Your request has been scheduled by allREady for {dateAssigned}. Please response with ""Y"" to confirm this request or ""N"" to cancel this request."
                //};
                //var sms = JsonConvert.SerializeObject(queuedSms);
                //await storageService.SendMessageAsync(QueueStorageService.Queues.SmsQueue, sms);
            }

            await mediator.PublishAsync(new RequestConfirmationsSent { RequestIds = notification.RequestIds});
        }
    }
}
