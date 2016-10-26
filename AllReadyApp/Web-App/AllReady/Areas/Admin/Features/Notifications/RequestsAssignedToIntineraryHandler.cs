using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Notifications
{
    public class RequestsAssignedToIntineraryHandler : IAsyncNotificationHandler<RequestsAssignedToIntinerary>
    {
        private readonly AllReadyContext context;
        private readonly ISmsSender smsSender;
        private readonly IMediator mediator;

        public RequestsAssignedToIntineraryHandler(AllReadyContext context, ISmsSender smsSender, IMediator mediator)
        {
            this.context = context;
            this.smsSender = smsSender;
            this.mediator = mediator;
        }

        public async Task Handle(RequestsAssignedToIntinerary notification)
        {
            var requestorPhoneNumbers = await context.Requests.Where(x => notification.RequestIds.Contains(x.RequestId)).Select(x => x.Phone).ToListAsync();
            var itinerary = await context.Itineraries.SingleAsync(x => x.Id == notification.ItineraryId);

            //TODO mgmccarthy: need to convert intinerary.Date to local time of the request's intinerary's campaign's timezone
            await smsSender.SendSmsAsync(requestorPhoneNumbers, 
                $@"Your request has been scheduled by allReady for {itinerary.Date.Date}. Please respond with ""Y"" to confirm this request or ""N"" to cancel this request.");

            await mediator.PublishAsync(new InitialRequestConfirmationsSent { ItineraryId = itinerary.Id, RequestIds = notification.RequestIds });
        }
    }
}