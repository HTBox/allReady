using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AllReady.Features.Notifications;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class SendInitialRequestConfirmationsHandler : IAsyncNotificationHandler<RequestsAssignedToItinerary>
    {
        private readonly AllReadyContext context;
        private readonly ISmsSender smsSender;
        private readonly IMediator mediator;

        public SendInitialRequestConfirmationsHandler(AllReadyContext context, ISmsSender smsSender, IMediator mediator)
        {
            this.context = context;
            this.smsSender = smsSender;
            this.mediator = mediator;
        }

        public async Task Handle(RequestsAssignedToItinerary notification)
        {
            var requestData = await context.Requests
                .Where(x => notification.RequestIds.Contains(x.RequestId))
                .Select(x => new {
                    phone = x.Phone,
                    requestId = x.RequestId
                }).ToListAsync();

            var itinerary = await context.Itineraries.SingleAsync(x => x.Id == notification.ItineraryId);

            //TODO mgmccarthy: need to convert itinerary.Date to local time of the request's intinerary's campaign's timezone
            await smsSender.SendSmsAsync(requestData.Select(x => x.phone).ToList(), 
                $@"Your request has been scheduled by allReady for {itinerary.Date.Date}. Please respond with ""Y"" to confirm this request or ""N"" to cancel this request.");

            foreach(var item in requestData)
            {
                await mediator.PublishAsync(NotificationSentMessage.SmsMessage(NotificationMessageType.InitialItineraryAssignment, item.phone, requestId: item.requestId, responseRequired: true));
            }

            await mediator.PublishAsync(new InitialRequestConfirmationsSent { ItineraryId = itinerary.Id, RequestIds = notification.RequestIds });
        }
    }
}