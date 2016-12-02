using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Requests
{
    public class ApiRequestProcessedNotificationHandler : IAsyncNotificationHandler<ApiRequestProcessedNotification>
    {
        private readonly AllReadyContext context;
        private readonly IMediator mediator;

        public ApiRequestProcessedNotificationHandler(AllReadyContext context, IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        public async Task Handle(ApiRequestProcessedNotification notification)
        {
            //UPDATE: this code is now up in the air b/c we're deciding how to associate incoming API Requetss into AllReady that do not involve picking an Event with a matching zip code.
            //for now, all SendRequestStatusToGetASmokeAlarm message sent will have a value of true on them, whic means we can service the request.
            //still waiting for feedback on whether we we'll service every Request or service none of the requests, and only change that to true when an Requst is associated with an Event in a 
            //new UI we're talking about building
            var request = await context.Requests.SingleOrDefaultAsync(x => x.ProviderId == notification.ProviderRequestId);

            await mediator.SendAsync(new SendRequestStatusToGetASmokeAlarm
            {
                Serial = notification.ProviderRequestId,
                Status = "new",
                //true if we've decided to service the Request or false if we've decided we can't service it
                Acceptance = request != null
            });
        }
    }
}